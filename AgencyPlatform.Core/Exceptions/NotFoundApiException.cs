using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Core.Exceptions
{
    public class NotFoundApiException : ApiException
    {
        public NotFoundApiException(string message)
            : base(message, 404, "NOT_FOUND")
        {
        }
    }
}
