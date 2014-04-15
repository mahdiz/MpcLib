﻿namespace MpcLib.DistributedSystem.ByzantineAgreement
{
	public class BroadcastMessage : Msg
	{
		public readonly int k;
		public readonly BroadcastStage Stage;

		public BroadcastMessage(BroadcastStage stage, int k)
		{
			Stage = stage;
			this.k = k;
		}

		public override int StageKey
		{
			get { return (int)Stage; }
		}
	}

	public class BroadcastMessage<T> : BroadcastMessage
	{
		public readonly T Content;

		public BroadcastMessage(BroadcastStage stage, T content, int k)
			: base(stage, k)
		{
			Content = content;
		}

		public override bool Equals(object obj)
		{
			return Content.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Content.GetHashCode();
		}
	}
}