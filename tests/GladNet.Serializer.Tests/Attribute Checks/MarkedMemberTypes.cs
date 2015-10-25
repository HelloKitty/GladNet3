using GladNet.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Serializer.Tests
{
	[TestFixture]
	public static class MarkedMemberTypes
	{

		[Test]
		[TestCase(typeof(NetworkMessage), MemberTypes.Property, typeof(NetSendable<PacketPayload>))]
		public static void Check_Expected_Member_Marked_Attributes(Type typeToCheckOn, MemberTypes memberType, Type typeToLookFor)
		{
			//arrange
			MemberInfo potentialMethodInfoOfMarked = typeToCheckOn.GetMembers(BindingFlags.Public | BindingFlags.Instance)
				.Where(mi => mi.MemberType == memberType)
				.Where(mi => mi.GetUnderlyingType() == typeToLookFor)
				.FirstOrDefault();

			//Make sure we found the methodinfo. Otherwise we should fail.
			Assert.IsNotNull(potentialMethodInfoOfMarked, "Failed to find Member of MemberType: " + memberType 
				+ " on Type: " + typeToCheckOn + " of Type: " + typeToLookFor);

			Assert.IsNotNull(potentialMethodInfoOfMarked.GetCustomAttribute<GladNetMemberAttribute>());
		}

		//TODO: Reflection lib
		//http://stackoverflow.com/a/16043551/4184238
		public static Type GetUnderlyingType(this MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Event:
					return ((EventInfo)member).EventHandlerType;
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Method:
					return ((MethodInfo)member).ReturnType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				case MemberTypes.TypeInfo:
					return ((Type)member);
				default:
					throw new ArgumentException
					(
						"Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
					);
			}
		}
	}
}
