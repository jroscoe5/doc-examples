using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace BlockOffice.BlockOfficeAPI
{
    /// <summary>
    /// Object returned by any BlockOfficeAPI request. Contains a flag indicating 
    /// if request was successfull and a JObject containing the body of the response.
    /// </summary>
    public class BlockOfficeAPIResponse
    {
        /// <summary>
        /// Flag indicating if request was successfull.
        /// </summary>
        public readonly bool Success;
        /// <summary>
        /// JObject containing the response from the request.
        /// </summary>
        public readonly JObject Content;
        internal BlockOfficeAPIResponse(bool success, JObject content)
        {
            Success = success;
            Content = content;
        }
    }

    /// <summary>
    /// BlockOfficeAPI functions.
    /// </summary>
    public class BlockOfficeAPI
    {
        private readonly Uri BASE_URI;
        private readonly HttpClient Client;
        private readonly Mutex HeaderLock;

        /// <summary>
        /// Constructor. Use baseUriString to point to either prod or test api.
        /// </summary>
        /// <param name="baseUriString">API Domain</param>
        public BlockOfficeAPI(string baseUriString = "http://api-demo-test.us-west-2.elasticbeanstalk.com")
        {
            Client = new HttpClient();
            HeaderLock = new Mutex();
            BASE_URI = new Uri(baseUriString);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password. Requires: 8 characters, both cases, at least one number</param>
        /// <param name="email">User's email</param>
        /// <param name="publicWallet">Public key of user's wallet</param>
        /// <param name="firstName">User's legal first name</param>
        /// <param name="lastName">User's legal last name</param>
        /// <param name="streetAddress">User's street address</param>
        /// <param name="city">User's city</param>
        /// <returns>Status code</returns>
        public async Task<BlockOfficeAPIResponse> RegisterUser(
            string username,
            string password,
            string email, 
            string publicWallet, 
            string firstName, 
            string lastName, 
            string streetAddress, 
            string city)
        {
            var uri = new Uri(BASE_URI, "/api/Users");
            var json = $"{{\"username\":\"{username}\"," +
                $"\"password\":\"{password}\"," +
                $"\"email\":\"{email}\"," +
                $"\"publicWallet\":\"{publicWallet}\"," +
                $"\"firstName\":\"{firstName}\"," +
                $"\"lastName\":\"{lastName}\"," +
                $"\"streetAddress\":\"{streetAddress}\"," +
                $"\"city\":\"{city}\"}}";

            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(uri, body);
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }
        
        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>JObject containing auth token used to call other API endpoints</returns>
        public async Task<BlockOfficeAPIResponse> LoginUser(string email, string password)
        {
            var uri = new Uri(BASE_URI, "/api/Users/login");
            var json = $"{{\"email\":\"{email}\", \"password\":\"{password}\"}}";
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(uri, body);
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Logs out a logged in user.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <returns>Status code</returns>
        public async Task<BlockOfficeAPIResponse> LogoutUser(string accessId)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, "/api/Users/logout");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.PostAsync(uri, null).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Retrieves all tickets for a user.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <param name="userId">User's id</param>
        /// <returns>JObject containing list of all tickets user currently has</returns>
        public async Task<BlockOfficeAPIResponse> GetTicketsForUser(string accessId, string userId)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, $"/api/Users/{userId}/tickets");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.GetAsync(uri).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Retrieves a specific ticket.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <param name="ticketId">Target ticket's id</param>
        /// <returns>JObject containing a single ticket</returns>
        public async Task<BlockOfficeAPIResponse> GetTicket(string accessId, string ticketId)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, $"/api/Tickets/{ticketId}");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.GetAsync(uri).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Retrieves a user's profile.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <param name="userId">Target user's id</param>
        /// <returns></returns>
        public async Task<BlockOfficeAPIResponse> GetUserProfile(string accessId, string userId)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, $"/api/Users/?filter[where][id]={userId}");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.GetAsync(uri).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Claims an unclaimed ticket.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <param name="claimCode">Generated claim code for ticket</param>
        /// <returns>Status Code</returns>
        public async Task<BlockOfficeAPIResponse> ClaimTicket(string accessId, string claimCode)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, $"/api/Tickets/claim/{claimCode}");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.GetAsync(uri).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Adds an ethereum wallet to a user's account.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <param name="userId">Target user's id</param>
        /// <param name="walletAddress">Address to add to target User's account</param>
        /// <returns>Status code</returns>
        public async Task<BlockOfficeAPIResponse> AddEthereumWallet(string accessId, string userId, string walletAddress)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, $"/api/Users/{userId}");
            var json = JObject.Parse("{ \"wallet_address\": \"" + walletAddress + "\"}").ToString();
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.PostAsync(uri, body).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Request the challenge for a ticket.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <param name="ticketId">Target ticket id</param>
        /// <returns>JObject containing the challenge string</returns>
        public async Task<BlockOfficeAPIResponse> RequestChallenge(string accessId, string ticketId)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, $"/api/Tickets/{ticketId}/challenge");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.PostAsync(uri, null).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }

        /// <summary>
        /// Redeem a claimed ticket.
        /// </summary>
        /// <param name="accessId">User's auth token</param>
        /// <param name="ticketId">Target ticket id</param>
        /// <param name="signature">Challenge string signed with user's secret key</param>
        /// <returns>Status code</returns>
        public async Task<BlockOfficeAPIResponse> RedeemTicket(string accessId, string ticketId, string signature)
        {
            HttpResponseMessage response;
            var uri = new Uri(BASE_URI, $"/api/tickets/{ticketId}/redeem");
            var json = JObject.Parse("{ \"signature\": \"" + signature + "\"}").ToString();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            lock (HeaderLock)
            {
                Client.DefaultRequestHeaders.Add("Authorization", accessId);
                response = Client.PostAsync(uri, content).Result;
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            var contents = JObject.Parse(await response.Content.ReadAsStringAsync());
            response.Dispose();

            return new BlockOfficeAPIResponse(response.IsSuccessStatusCode, contents);
        }
    }
}
