using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using GladNet.Serializer;

namespace GladNet.Lidgren.Server
{
	public class InternalUnityApplicationBase : ApplicationBase
	{
		/// <inheritdoc />
		public InternalUnityApplicationBase(IDeserializerStrategy deserializer, ISerializerStrategy serializer, ILog logger, IManagedClientSessionFactory sessionManagedFactory) 
			: base(deserializer, serializer, logger, sessionManagedFactory)
		{

		}

		/// <inheritdoc />
		protected override void OnServerStop()
		{
			//Do nothing; Unity3D won't implement this in here.
		}

		/// <inheritdoc />
		public override void RegisterTypes(ISerializerRegistry registry)
		{
			//TODO: Unity3D specific types?
		}
	}
}
