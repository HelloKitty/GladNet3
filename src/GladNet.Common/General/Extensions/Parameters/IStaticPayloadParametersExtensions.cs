using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class IStaticPayloadParametersExt
	{
		public static bool Verify(this IStaticPayloadParameters actualParameters, IMessageParameters expectedParameters)
		{
			if (actualParameters == null)
				throw new ArgumentNullException("actualParameters", "actualParameters cannot be null for an extension method.");

			if (expectedParameters == null)
				throw new ArgumentNullException("expectedParameters", "expectedParameters cannot be null for an extension method.");

			return Verify(actualParameters, expectedParameters.Encrypted, expectedParameters.Channel, expectedParameters.DeliveryMethod);
		}

		public static bool Verify(this IStaticPayloadParameters parameters, bool encrypt, byte channel, DeliveryMethod method)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters", "Parameters cannot be null for an extension method.");

			//Checks if the parameters match the expectation.
			return parameters.Channel == channel && parameters.Encrypted == encrypt && parameters.DeliveryMethod == method;
		}
	}
}
