using GladNet.Common;
using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	public static class IStaticPayloadParametersExt
	{
		public static bool Verify(this IStaticPayloadParameters actualParameters, IMessageParameters expectedParameters)
		{
			if (actualParameters == null) throw new ArgumentNullException(nameof(actualParameters), $"{nameof(actualParameters)} cannot be null for {nameof(Verify)} extension method.");
			if (expectedParameters == null) throw new ArgumentNullException(nameof(expectedParameters), $"{nameof(expectedParameters)} cannot be null for {nameof(Verify)} extension method.");

			return Verify(actualParameters, expectedParameters.Encrypted, expectedParameters.Channel, expectedParameters.DeliveryMethod);
		}

		public static bool Verify(this IStaticPayloadParameters actualParameters, bool encrypt, byte channel, DeliveryMethod method)
		{
			if (actualParameters == null) throw new ArgumentNullException(nameof(actualParameters));

			//Checks if the parameters match the expectation.
			return actualParameters.Channel == channel && actualParameters.Encrypted == encrypt && actualParameters.DeliveryMethod == method;
		}
	}
}
