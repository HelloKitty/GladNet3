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
		public int Serialize<TTypeToSerialize>(TTypeToSerialize data, Span<byte> buffer)
		{
			int offset = 0;
			Serializer.Write(data, buffer, ref offset);
			return offset;
		}

		/// <inheritdoc />
		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(Span<byte> buffer)
		{
			throw new NotImplementedException();
		}
	}
}
