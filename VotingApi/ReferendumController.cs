using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Net.Http;
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
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The supplied authentication details are not valid or do not grant access to this method.")]
        public HttpResponseMessage ReferendumLookup(HttpRequestMessage requestMessage, string id)
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
    }
}