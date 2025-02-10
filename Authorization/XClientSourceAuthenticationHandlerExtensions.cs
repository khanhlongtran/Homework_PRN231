using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization
{
    public static class XClientSourceAuthenticationHandlerExtensions
    {
        public static AuthenticationBuilder AddXClientSource(this AuthenticationBuilder builder, Action<XClientSourceAuthenticationHandlerOptions> configureOptions)
        {
            return builder
                    .AddScheme<XClientSourceAuthenticationHandlerOptions, XClientSourceAuthenticationHandler>("X-Client-Source", configureOptions);
        }

    }
}
