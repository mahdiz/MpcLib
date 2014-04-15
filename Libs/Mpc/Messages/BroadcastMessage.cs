﻿using MpcLib.DistributedSystem.ByzantineAgreement;

namespace MpcLib.DistributedSystem.Mpc
{
	/// <summary>
	/// Wraps a Byzantine agreement broadcast message.
	/// </summary>
	public class BroadcastMessage<TData> : MpcMsg
	{
		public BroadcastStage Stage { get { return baMsg.Stage; } }

		public TData Content { get { return baMsg.Content; } }

		private readonly ByzantineAgreement.BroadcastMessage<TData> baMsg;

		public BroadcastMessage(BroadcastStage stage, TData content, int k)
			: base(Mpc.Stage.SecureBroadcast)
		{
			baMsg = new ByzantineAgreement.BroadcastMessage<TData>(stage, content, k);
		}
	}
}