using Guytp.Data;

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
        public Referendum Get(string id)
        {
            return null;
        }
    }
}