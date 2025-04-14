using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Core.Exceptions
{
    public class UnauthorizedAccessApiException : ApiException
    {
        public UnauthorizedAccessApiException(string message)
            : base(message, 403, "UNAUTHORIZED_ACCESS")
        {
        }
    }
}
