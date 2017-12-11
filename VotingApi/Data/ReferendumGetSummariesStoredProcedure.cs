using System;
using Guytp.Data;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// The ReferendumGetSummaries stored procedure looks up all referendums but excludes voter and answer details.
    /// </summary>
    internal class ReferendumGetSummariesStoredProcedure : StoredProcedure
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="id">
        /// The ID of the referendum to lookup.
        /// </param>
        public ReferendumGetSummariesStoredProcedure()
            : base("ReferendumGetSummaries", new Type[] { typeof(ReferendumGetSummaryRow) })
        {
        }
        #endregion
    }
}