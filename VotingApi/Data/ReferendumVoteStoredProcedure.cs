using System;

using Guytp.Data;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// The ReferendumVote stored procedure allows a registered voter to vote in a referendum.
    /// </summary>
    internal class ReferendumVoteStoredProcedure : StoredProcedure
    {
        #region Properties
        /// <summary>
        /// Gets the referendum to register the vote for.
        /// </summary>
        [StoredProcedureParameter]
        public int ReferendumId { get; private set; }

        /// <summary>
        /// Gets the address to register the vote for.
        /// </summary>
        [StoredProcedureParameter]
        public string Address { get; private set; }

        /// <summary>
        /// Gets the vote to cast.
        /// </summary>
        [StoredProcedureParameter]
        public int Vote { get; private set; }

        /// <summary>
        /// Gets the signature for the vote.
        /// </summary>
        [StoredProcedureParameter]
        public string Signature { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="referendumId">
        /// The referendum to register the vote for.
        /// </param>
        /// <param name="address">
        /// The address to register the vote for.
        /// </param>
        /// <param name="vote">
        /// The vote to cast.
        /// </param>
        /// <param name="signature">
        /// The signature for the vote.
        /// </param>
        public ReferendumVoteStoredProcedure(int referendumId, string address, int vote, string signature)
            : base("ReferendumVote")
        {
            ReferendumId = referendumId;
            Address = address;
            Vote = vote;
            Signature = signature;
        }
        #endregion
    }
}