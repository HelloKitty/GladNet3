using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
{
	[TestFixture]
	public static class IMessageParametersTests
	{
		[Test]
		public static void Test_Verify_Extension_With_Null(
			[Values(0)] byte channel,
			[Values(true, false)] bool encrypt,
			[EnumRange(typeof(DeliveryMethod))] DeliveryMethod method)
		{
			//Should throw if it's null
			Assert.That(() => ((IStaticPayloadParameters)null).VerifyExt(Mock.Of<IMessageParameters>()) , Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => ((IStaticPayloadParameters)Mock.Of<IStaticPayloadParameters>()).VerifyExt(null), Throws.TypeOf<ArgumentNullException>());

			Assert.That(() => ((IStaticPayloadParameters)null).VerifyExt(encrypt, channel, method), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void Test_Verify_Extensions_With_NonNull(
			[Values(0)] byte channel, 
			[Values(true, false)] bool encrypt, 
			[EnumRange(typeof(DeliveryMethod))] DeliveryMethod method)
		{
			//arrange
			Mock<IStaticPayloadParameters> parameters = new Mock<IStaticPayloadParameters>(MockBehavior.Strict);
			Mock<IMessageParameters> expectedParameters = new Mock<IMessageParameters>(MockBehavior.Strict);
			IEnumerable<Mock<IMessageParameters>> parameterCollection = new List<Mock<IMessageParameters>> { parameters.As<IMessageParameters>(), expectedParameters };

			foreach(var mockP in parameterCollection)
			{
				mockP.Setup(x => x.Channel).Returns(channel);
				mockP.Setup(x => x.Encrypted).Returns(encrypt);
				mockP.Setup(x => x.DeliveryMethod).Returns(method);
			}

			//assert
			Assert.True(parameters.Object.VerifyExt(expectedParameters.Object));
			Assert.True(parameters.Object.VerifyExt(expectedParameters.Object.Encrypted, expectedParameters.Object.Channel, expectedParameters.Object.DeliveryMethod));

			//Test that it doesn't return true if they differ
			Assert.False(parameters.Object.VerifyExt(!(expectedParameters.Object.Encrypted),(byte)(expectedParameters.Object.Channel + 1), expectedParameters.Object.DeliveryMethod + 1));
		}
	}
}
