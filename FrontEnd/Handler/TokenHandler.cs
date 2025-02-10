
namespace FrontEnd.Handler
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Kiểm tra xem header đã có chưa, nếu chưa thì thêm vào
            if (!request.Headers.Contains("X-Client-Source"))
            {
                request.Headers.Add("X-Client-Source", "khanhlongtran.com");
            }

            if (!request.Headers.Contains("Token"))
            {
                request.Headers.Add("Token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJraGFuaGxvbmd0cmFuLmNvbSIsIm5hbWUiOiJLaGFuaCIsImlhdCI6MTUxNjIzOTAyMiwiZXhwIjoyNTE2MjM5MDIyLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjpbIk9yZGVyTWFuYWdlciIsIk1hbmFnZXIiXX0.y8i_V66x3CgaE-UrfhG3N6dNmiz8F9iIHAijIvNpb94");
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
