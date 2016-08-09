using GladNet.Payload.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

//hijack namespace for easier extension
namespace GladNet.Serializer
{
	public static class SerializationRegistryExtensions
	{
		/// <summary>
		/// Register the <see cref="GladNet.Payload.Authentication"/> payloads with the <see cref="ISerializerRegistry"/>.
		/// </summary>
		/// <param name="registry">Registry to register the payloads to.</param>
		/// <returns>The registry for fluent chaining.</returns>
		public static ISerializerRegistry RegisterAutenticationPayloads(this ISerializerRegistry registry)
		{
			//Register the payloads.
			registry.Register(typeof(AuthenticationRequest));
			registry.Register(typeof(AuthenticationResponse));

			//return for fluent chaining.
			return registry;
		}
	}
}
