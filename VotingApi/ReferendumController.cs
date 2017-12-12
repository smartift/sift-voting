using Nethereum.Signer;
using Nethereum.Web3;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// This controller exposes methods that allow a user to vote on a particualr SIFT referendum and provides details about a referendum.
    /// </summary>
    public class ReferendumController : ApiController
    {
        /// <summary>
        /// Retrieves all information about a referendum including voters and their votes.
        /// </summary>
        /// <param name="requestMessage">
        /// The HTTP Request Message detailing everything about this call.
        /// </param>
        /// <param name="id">
        /// The ID of the referendum to get information about.
        /// </param>
        /// <returns>
        /// Full details about the specified referendum.
        /// </returns>
        [HttpGet]
        [Route("referendum/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Referendum), Description = "Full information about the specified referendum, including current voting information.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The specified vote could not be found")]
        public HttpResponseMessage ReferendumLookup(HttpRequestMessage requestMessage, int id)
        {
            // Lookup the referendum and 404 it if we cannot find it, otherwise return it as-is
            using (ReferendumRepository repo = new ReferendumRepository())
            {
                Referendum referendum = repo.Get(id);
                if (referendum == null)
                    return requestMessage.CreateResponse(HttpStatusCode.NotFound);
                return requestMessage.CreateResponse(HttpStatusCode.OK, referendum);
            }
        }

        /// <summary>
        /// Votes in a specific referendum.
        /// </summary>
        /// <param name="requestMessage">
        /// The HTTP Request Message detailing everything about this call.
        /// </param>
        /// <param name="id">
        /// The ID of the referendum to vote in.
        /// </param>
        /// <param name="request">
        /// Full details of the request to vote in.
        /// </param>
        [HttpPut]
        [Route("referendum/{id}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "The vote was successful.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ValidationResponse), Description = "The request failed due to one or more errors.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The specified vote could not be found")]
        public HttpResponseMessage ReferendumVote(HttpRequestMessage requestMessage, int id, VoteRequest request)
        {
            // Find the referendum that matches
            using (ReferendumRepository repo = new ReferendumRepository())
            {

                Referendum referendum = repo.Get(id);
                if (referendum == null)
                    return requestMessage.CreateResponse(HttpStatusCode.NotFound);

                // First perform the same checks that the client performs - required fields are provided, the Ethereum address is valid and in our voting list
                List<string> invalidFields = new List<string>();
                List<string> errorMessages = new List<string>();
                if (string.IsNullOrEmpty(request?.Address))
                {
                    invalidFields.Add("voteAddress");
                    errorMessages.Add("Your Ethereum address is required");
                }
                else if (request.Address.Length != 42)
                {
                    invalidFields.Add("voteAddress");
                    errorMessages.Add("Your Ethereum address is not valid");
                }
                else
                {
                    bool found = false;
                    foreach (Voter voter in referendum.Electorate)
                    {
                        if (voter.Address.ToLower() != request.Address.ToLower())
                            continue;
                        if (voter.VoteCount == 0)
                            continue;
                        found = true;
                        break;
                    }
                    if (!found)
                    {
                        invalidFields.Add("voteAddress");
                        errorMessages.Add("Your Ethereum address is not entitled to vote");
                    }
                }
                if (request.Vote < 1)
                {
                    invalidFields.Add("voteAnswer");
                    errorMessages.Add("Your need to select your voting intention");
                }
                else if (request.Vote > referendum.Answers.Length)
                {
                    invalidFields.Add("voteAnswer");
                    errorMessages.Add("Your vote is not valid for this referendum");
                }
                if (string.IsNullOrEmpty(request.SignedVoteMessage))
                {
                    invalidFields.Add("signature");
                    errorMessages.Add("Your need to sign your vote");
                }
                else if (request.SignedVoteMessage.Length != 132)
                {
                    invalidFields.Add("signature");
                    errorMessages.Add("Your signature's hex string is not the correct length");
                }

                // Now confirm that the signature is that which we expect, if not generate an error
                if (!invalidFields.Contains("signature"))
                {
                    try
                    {
                        MessageSigner signer = new MessageSigner();
                        string message = "R" + id + " V" + request.Vote + " " + request.Address.ToLower();
                        string v1Address = signer.EcRecover(new Nethereum.Util.Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(message)), request.SignedVoteMessage);
                        string v2Address = signer.EcRecover(new Nethereum.Util.Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes("\u0019Ethereum Signed Message:\n" + message.Length + message)), request.SignedVoteMessage);
                        if (v1Address.ToLower() != request.Address.ToLower() && v2Address.ToLower() != request.Address.ToLower())
                        {
                            invalidFields.Add("signature");
                            errorMessages.Add("Your signature is not correct");
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidFields.Add("signature");
                        errorMessages.Add("Unexpected error decoding your signature: " + ex.Message);
                    }
                }

                // Check timings for the vote
                if (DateTime.UtcNow < referendum.StartTime)
                {
                    invalidFields.Add(string.Empty);
                    errorMessages.Add("The vote has not yet started");
                }
                if (DateTime.UtcNow > referendum.EndTime)
                {
                    invalidFields.Add(string.Empty);
                    errorMessages.Add("The vote has already ended");
                }

                // If any errors are present, return them now.
                if (errorMessages.Count > 0)
                    return requestMessage.CreateResponse(HttpStatusCode.BadRequest, new ValidationResponse(invalidFields.ToArray(), errorMessages.ToArray()));

                // Add or update the vote in the backend repo
                repo.Vote(id, request.Address, request.Vote, request.SignedVoteMessage);

                // Confirm this to the user
                return requestMessage.CreateResponse(HttpStatusCode.NoContent);
            }
        }


        /// <summary>
        /// Retrieves summary information about all referendums - excluding the voter and answer details.
        /// </summary>
        /// <param name="requestMessage">
        /// The HTTP Request Message detailing everything about this call.
        /// </param>
        /// <returns>
        /// Summary details about all referendum.
        /// </returns>
        [HttpGet]
        [Route("referendum")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Referendum), Description = "Summary details about all referendum, excluding voter and answer details.")]
        public HttpResponseMessage ReferendumSummary(HttpRequestMessage requestMessage)
        {
            using (ReferendumRepository repo = new ReferendumRepository())
            {
                Referendum[] referendums = repo.GetSummaries();
                return requestMessage.CreateResponse(HttpStatusCode.OK, referendums);
            }
        }
    }
}