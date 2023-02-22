using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingWebAPI.Common.Exceptions
{
    public class DALException : BookingWebAPIException
    {
        public DALException(string errorCode) : base(errorCode) { }
    }
}
