using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Extensions for the <see cref="IMessageHandlerStrategy{TPeerType, TNetworkMessageType}}"/>.
	/// </summary>
	public static class ChainHandlerExtensions
	{
		/// <summary>
		/// Builds a handler service bridge around the provided object.
		/// </summary>
		/// <typeparam name="TPeerType">The peer type (inferred usually)</typeparam>
		/// <param name="strat">Message handler strategy instance.</param>
		/// <returns>A message handler service.</returns>
		public static IEventMessageHandlerService<TPeerType> ToService<TPeerType>(this IMessageHandlerStrategy<TPeerType, IEventMessage> strat)
			where TPeerType : INetPeer
		{
			return new EventMessageHandlerService<TPeerType>(strat);
		}

		/// <summary>
		/// Builds a handler service bridge around the provided object.
		/// </summary>
		/// <typeparam name="TPeerType">The peer type (inferred usually)</typeparam>
		/// <param name="strat">Message handler strategy instance.</param>
		/// <returns>A message handler service.</returns>
		public static IRequestMessageHandlerService<TPeerType> ToService<TPeerType>(this IMessageHandlerStrategy<TPeerType, IRequestMessage> strat)
			where TPeerType : INetPeer
		{
			return new RequestMessageHandlerService<TPeerType>(strat);
		}

		/// <summary>
		/// Builds a handler service bridge around the provided object.
		/// </summary>
		/// <typeparam name="TPeerType">The peer type (inferred usually)</typeparam>
		/// <param name="strat">Message handler strategy instance.</param>
		/// <returns>A message handler service.</returns>
		public static IResponseMessageHandlerService<TPeerType> ToService<TPeerType>(this IMessageHandlerStrategy<TPeerType, IResponseMessage> strat)
			where TPeerType : INetPeer
		{
			return new ResponseMessageHandlerService<TPeerType>(strat);
		}
	}
}
