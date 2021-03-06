﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MpcLib.Common.FiniteField;
using MpcLib.DistributedSystem.QuorumSystem;

namespace MpcLib.MpcProtocols.Dkms
{
	public class DkmsParty : Processor<DkmsProtocol>
	{
		public Zp Input
		{
			get
			{
				if (Protocol == null)
					return null;
				return Protocol.Input;
			}
		}

		public Zp Result
		{
			get
			{
				if (Protocol == null)
					return null;
				return Protocol.Result;
			}
		}

		public DkmsParty()
		{
		}

		public virtual void Init(Circuit circuit, Zp input, int numSlots,
			IList<int> players, int numQuorums, int quorumSize, QuorumBuildingMethod qbMethod)
		{
			base.Init(players, numQuorums, quorumSize, qbMethod);
			Protocol = new DkmsProtocol(circuit, this, players, input, numSlots, null);
		}

		public override void Start()
		{
			base.Start();		// starts quorum building. we will continue OnQbFinish.
		}

		protected override void OnQbFinish(Dictionary<int, int[]> quorumsMap)
		{
			// quorums are built. run the scalable SMPC protocol.
			Debug.Assert(Protocol != null);
			Protocol.QuorumsMap = quorumsMap;
			Protocol.Run();
		}

		//public override void Receive(Msg msg)
		//{
		//	base.Receive(msg);

		//	Debug.Assert(msg is DkmsMsg);
		//	//Protocol.Receive(msg as DkmsMsg);
		//}
	}
}