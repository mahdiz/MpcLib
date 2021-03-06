using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using MpcLib.Commitments.PolyCommitment;
using MpcLib.Common;
using MpcLib.Common.FiniteField;
using MpcLib.DistributedSystem;
using MpcLib.SecretSharing.eVSS;

namespace MpcLib.SecretSharing.QuorumShareRenewal //what?
{

    public class QuorumShareRenewalProtocol : MultiQuorumProtocol<Share<BigZp>>
    {
        private const int FROM = 0;
        private const int TO = 1;

        private ShareRenewalRound CurrentRound;
        private int IntermediateRoundsRemaining;
        private BigZp[] Shares;

        private int FinalRoundInitialSharesPerParty;
        private BigInteger Prime;
        private bool FinalRound;

        public QuorumShareRenewalProtocol(Party me, Quorum quorumFrom, Quorum quorumTo, Share<BigZp> share, BigInteger prime)
            : this(me, quorumFrom, quorumTo, share, prime, quorumFrom.GetNextTwoProtocolId(quorumTo))
        {  
        }

        public QuorumShareRenewalProtocol(Party me, Quorum quorumFrom, Quorum quorumTo, Share<BigZp> share, BigInteger prime, ulong protocolId)
            : base(me, new Quorum[] { quorumFrom, quorumTo }, protocolId)
        {
            Debug.Assert(share == null || !share.IsPublic);  // makes no sense to do share renewal on a public value
            Prime = prime;
            if (share != null)
                Shares = new BigZp[] { share.Value };
        }

        public override void HandleMessage(int fromId, Msg msg)
        {
            if (CurrentRound == null && msg.Type == MsgType.NextRound)
            {
                // want to start the next round
                if (IntermediateRoundsRemaining > 0)
                {
                    CurrentRound = new ShareRenewalRound(Me, Quorums[FROM], Quorums[TO], Shares, Prime, Shares.Length, 3 * Shares.Length, ProtocolId);
                }
                else
                {
                    CurrentRound = new ShareRenewalRound(Me, Quorums[FROM], Quorums[TO], Shares, Prime, Shares.Length, 1, ProtocolId);
                    FinalRound = true;
                }
                CurrentRound.Start();
            }
            else
            {
                CurrentRound.HandleMessage(fromId, msg);

                if (CurrentRound.IsCompleted)
                {
                    if (FinalRound)
                    {
                        if (Quorums[TO].HasMember(Me.Id))
                        {
                            Debug.Assert(CurrentRound.Result.Length == 1);
                            Result = new Share<BigZp>(CurrentRound.Result[0], false);
                        }
                            
                        IsCompleted = true;
                    }
                    else
                    {
                        Shares = CurrentRound.Result;
                        IntermediateRoundsRemaining--;
                        CurrentRound = null;
                        Send(Me.Id, new Msg(MsgType.NextRound));
                        // use this to ensure all of the parties stay on the same round
                    }
                }
            }
        }

        public override void Start()
        {
            // determine how many rounds we will need so that the total number of shares is within a factor of 3 of the number in the receiving quorum
            IntermediateRoundsRemaining = 0;
            int shareCount = Quorums[FROM].Size;
            while (3*shareCount < Quorums[TO].Size)
            {
                IntermediateRoundsRemaining++;
                shareCount *= 3;
            }

            FinalRoundInitialSharesPerParty = shareCount / Quorums[FROM].Size;

            if (Quorums[FROM].HasMember(Me.Id))
            {
                if (IntermediateRoundsRemaining > 0)
                {
                    // set up the first intermediate round
                    CurrentRound = new ShareRenewalRound(Me, Quorums[FROM], Quorums[FROM], Shares, Prime, 1, 3, ProtocolId);
                    FinalRound = false;
                }
                else
                {
                    // set up the final round
                    CurrentRound = new ShareRenewalRound(Me, Quorums[FROM], Quorums[TO], Shares, Prime, 1, 1, ProtocolId);
                    FinalRound = true;
                }
            }
            else
            {
                // I'm only receiving, so I want to set up the final round
                CurrentRound = new ShareRenewalRound(Me, Quorums[FROM], Quorums[TO], null, Prime, FinalRoundInitialSharesPerParty, 1, ProtocolId);
                FinalRound = true;
            }
            
            CurrentRound.Start();
            if (CurrentRound.IsCompleted && FinalRound)
            {
                IsCompleted = true;
            }
                
        }
    }

    public class ShareRenewalRound : MultiQuorumProtocol<BigZp[]>
    {
        private const int FROM = 0;
        private const int TO = 1;

        private PolyCommit PolyCommit;
        private BigZp[] StartShares;
        private int StartSharesPerParty;
        private int FinalSharesPerParty;
        private int NewPolyDeg;
        private int FinalShareCount;
        private int StartShareCount;
        private BigInteger Prime;
        private IList<BigZp> VandermondeInv;
        private int ReceivedReshareCount;
        private int MyQuorumIndex;

        private Dictionary<int, int> numCommitsRecv = new Dictionary<int, int>();
        private Dictionary<int, CommitMsg> commitsRecv = new Dictionary<int, CommitMsg>();
        private Dictionary<int, List<ShareWitnessMsg<BigZp>>[]> sharesRecv = new Dictionary<int, List<ShareWitnessMsg<BigZp>>[]>();

        public ShareRenewalRound(Party me, Quorum quorumFrom, Quorum quorumTo, BigZp[] startShares, BigInteger prime, int startSharesPerParty, int finalSharesPerParty, ulong protocolId)
            : base(me, new Quorum[] { quorumFrom, quorumTo }, protocolId)
        {
            FinalSharesPerParty = finalSharesPerParty;
            StartSharesPerParty = startSharesPerParty;
            StartShares = startShares;
            Prime = prime;
            ReceivedReshareCount = 0;

            foreach (var from in Quorums[0].Members)
            {
                sharesRecv[from] = new List<ShareWitnessMsg<BigZp>>[startSharesPerParty];
                for (int i = 0; i < startSharesPerParty; i++)
                {
                    numCommitsRecv[from] = 0;
                    sharesRecv[from][i] = new List<ShareWitnessMsg<BigZp>>();
                }
            }

            StartShareCount = Quorums[FROM].Size * StartSharesPerParty;
            FinalShareCount = Quorums[TO].Size * FinalSharesPerParty;
            NewPolyDeg = (int)Math.Ceiling(FinalShareCount / 3.0) - 1;

            VandermondeInv = StaticCache.GetVandermondeInvColumn(Prime, StartShareCount);

            if (Quorums[TO].HasMember(Me.Id))
            {
                var quorumIter = Quorums[TO].Members.GetEnumerator();
                int i = 0;
                while (quorumIter.MoveNext())
                {
                    if (Me.Id == quorumIter.Current)
                    {
                        MyQuorumIndex = i;
                        break;
                    }
                    i++;
                }
            }

            if (Quorums[TO] is ByzantineQuorum)
            {
                var byzTo = Quorums[TO] as ByzantineQuorum;
                if (FinalShareCount == Quorums[TO].Size)
                    PolyCommit = byzTo.PolyCommit;
                else
                {
                    PolyCommit = new PolyCommit();
                    PolyCommit.Setup(NewPolyDeg + 1, byzTo.Seed);
                }
            }
        }
        
        public override void HandleMessage(int fromId, Msg msg)
        {
            switch (msg.Type)
            {
                case MsgType.Commit:
                    commitsRecv[fromId] = msg as CommitMsg;
                    numCommitsRecv[fromId]++;
                    Debug.Assert(numCommitsRecv[fromId] <= StartSharesPerParty);
                    break;

                case MsgType.Share:
                    if (commitsRecv.ContainsKey(fromId))
                    {
                        int whichOrigShare = numCommitsRecv[fromId] - 1;
                        int whichFinalFromOrigShare = sharesRecv[fromId][whichOrigShare].Count;
                        int evalPoint = MyQuorumIndex * FinalSharesPerParty + whichFinalFromOrigShare + 1;
                        var swMessage = msg as ShareWitnessMsg<BigZp>;
                        sharesRecv[fromId][numCommitsRecv[fromId] - 1].Add(swMessage);
                        ReceivedReshareCount++;
                        if (PolyCommit != null && !PolyCommit.VerifyEval(commitsRecv[fromId].Commitment, new BigZp(Prime, evalPoint), swMessage.Share, swMessage.Witness))
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else Console.WriteLine("No commitment from " + fromId);

                    // send the message to start the next round when we have received all of the shares from everybody
                    if (ReceivedReshareCount == StartShareCount * FinalSharesPerParty)
                    {
                        Send(Me.Id, new Msg(MsgType.NextRound));
                    }
                    break;
                case MsgType.NextRound:
                    Recombine();
                    break;
            }
        }

        public override void Start()
        {
            if (StartShares == null)
                return;

            foreach (var startShare in StartShares)
            {
                Reshare(startShare);
            }
            
            if (!Quorums[TO].HasMember(Me.Id))
            {
                IsCompleted = true;
              //  Send(Me.Id, new Msg(MsgType.NextRound));
            }
        }

        private void Reshare(BigZp secret)
        {
            Debug.Assert(Prime == secret.Prime);

            IList<BigZp> coeffs = null;

            var shares = BigShamirSharing.Share(secret, FinalShareCount, NewPolyDeg, out coeffs);

            MG[] witnesses = null;
            MG commitment = null;
            if (PolyCommit != null)
            {
                commitment = BigShamirSharing.GenerateCommitment(FinalShareCount, coeffs.ToArray(), Prime, ref witnesses, (Quorums[TO] as ByzantineQuorum).PolyCommit);
                
                // fake random generation round
                for (int i = 0; i < Quorums[TO].Size; i++)
                {
                    NetSimulator.FakeMulticast(Quorums[TO].Size, secret.Size);
                }
                // fake commitment and challenge response
                NetSimulator.FakeMulticast(Quorums[TO].Size, secret.Size);
                NetSimulator.FakeMulticast(Quorums[TO].Size, secret.Size);
            }
            else
                witnesses = new MG[FinalShareCount];
            
            QuorumBroadcast(new CommitMsg(commitment), TO);
            
            // create the share messages
            if (PolyCommit != null)
                Debug.Assert(PolyCommit.VerifyEval(commitment, new BigZp(Prime, 2), shares[1], witnesses[1]));
            Debug.Assert(BigShamirSharing.Recombine(shares, NewPolyDeg, Prime) == secret);

            int numSent = 0;
            //int delay = (PolyCommit != null) ? 1 : 0;
            foreach (var toMember in Quorums[TO].Members)
            {
                for (int i = 0; i < FinalSharesPerParty; i++)
                {
                    Send(toMember, new ShareWitnessMsg<BigZp>(shares[numSent], witnesses[numSent]));
                    numSent++;
                }
            }
        }

        private void Recombine()
        {
            BigZp[,] orderedShares = new BigZp[FinalSharesPerParty,StartShareCount];

            int senderPos = 0;
            foreach (var sender in Quorums[FROM].Members)
            {
                var sharesFromSender = sharesRecv[sender];
                for (int i = 0; i < StartSharesPerParty; i++)
                {
                    var resharesForShare = sharesFromSender[i];
                    for (int j = 0; j < resharesForShare.Count; j++)
                    {
                        var msg = resharesForShare[j];
                        orderedShares[j, senderPos * StartSharesPerParty + i] = msg.Share;
                    }
                }
                senderPos++;
            }

            Result = new BigZp[FinalSharesPerParty];

            for (int i = 0; i < FinalSharesPerParty; i++)
            {
                Result[i] = new BigZp(Prime);
                for (int j = 0; j < StartShareCount; j++)
                {
                    Result[i] += orderedShares[i,j] * VandermondeInv[j];
                }
            }

            IsCompleted = true;
        }
    }

    public class QuorumTaggedShareRenewalProtocol : MultiQuorumProtocol<Tuple<Share<BigZp>, Share<BigZp>>>
    {
        Tuple<Share<BigZp>, Share<BigZp>> Shares;
        BigInteger Prime;

        public QuorumTaggedShareRenewalProtocol(Party me, Quorum quorumFrom, Quorum quorumTo, Tuple<Share<BigZp>, Share<BigZp>> shares, BigInteger prime, ulong protocolId)
            : base(me, new Quorum[] { quorumFrom, quorumTo }, protocolId)
        {
            if (shares != null)
                Debug.Assert(shares.Item1.Value.Prime == shares.Item2.Value.Prime);
            Shares = shares;
            Prime = prime;
        }

        public override void HandleMessage(int fromId, Msg msg)
        {
            Debug.Assert(msg is SubProtocolCompletedMsg);

            var reshared = (msg as SubProtocolCompletedMsg).ResultList.Cast<Share<BigZp>>().ToArray();

            Result = new Tuple<Share<BigZp>, Share<BigZp>>(reshared[0], reshared[1]);
            IsCompleted = true;
        }

        public override void Start()
        {
            ExecuteSubProtocols(new Protocol[]
            {
                new QuorumShareRenewalProtocol(Me, Quorums[0], Quorums[1], (Shares == null) ? null : Shares.Item1, Prime, ProtocolId + 1),
                new QuorumShareRenewalProtocol(Me, Quorums[0], Quorums[1], (Shares == null) ? null : Shares.Item2, Prime, ProtocolId + 2)
            });
        }
    }
}
