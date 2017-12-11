namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// A vote request encapsulates a request for a user to vote in a specific referendum.  The user provides their voting intention and a signed message confirming this and votes accordingly.
    /// </summary>
    public class VoteRequest
    {
        #region Properties
        /// <summary>
        /// Gets the address that is voting.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets the signed message confirming the vote.
        /// </summary>
        public string SignedVoteMessage { get; private set; }

        /// <summary>
        /// Gets the vote that is being made where 1 is the first option in the list.  A value of 0 (no vote) is not valid.
        /// </summary>
        public int Vote { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="address">
        /// The address that is voting.
        /// </param>
        /// <param name="signedVoteMessage">
        /// The signed message confirming the vote.
        /// </param>
        /// <param name="vote">
        /// The vote that is being made where 1 is the first option in the list.  A value of 0 (no vote) is not valid.
        /// </param>
        public VoteRequest(string address, string signedVoteMessage, int vote)
        {
            Address = address;
            SignedVoteMessage = signedVoteMessage;
            Vote = vote;
        }
        #endregion
    }
}