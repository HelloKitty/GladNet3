using Easyception;
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
			Throw<ArgumentNullException>.If.IsNull(actualParameters)
				?.Now(nameof(actualParameters), $"{nameof(actualParameters)} cannot be null for {nameof(Verify)} extension method.");

			Throw<ArgumentNullException>.If.IsNull(expectedParameters)
				?.Now(nameof(expectedParameters), $"{nameof(expectedParameters)} cannot be null for {nameof(Verify)} extension method.");

			return Verify(actualParameters, expectedParameters.Encrypted, expectedParameters.Channel, expectedParameters.DeliveryMethod);
		}

		public static bool Verify(this IStaticPayloadParameters actualParameters, bool encrypt, byte channel, DeliveryMethod method)
		{
			Throw<ArgumentNullException>.If.IsNull(actualParameters)
				?.Now(nameof(actualParameters), $"{nameof(actualParameters)} cannot be null for {nameof(Verify)} extension method.");

			//Checks if the parameters match the expectation.
			return actualParameters.Channel == channel && actualParameters.Encrypted == encrypt && actualParameters.DeliveryMethod == method;
		}
	}
}
