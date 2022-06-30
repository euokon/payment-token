using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBN_Card_Payment.Helper
{
    public static class ConvertDateExtension
    {
        public static DateTime? ToNullableDateTime(this string date)
        {
            return string.IsNullOrEmpty(date) ? (DateTime?)null : Convert.ToDateTime(date);
        }
    }
}
