namespace FrontEnd.Handler
{
    public class BaseServiceHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseServiceHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("ApiClient");
        }
    }
}
