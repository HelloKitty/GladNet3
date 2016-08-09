using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GladNet.Payload.Authentication
{
	/// <summary>
	/// Framework/spec agnostic Authentication request.
	/// </summary>
	[GladNetSerializationContract]
	[GladNetSerializationInclude((int)GladNetIncludeInternalIndex.AuthenticationRequest, typeof(PacketPayload), false)] //use the internal ctor
	public class AuthenticationRequest : PacketPayload
	{
		/// <summary>
		/// Username credential for authentication.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index1)]
		public string UserName { get; private set; }

		/// <summary>
		/// Password credential for authentication.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index2)]
		public string Password { get; private set; }

		/// <summary>
		/// Creates a new wire-ready authentication request viable for most authentication frameworks/specs.
		/// </summary>
		/// <param name="userName">The username credential to use.</param>
		/// <param name="password">The password credential to use.</param>
		public AuthenticationRequest(string userName, string password)
		{
			if (String.IsNullOrEmpty(userName))
				throw new ArgumentException($"Provided {nameof(userName)} value is not valid. Must be non-empty and not null.", nameof(userName));

			if (String.IsNullOrEmpty(password))
				throw new ArgumentException($"Provided {nameof(password)} value is not valid. Must be non-empty and not null.", nameof(password));

			UserName = userName;
			Password = password;
		}

		/// <summary>
		/// Protected serialization ctor.
		/// </summary>
		protected AuthenticationRequest()
		{

		}
	}
}
