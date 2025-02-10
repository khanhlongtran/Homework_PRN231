using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Authorization
{
    public class XClientSourceAuthenticationHandlerOptions : AuthenticationSchemeOptions
    {
        // Options cần nhận vào 2 thằng là X-Client-Source và Token nên sẽ cần một Func với 2 tham số đó và return bool để check xem đúng hay sai
        // Ngoai ra can them thang IssuerSigningKey de xac thuc chu ky 
        public Func<string, SecurityToken, ClaimsPrincipal, bool> ClientValidator { get; set; } = (clientSource, token, principal) => false;
        public string IssuerSigningKey { get; set; } = string.Empty;
    }
}
