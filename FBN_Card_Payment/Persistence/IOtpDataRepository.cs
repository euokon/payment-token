using FBN_Card_Payment.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBN_Card_Payment.Persistence
{
    public interface IOtpDataRepository
    {
        Task<OtpItem> GetOtpValue(string otpId);
        Task<ActionResponse> AddOtpData(OtpItem otpItem);
        Task<ActionResponse> ActivateOtp(string otpId);
        Task<ActionResponse> DeactivateOtp(string otpId);
    }
}
