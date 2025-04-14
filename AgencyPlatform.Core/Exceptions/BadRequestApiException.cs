using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Core.Exceptions
{
    public class BadRequestApiException : ApiException
    {
        public BadRequestApiException(string message)
            : base(message, 400, "BAD_REQUEST")
        {
        }
    }
}
