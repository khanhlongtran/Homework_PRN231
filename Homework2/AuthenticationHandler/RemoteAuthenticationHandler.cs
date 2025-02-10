using ClientAuthentication;

namespace Homework2.AuthenticationHandler
{
    public class RemoteAuthenticationHandler : IClientSourceAuthenticationHandler
    {
        private string _authenticationServiceUrl;
        private static HttpClient _httpClient = new HttpClient();

        public RemoteAuthenticationHandler(string authenticationServiceUrl)
        {
            _authenticationServiceUrl = authenticationServiceUrl;
        }
        public bool Validate(string clientSource)
        {
            if (string.IsNullOrEmpty(clientSource))
            {
                return false;
            }

            var response = _httpClient.GetAsync($"{_authenticationServiceUrl}/api/clientsource/{clientSource}").Result;

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
