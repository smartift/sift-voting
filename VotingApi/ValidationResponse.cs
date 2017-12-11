namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// A validation response is used to inform the calling client that the request failed for particular validation reasons.
    /// </summary>
    public class ValidationResponse
    {
        #region Properties
        /// <summary>
        /// Gets a list of any fields that caused a validation error.  This may be null if validation failures are unrelated to specific fields.
        /// </summary>
        public string[] InvalidFields { get; }

        /// <summary>
        /// Gets a list of array messages describing what validation errors occurred.
        /// </summary>
        public string[] ErrorMessages { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="invalidFields">
        /// A list of any fields that caused a validation error.  This may be null if validation failures are unrelated to specific fields.
        /// </param>
        /// <param name="errorMessages">
        /// A list of array messages describing what validation errors occurred.
        /// </param>
        public ValidationResponse(string[] invalidFields, string[] errorMessages)
        {
            InvalidFields = invalidFields;
            ErrorMessages = errorMessages;
        }
        #endregion
    }
}