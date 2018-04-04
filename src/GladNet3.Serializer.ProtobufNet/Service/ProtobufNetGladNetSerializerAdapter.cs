using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using Reinterpret.Net;

namespace GladNet
{
	/// <summary>
	/// Adapter around the Marc Garvell's Protobuf-Net.
	/// </summary>
	public sealed class ProtobufNetGladNetSerializerAdapter : INetworkSerializationService
	{
		/// <summary>
		/// The protobuf-net message prefixing style.
		/// </summary>
		public PrefixStyle PrefixStyle { get; }

		public ProtobufNetGladNetSerializerAdapter(PrefixStyle prefixStyle = PrefixStyle.None)
		{
			if(!Enum.IsDefined(typeof(PrefixStyle), prefixStyle)) throw new ArgumentOutOfRangeException(nameof(prefixStyle), "Value should be defined in the PrefixStyle enum.");

			PrefixStyle = prefixStyle;
		}

		//TODO: Should we provide a buffer to write into?
		/// <inheritdoc />
		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data)
		{
			//TODO: Should we reuse the memory stream/provide buffer?
			using(MemoryStream stream = new MemoryStream())
			{
				if(PrefixStyle == PrefixStyle.None)
					ProtoBuf.Serializer.Serialize(stream, data);
				else
					Serializer.SerializeWithLengthPrefix(stream, data, PrefixStyle);

				//This is important to push the stream back
				stream.Position = 0;

				//TODO: Should we copy this to a buffer? Or use a provided buffer?
				return stream.ToArray();
			}
		}

		/// <inheritdoc />
		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] buffer, int start, int count)
		{
			if(PrefixStyle == PrefixStyle.None)
				return Serializer.Deserialize<TTypeToDeserializeTo>(new MemoryStream(buffer, start, count, false));

			return Serializer.DeserializeWithLengthPrefix<TTypeToDeserializeTo>(new MemoryStream(buffer, start, count, false), PrefixStyle);
		}

		/// <inheritdoc />
		public async Task<TTypeToDeserializeTo> DeserializeAsync<TTypeToDeserializeTo>(IBytesReadable bytesReadable, CancellationToken token)
		{
			//TODO: Implement no prefixing support
			if(PrefixStyle == PrefixStyle.None)
				throw new NotSupportedException("Protobuf-Net deserialization without length prefixing is not yet supported.");

			//To do the async read operation with Protobuf-Net we need to do some manual buffering
			int prefixSize = checked((int)await ReadLongLengthPrefix(bytesReadable, PrefixStyle, token).ConfigureAwait(false));

			Console.WriteLine($"Prefix size: {prefixSize}");

			//TODO: Reduce allocations somehow
			byte[] bytes = new byte[prefixSize];

			int count = await bytesReadable.ReadAsync(bytes, 0, prefixSize, token)
				.ConfigureAwait(false);

			//0 means that the socket disconnected
			if(count == 0)
				return default(TTypeToDeserializeTo);

			return Serializer.Deserialize<TTypeToDeserializeTo>(new MemoryStream(bytes));
		}

		//From: https://github.com/mgravell/protobuf-net/blob/38a2d0b6095dad08c57ef5bd7dc821643a86a4a1/src/protobuf-net/ProtoReader.cs
		/// <summary>
		/// Reads the length-prefix of a message from a stream without buffering additional data, allowing a fixed-length
		/// reader to be created.
		/// </summary>
		public static async Task<long> ReadLongLengthPrefix(IBytesReadable bytesReadable, PrefixStyle style, CancellationToken token)
		{
			switch(style)
			{
				case PrefixStyle.Base128:
					// check for a length
					return (long)await TryReadUInt64Variant(bytesReadable, token);
				case PrefixStyle.Fixed32:
					{
						//TODO: Figure out a way to reduce allocations
						byte[] bytes = await ReadFixed4Byte(bytesReadable, style, token)
							.ConfigureAwait(false);

						//Means the socket disconnected
						if(bytes == null)
							return 0;

						if(BitConverter.IsLittleEndian)
						{
							return bytes.Reinterpret<int>();
						}
						else
							throw new NotSupportedException("TODO: Implement big endian prefix handling.");
					}
				case PrefixStyle.Fixed32BigEndian:
				{
					//TODO: Figure out a way to reduce allocations
					byte[] bytes = await ReadFixed4Byte(bytesReadable, style, token)
						.ConfigureAwait(false);

					//Means the socket disconnected
					if(bytes == null)
						return 0;
					
					//TODO: Improve efficiency
					return (bytes[0] << 24)
						| (bytes[1] << 16)
						| (bytes[2] << 8)
						| bytes[3];
				}
				default:
					throw new ArgumentOutOfRangeException(nameof(style));
			}
		}

		//TODO: Doc
		/// <summary>
		/// Asyncly reads a byte chunk from the <see cref="bytesReadable"/>
		/// or throws if unable.
		/// </summary>
		/// <param name="bytesReadable"></param>
		/// <param name="style"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private static async Task<byte[]> ReadFixed4Byte(IBytesReadable bytesReadable, PrefixStyle style, CancellationToken token)
		{
			byte[] bytes = new byte[4];
			int count = await bytesReadable.ReadAsync(bytes, 0, 4, token)
				.ConfigureAwait(false);

			//0 means the socket disconnected
			if(count == 0)
				return null;

			if(count < 4)
				throw new InvalidOperationException($"Protobuf-Net could not read length prefix: {style}");

			return bytes;
		}

		/// <returns>The number of bytes consumed; 0 if no data available</returns>
		private static async Task<ulong> TryReadUInt64Variant(IBytesReadable bytesReadable, CancellationToken token)
		{
			//TODO: Reuse-share buffer to reduce allocations
			byte[] tempBuffer = new byte[9];

			ulong value = 0;
			int count = await bytesReadable.ReadAsync(tempBuffer, 0, 1, token)
				.ConfigureAwait(false);

			//0 means that the socket disconnected
			if(count == 0)
				return 0;

			int b = tempBuffer[0];

			if(b < 0) { return 0; }

			value = (uint)b;

			if((value & 0x80) == 0) { return 1; }

			value &= 0x7F;

			int bytesRead = 1, shift = 7;

			while(bytesRead < 9)
			{
				count = await bytesReadable.ReadAsync(tempBuffer, bytesRead, 1, token)
					.ConfigureAwait(false);

				//0 means the underlying socket disconnected
				if(count == 0)
					return 0;

				b = tempBuffer[bytesRead];

				if(b < 0) throw EoF(null);

				value |= ((ulong)b & 0x7F) << shift;
				shift += 7;

				if((b & 0x80) == 0) return value;

				bytesRead++;
			}

			count = await bytesReadable.ReadAsync(tempBuffer, bytesRead, 1, token)
				.ConfigureAwait(false);

			if(count == 0)
				return 0;

			b = tempBuffer[bytesRead];

			if(b < 0) throw EoF(null);

			if((b & 1) == 0) // only use 1 bit from the last byte
			{
				value |= ((ulong)b & 0x7F) << shift;
			}

			return value;
		}

		private static Exception EoF(ProtoReader source)
		{
			return AddErrorData(new EndOfStreamException(), source);
		}

		internal static Exception AddErrorData(Exception exception, ProtoReader source)
		{
#if !CF && !FX11 && !PORTABLE
			if(exception != null && source != null && !exception.Data.Contains("protoSource"))
			{
				//We removed depth because it's not exposed in any way
				exception.Data.Add("protoSource", $"tag={source.FieldNumber}; wire-type={source.WireType}; offset={source.LongPosition};");
			}
#endif
			return exception;
		}
	}
}
