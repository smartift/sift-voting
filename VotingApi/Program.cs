using Guytp.WebApi;

namespace Lts.Sift.Voting.Api
{
    /// <summary>
    /// This class is the main entry point of the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// This method is the main entry point of the application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            new WebApiService().Bootstrap();
        }
    }
}