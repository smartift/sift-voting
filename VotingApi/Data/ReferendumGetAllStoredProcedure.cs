using System;
using Guytp.Data;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// The ReferendumGetSummaries stored procedure looks up all referendums but excludes voter and answer details.
    /// </summary>
    internal class ReferendumGetAllStoredProcedure : StoredProcedure
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public ReferendumGetAllStoredProcedure
            ()
            : base("ReferendumGetAll", new Type[] { typeof(ReferendumGetSummaryRow), typeof(ReferendumGetAnswerRow), typeof(ReferendumGetVoterRow) })
        {
        }
        #endregion
    }
}