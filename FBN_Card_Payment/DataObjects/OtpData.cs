using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBN_Card_Payment.DataObjects
{
    //public class OtpData
    //{
    //}

    public class OtpItem
    {
        public string OtpId { get; set; }
        public string Username { get; set; }
        public string OtpValue { get; set; }
        public char OtpStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? DateActivated { get; set; }
        public DateTime? DateDeactivated { get; set; }
    }

    public class UserData
    {
        public string User { get; set; }
        public string OtpValue { get; set; }
    }
}
