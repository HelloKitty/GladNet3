using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GladNet.Payload.Authentication
{
	/// <summary>
	/// GladNet visible version of a JWT/oAuth authentication response.
	/// </summary>
	[GladNetSerializationContract]
	[GladNetSerializationInclude((int)GladNetIncludeInternalIndex.AuthenticationResponse, typeof(PacketPayload), false)] //use the internal ctor
	public class AuthenticationResponse : PacketPayload
	{
		/// <summary>
		/// Optional oAuth2 response code.
		/// Refer to spec on page 27-28.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index1)]
		public ResponseErrorCode? OptionalError { get; private set; }

		/// <summary>
		/// Optional error message if the <see cref="OptionalError"/> <see cref="ResponseErrorCode"/> is set.
		/// Otherwise empty or null dependending on serialization implementation.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index2)]
		public string OptionalErrorMessage { get; private set; }

		/// <summary>
		/// Indicates if the <see cref="AuthenticationResponse"/> is a successful response.
		/// </summary>
		//If there is no error value we should assume the response is successful.
		public bool AuthenticationSuccessful { get { return !OptionalError.HasValue; } }

		/// <summary>
		/// Optional ctor for constructing an errored response.
		/// </summary>
		/// <param name="responseCode">Error code (reason).</param>
		/// <param name="errorMessage">Human readable reason message.</param>
		public AuthenticationResponse(ResponseErrorCode responseCode, string errorMessage)
		{
			OptionalError = responseCode;
			OptionalErrorMessage = errorMessage;
		}

		/// <summary>
		/// Constructrs a successful response.
		/// </summary>
		public AuthenticationResponse()
		{
				
		}
	}
}
