using Guytp.Data;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// This class represents a row from the second dataset returned from a ReferendumGet stored procedure call representing an answer in Referendum.
    /// </summary>
    internal class ReferendumGetAnswerRow
    {
        #region Properties
        /// <summary>
        /// Gets the ID of the referendum that this answer belongs to.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets a possible answer to the voting question.
        /// </summary>
        public string Answer { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="answer">
        /// A possible answer to the voting question.
        /// </param>
        [StoredProcedureDataSetConstructor]
        public ReferendumGetAnswerRow(int id, string answer)
        {
            Id = id;
            Answer = answer;
        }
        #endregion
    }
}