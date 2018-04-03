using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using JetBrains.Annotations;

namespace GladNet
{
	/// <summary>
	/// Adapter around the FreecraftCore <see cref="ISerializerService"/>.
	/// </summary>
	public sealed class FreecraftCoreGladNetSerializerAdapter : INetworkSerializationService
	{
		/// <summary>
		/// The FreecraftCore serializer that should be adapted to the GladNet3 API.
		/// </summary>
		private ISerializerService Serializer { get; }

		public FreecraftCoreGladNetSerializerAdapter([NotNull] ISerializerService serializer)
		{
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));

			Serializer = serializer;
		}

		/// <inheritdoc />
		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data)
		{
			return Serializer.Serialize(data);
		}

		/// <inheritdoc />
		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] buffer, int start, int count)
		{
			return Serializer.Deserialize<TTypeToDeserializeTo>(new FixedBufferWireReaderStrategy(buffer, start, count));
		}

		/// <inheritdoc />
		public Task<TTypeToDeserializeTo> DeserializeAsync<TTypeToDeserializeTo>(IBytesReadable bytesReadable, CancellationToken token)
		{
			//We have to manually add peek buffering 
			return Serializer.DeserializeAsync<TTypeToDeserializeTo>(new AsyncWireReaderBytesReadableAdapter(bytesReadable)
					.PeekWithBufferingAsync())
;
		}
	}
}
