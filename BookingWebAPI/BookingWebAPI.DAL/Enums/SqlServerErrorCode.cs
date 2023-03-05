using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingWebAPI.DAL.Enums
{
    internal enum SqlServerErrorCode
    {
        CannotInsertNull = 515,
        StringOrBinaryTruncated = 2628
    }
}
