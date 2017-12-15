using System;
using System.Linq;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// A referendum contains a single question that can be voted on by the electorate.
    /// </summary>
    public class Referendum
    {
        #region Properties
        /// <summary>
        /// Gets the unique ID for this referendum
        /// </summary>
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

        /// <summary>
        /// Gets a list of possible answers for this voting question.
        /// </summary>
        public string[] Answers { get; private set; }

        /// <summary>
        /// Gets the number of votes cast for each of the answers.
        /// </summary>
        public int[] VoteCount { get; private set; }

        /// <summary>
        /// Gets the percentage of the voting electorate that have voted for each of the answers.  Each index matches the corresponding answer.
        /// </summary>
        public decimal[] VotePercentages { get; private set; }

        /// <summary>
        /// Gets a list of voters that are eligible to vote and how they voted.
        /// </summary>
        public Voter[] Electorate { get; private set; }
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
        /// <param name="answers">
        /// A list of possible answers for this voting question.
        /// </param>
        /// <param name="electorate">
        /// A list of voters that are eligible to vote and how they voted.
        /// </param>
        /// <param name="createTime">
        /// The time that this vote was created (in UTC).
        /// </param>
        public Referendum(int id, DateTime startTime, DateTime endTime, string question, string[] answers, Voter[] electorate, DateTime createTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            Question = question;
            Answers = answers;
            Electorate = electorate;
            CreateTime = createTime;
            if (answers.Length > 0)
            {
                VoteCount = new int[answers.Length];
                VotePercentages = new decimal[answers.Length];
                int totalVotes = electorate.Where(el => el.Vote > 0).Sum(el => el.VoteCount);
                if (totalVotes > 0)
                    for (int i = 0; i < answers.Length; i++)
                    {
                        VoteCount[i] = electorate.Where(el => el.Vote == i + 1).Sum(el => el.VoteCount);
                        VotePercentages[i] = Math.Round(((decimal)VoteCount[i] / (decimal)totalVotes) * 100m, 1);
                    }
            }
        }
        #endregion

        /// <summary>
        /// Strips electorate details from the referendum to create a more compact object when summarised are sufficient.
        /// </summary>
        internal void StripVoters()
        {
            Electorate = null;
        }
    }
}