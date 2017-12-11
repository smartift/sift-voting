using Guytp.Data;
using System;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// This class represents a row from the first dataset returned from a ReferendumGet stored procedure call representing a Referendum.
    /// </summary>
    internal class ReferendumGetSummaryRow
    {
        #region Properties
        /// <summary>
        /// Gets the unique ID for this referendum
        public int Id { get; private set; }

        /// <summary>
        /// Gets the time that voting may commence for this vote (in UTC).
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the time that voting must be received by for this vote (in UTC).
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Gets the time that this vote was created (in UTC).
        /// </summary>
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// Gets the text of this voting question.
        /// </summary>
        public string Question { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="id">
        /// The unique ID for this referendum
        /// </param>
        /// <param name="startTime">
        /// The time that voting may commence for this vote (in UTC).
        /// </param>
        /// <param name="endTime">
        /// The time that voting must be received by for this vote (in UTC).
        /// </param>
        /// <param name="question">
        /// The text of this voting question.
        /// </param>
        /// <param name="createTime">
        /// The time that this vote was created (in UTC).
        /// </param>
        [StoredProcedureDataSetConstructor]
        public ReferendumGetSummaryRow(int id, string question, DateTime startTime, DateTime endTime, DateTime createTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            Question = question;
            CreateTime = createTime;
        }
        #endregion
    }
}