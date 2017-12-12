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

        /// <summary>
        /// Gets all referendums from the database excluding answer and voter rows.
        /// </summary>
        /// <returns>
        /// An array of all referendums from the database.
        /// </returns>
        public Referendum[] GetSummaries()
        {
            using (StoredProcedureDataSetReader reader = ExecuteProcedureReader(new ReferendumGetSummariesStoredProcedure()))
            {
                ReferendumGetSummaryRow[] summaryRows = reader.GetDataSetList<ReferendumGetSummaryRow>();
                if (summaryRows == null)
                    return null;
                return summaryRows.Select(r => new Referendum(r.Id, r.StartTime, r.EndTime, r.Question, null, null, r.CreateTime)).ToArray();
            }
        }

        /// <summary>
        /// Updates a particular voter's record in a referendum.  If the voter does not exist the vote will not be registered.
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
        public void Vote(int referendumId, string address, int vote, string signature)
        {
            ExecuteProcedureNonQuery(new ReferendumVoteStoredProcedure(referendumId, address, vote, signature));
        }
    }
}