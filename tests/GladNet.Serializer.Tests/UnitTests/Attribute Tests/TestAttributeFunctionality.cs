using GladNet.Serializer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
{
	[TestFixture]
	public static class TestAttributeFunctionality
	{
		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(int.MinValue)]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public static void Test_MemberAttribute_Ctor_With_Invalid_Tags(int tagID)
		{
			//arrange
			GladNetMemberAttribute gma = new GladNetMemberAttribute(tagID);

			//Should throw due to invalid tagID
			//See ctor of https://github.com/mgravell/protobuf-net/blob/e601b359c6ae56afc159754d29f5e7d0f05a01f5/protobuf-net/ProtoMemberAttribute.cs
			//attribute is based on protobuf-net specs.

			Assert.Fail("Expected exception indicating non-positive values were not excepted.");
		}

		[Test]
		[TestCase(1)]
		[TestCase(int.MaxValue)]
		public static void Test_MemberAttribute_Ctor_With_Valid_Tags(int tagID)
		{
			//arrange
			GladNetMemberAttribute gma = new GladNetMemberAttribute(tagID);

			//assert
			Assert.AreEqual(gma.TagID, tagID);
		}

		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(int.MinValue)]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public static void Test_IncludeAttribute_Ctor_With_Invalid_Tags(int tagID)
		{
			//arrange
			GladNetSerializationIncludeAttribute gma = new GladNetSerializationIncludeAttribute(tagID, typeof(GladNetSerializationIncludeAttribute));

			//Should throw due to invalid tagID
			//See ctor of https://github.com/mgravell/protobuf-net/blob/e601b359c6ae56afc159754d29f5e7d0f05a01f5/protobuf-net/ProtoMemberAttribute.cs
			//attribute is based on protobuf-net specs.

			Assert.Fail("Expected exception indicating non-positive values were not excepted.");
		}

		[Test] //just picking a random Type to use for testing.
		[TestCase(1, typeof(GladNetSerializationIncludeAttribute))]
		[TestCase(int.MaxValue, typeof(GladNetSerializationIncludeAttribute))]
		public static void Test_IncludeAttribute_Ctor_With_Valid_Tags(int tagID, Type derivedType)
		{
			//arrange
			GladNetSerializationIncludeAttribute gma = new GladNetSerializationIncludeAttribute(tagID, derivedType);

			//assert
			Assert.AreEqual(gma.TagID, tagID);
			Assert.AreEqual(gma.DerivedTypeToInclude, derivedType);
		}


		[Test]
		[TestCase(int.MaxValue, null)]
		[ExpectedException(typeof(ArgumentNullException))]
		public static void Test_IncludeAttribute_Ctor_With_Null_Type(int tagID, Type derivedType)
		{
			//arrange
			GladNetSerializationIncludeAttribute gma = new GladNetSerializationIncludeAttribute(tagID, derivedType);

			//assert
			Assert.Fail("Expected to fail from null Type. Should not accept a null Type and should instead throw.");
		}
	}
}
