using System;

using Guytp.Data;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// The ReferendumGet stored procedure looks up a single referendum and all its associated data by its primary key.
    /// </summary>
    internal class ReferendumGetStoredProcedure : StoredProcedure
    {
        #region Properties
        /// <summary>
        /// Gets the ID of the referendum to lookup.
        /// </summary>
        [StoredProcedureParameter]
        public int Id { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="id">
        /// The ID of the referendum to lookup.
        /// </param>
        public ReferendumGetStoredProcedure(int id)
            : base("ReferendumGet", new Type[] { typeof(ReferendumGetSummaryRow), typeof(ReferendumGetAnswerRow), typeof(ReferendumGetVoterRow) })
        {
            Id = id;
        }
        #endregion
    }
}