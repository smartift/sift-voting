using Guytp.Data;
using System.Linq;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// A ReferendumRepository hooks up to the SIFT Referendum backend database.
    /// </summary>
    public class ReferendumRepository : SqlRepository
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public ReferendumRepository()
            : base("SiftReferendum")
        {
        }
        #endregion

        /// <summary>
        /// Gets a referendum from the database based on its unique ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the referendum to get.
        /// </param>
        /// <returns>
        /// The specified referendum if found, otherwise null.
        /// </returns>
        public Referendum Get(int id)
        {
            using (StoredProcedureDataSetReader reader = ExecuteProcedureReader(new ReferendumGetStoredProcedure(id)))
            {
                ReferendumGetSummaryRow summaryRow = reader.GetDataSetRow<ReferendumGetSummaryRow>();
                if (summaryRow == null)
                    return null;
                ReferendumGetAnswerRow[] answerRows = reader.GetDataSetList<ReferendumGetAnswerRow>();
                ReferendumGetVoterRow[] voterRows = reader.GetDataSetList<ReferendumGetVoterRow>();
                return new Referendum(id, summaryRow.StartTime, summaryRow.EndTime, summaryRow.Question, answerRows == null ? null : answerRows.Select(ar => ar.Answer).ToArray(), voterRows == null ? null : voterRows.Select(vr => new Voter(vr.Address, vr.VoteCount, vr.Vote, vr.SignedVoteMessage)).ToArray(), summaryRow.CreateTime);
            }
        }
    }
}