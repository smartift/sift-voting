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
        public ReferendumGetAnswerRow(string answer)
        {
            Answer = answer;
        }
        #endregion
    }
}