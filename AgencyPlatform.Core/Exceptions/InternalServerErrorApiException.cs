using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Core.Exceptions
{
    public class InternalServerErrorApiException : ApiException
    {
        public InternalServerErrorApiException(string message)
            : base(message, 500, "INTERNAL_SERVER_ERROR")
        {
        }
    }
}
