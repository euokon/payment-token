using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBN_Card_Payment.DataObjects
{
    public class ActionResponse
    {
        public bool Status { get; set; }
        public string ResponseCode { get; set; }
        public string Message { get; set; }
    }
}
