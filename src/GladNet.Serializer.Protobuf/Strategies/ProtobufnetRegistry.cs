using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fasterflect;
using System.Reflection;

namespace GladNet.Serializer.Protobuf
{
	public class ProtobufnetRegistry : ISerializerRegistry
	{
		public bool Register(Type typeToRegister)
		{
			//Ok so here the fun begins.
			//We need a recursive algorithm for walking the graph of the Type to register each
			//serializable type. So, for every member we must look down into that Type recursively and register it.
			//However, we must make sure to check if it's already been registered or we might get circular graphs which
			//would overflow. But that'd be a weird type anyway.

			typeToRegister.ThrowIfNull(nameof(typeToRegister));

			if (RuntimeTypeModel.Default.IsDefined(typeToRegister))
				return true;

			//If it's not defined we need to add it.
			if (typeToRegister.Attribute<GladNetSerializationContractAttribute>() == null)
				return false;

			MetaType typeModel = RuntimeTypeModel.Default.Add(typeToRegister, false);

			//Add each member
			foreach (MemberInfo mi in typeToRegister.MembersWith<GladNetMemberAttribute>(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility))
			{
				typeModel.Add(mi.Attribute<GladNetMemberAttribute>().TagID, mi.Name);

				//Now we might need to register this type aswell.
				//Recur to try to register this type
				Register(mi.Type());
			}

			//If might have a include on it so we should check it to register it with the subtype

			GladNetSerializationIncludeAttribute include = typeToRegister.Attribute<GladNetSerializationIncludeAttribute>();

			if (include != null)
				typeModel.AddSubType(include.TagID, include.DerivedTypeToInclude);

			return true;
		}
	}
}
