using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Authorization
{
    public class XClientSourceAuthenticationHandler : AuthenticationHandler<XClientSourceAuthenticationHandlerOptions>
    {
        public XClientSourceAuthenticationHandler(IOptionsMonitor<XClientSourceAuthenticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        // Ở đây diễn ra việc mình sẽ authen như nào sau khi add app.UseAuthentication();
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Ghtk no yeu cau cai nay nen la cung phai lam
            var clientSource = Context.Request.Headers["X-Client-Source"];
            var token = Context.Request.Headers["Token"];

            if (clientSource.Count == 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing X-Client-Source header"));
            }
            if (token.Count == 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Token header"));
            }

            var clientSourceValue = clientSource.FirstOrDefault();
            var tokenValue = token.FirstOrDefault();

            if (!string.IsNullOrEmpty(clientSourceValue) && !string.IsNullOrEmpty(token) && VerifyClient(clientSourceValue, tokenValue!, out var principal))
            {
                //var identity = new ClaimsIdentity(Scheme.Name);
                //identity.AddClaim(new Claim(ClaimTypes.Name, clientSourceValue));
                //principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));

            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid X-Client-Source or Token"));
            }


        }

        // Mình thêm list role trong jwt rồi, khi mà validate -> cần ghi trong biến principal 
        private bool VerifyClient(string clientSourceValue, string tokenValue, out ClaimsPrincipal? principal)
        {
            if (!Validate(tokenValue, out var token, out principal))
            {
                return false;
            }
            var sub = (token as JwtSecurityToken)!.Subject; // Convert "sub" in jwt to string for comparing
            // So sánh X-Client-Source với sub trong jwt, phải trùng thì mới đúng
            if (clientSourceValue != sub)
            {
                return false;
            }
            // Thằng này sẽ kết nối tới database để kiểm tra thông tin client trong db vì 1 số lý do
            // Giả sử tài khoản bị khóa -> jwt phi trạng thái nên vẫn còn hạn sử dụng nhưng db thì đã update 
            // Đồng thời phải kiểm tra cả db nhưng vẫn phải sử dụng thằng jwt để compare vì 
            // 1. Vì là stateless => giảm tải cho Server và Database phải query quá nhiều
            // 2. Vì stateless authentication => không cần lưu trạng thái người dùng, mọi thứ đều trong token, cần lắm mới phải vào db
            // 3. Bảo mật bằng SigningKey => ngăn giả mạo token
            // 4. Ngoài ra đây cũng là 1 cách để implement JWT sử dụng database
            /*
             * API token sẽ được gửi cùng với client source xem người dùng có quyền gì -> vào trang cấu hình API để đăng kí -> họ có 1 db để lưu trữ các token của người dùng đó -> với các token khác nhau thì họ có thể có các cái quyền khác nhau -> phân quyền nè -> lưu token cùng danh sách các quyền ứng với token này cx sẽ lưu vào db -> mỗi request đến truy vấn db -> sẽ biết đc quyền, hết hạn hay chưa -> 3 bảng gồm client - token - quyền tương ứng với token. 1 token -> nhiều token -> 1 token nhiều quyền truy cập 
             */
            // Đây là delegate => xem phần implement của nó ở program
            if (!Options.ClientValidator(clientSourceValue, token!, principal!))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// token (chuỗi đã mã hóa) => Giải mã thành json => trích từng phần ném vô ClaimsPrincipal
        /// </summary>
        /// <param name="tokenValue"></param>
        /// <param name="token"></param>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        private bool Validate(string tokenValue, out SecurityToken? token, out ClaimsPrincipal? claimsPrincipal)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.IssuerSigningKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
            try
            {
                claimsPrincipal = handler.ValidateToken(tokenValue,
                                                        tokenValidationParameters,
                                                        out token);
                return true;
            } catch (Exception ex)
            {
                token = null;
                claimsPrincipal = null;
                return false;
            }
        }
    }
}
