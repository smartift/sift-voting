namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// A voter is a single Ethereum address that is eligible to vote and contains their responses to the vote.
    /// </summary>
    public class Voter
    {
        #region Properties
        /// <summary>
        /// Gets the address of this voter.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets the count of votes that this voter has.
        /// </summary>
        public uint VoteCount { get; private set; }

        /// <summary>
        /// Gets or sets the vote that this voter has made.  A value of 0 indicates the voter has not voted, a value of 1 indicates the first answer from the Answers array within VoteInformation.
        /// </summary>
        public int Vote { get; set; }

        /// <summary>
        /// Gets or sets the signed vote message that this voter used to cast their vote.
        /// </summary>
        public string SignedVoteMessage { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="address">
        /// The address of this voter.
        /// </param>
        /// <param name="voteCount">
        /// The count of votes that this voter has.
        /// </param>
        /// <param name="vote">
        /// The vote that this voter has made.  A value of 0 indicates the voter has not voted, a value of 1 indicates the first answer from the Answers array within VoteInformation.
        /// </param>
        /// <param name="signedVoteMessage">
        /// The signed vote message that this voter used to cast their vote.
        /// </param>
        public Voter(string address, uint voteCount, int vote, string signedVoteMessage)
        {
            Address = address;
            VoteCount = voteCount;
            Vote = vote;
            SignedVoteMessage = signedVoteMessage;
        }
        #endregion
    }
}