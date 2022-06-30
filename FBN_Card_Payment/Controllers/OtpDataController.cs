using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBN_Card_Payment.DataObjects;
using FBN_Card_Payment.Persistence;
using FBN_Card_Payment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBN_Card_Payment.Controllers
{
    [Route("api/card-request")]
    [ApiController]
    public class OtpDataController : ControllerBase
    {
        private readonly IOtpDataRepository _otpDataRepository;
        private readonly IEmailService _emailService;
        public OtpDataController(IOtpDataRepository otpDataRepository, IEmailService emailService)
        {
            _otpDataRepository = otpDataRepository;
            _emailService = emailService;
        }

        //public string GenerateOtp()
        //{
        //    Random random = new Random();
        //    var digits = "0123456789";
        //    var randomOtp = new string(Enumerable.Repeat(digits, 6)
        //        .Select(a => a[random.Next(a.Length)]).ToArray());
        //    string stringEncode;
        //    byte[] data = ASCIIEncoding.ASCII.GetBytes(randomOtp);
        //    stringEncode = Convert.ToBase64String(data);

        //    return randomOtp;
        //}

        private string EncryptOtp()
        {
            string stringEncode;
            string randomTextDataEncode;
            string dbString = "";
            string randomText = "9876543210";

            Random random = new Random();
            var digits = "0123456789";
            var randomOtp = new string(Enumerable.Repeat(digits, 6)
                .Select(a => a[random.Next(a.Length)]).ToArray());

            try
            {
                byte[] randomTextData = ASCIIEncoding.ASCII.GetBytes(randomText);
                randomTextDataEncode = Convert.ToBase64String(randomTextData);
                byte[] data = ASCIIEncoding.ASCII.GetBytes(randomOtp);
                stringEncode = Convert.ToBase64String(data);

                dbString = stringEncode + randomTextDataEncode;
            }
            catch (Exception ex)
            {
                dbString = "";
            }

            return dbString;
        }

        private string DecryptOtp(string stringEncode)
        {
            string stringDecode;
            string tokenValue = "";
            try
            {
                byte[] data = Convert.FromBase64String(stringEncode);
                stringDecode = System.Text.ASCIIEncoding.ASCII.GetString(data);

                tokenValue = stringDecode.Substring(0, 6);
            }
            catch (Exception ex)
            {
                tokenValue = "";
            }
            return tokenValue;
        }

        [HttpPost("generate-otp")]
        public async Task<ActionResult> AddOtpData(UserData userData)
        {
            var encrytedOtp = EncryptOtp();
            var decrytedOtp = DecryptOtp(encrytedOtp);

            string subject = $"TEST TRANSACTION";
            string toAddrs = "eokon@unionbankng.com";
            //string toAddrs = "euokon@gmail.com";
            string body = @$"Dear Esteemed Customer,<br/><br/>
                            See below the OTP needed to complete your transaction. <br/><br/>

                            {decrytedOtp}

                            <br/> <br/>  
                            Regards,<br/>
                            Thank you for your patronage";

            _emailService.SendEmail(toAddrs, subject, body);

            OtpItem otpItem = new OtpItem()
            {
                OtpId = Guid.NewGuid().ToString().Replace("-", ""),
                Username = userData.User,
                OtpValue = encrytedOtp
            };

            var response = await _otpDataRepository.AddOtpData(otpItem).ConfigureAwait(false);
            if (response.Status)
                return Ok();
            return BadRequest();
        }

        [HttpPost("activate-otp")]
        public async Task<ActionResult> ActivateOtp(UserData userData)
        {
            var otpUser = await _otpDataRepository.GetOtpValue(userData.User).ConfigureAwait(false);
            string decryptedToken = DecryptOtp(otpUser.OtpValue);

            if (otpUser.OtpId != null && userData.OtpValue.Equals(decryptedToken))
            {
                var response = await _otpDataRepository.ActivateOtp(otpUser.OtpId).ConfigureAwait(false);
                if (response.Status)
                    return Ok();
                return BadRequest("Transaction failed, please try again");
            }
            else
            {
                return NotFound("OTP is invalid or has expired");
            }
        }

        [HttpPost("deactivate-otp")]
        public async Task<ActionResult> DeactivateOtp(UserData userData)
        {
            var otpUser = await _otpDataRepository.GetOtpValue(userData.User).ConfigureAwait(false);
            string decryptedToken = DecryptOtp(otpUser.OtpValue);

            if (otpUser.OtpId != null && userData.OtpValue.Equals(decryptedToken))
            {
                var response = await _otpDataRepository.DeactivateOtp(otpUser.OtpId).ConfigureAwait(false);
                if (response.Status)
                    return Ok();
                return BadRequest("Transaction failed, please try again");
            }
            else
            {
                return NotFound("OTP is invalid or has expired");
            }
        }


    }
}