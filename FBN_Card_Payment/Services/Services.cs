using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FBN_Card_Payment.Services
{
    public interface IEmailService
    {
        bool SendEmail(string toAddrs, string mailSubject, string mailBody);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        MailMessage mail;
        SmtpClient smtp;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool SendEmail(string toAddrs, string mailSubject, string mailBody)
        {
            bool isSend = false;

            try
            {
                mail = new MailMessage();
                if (toAddrs != null)
                    mail.To.Add(new MailAddress(toAddrs));

                mail.From = new MailAddress(configuration.GetValue<string>("AppSettings:MailId"), configuration.GetValue<string>("AppSettings:MailName"));
                mail.Subject = mailSubject;
                mail.Body = mailBody;
                mail.IsBodyHtml = true;
                mail.ReplyToList.Add(configuration.GetValue<string>("AppSettings:ReplyMailId"));

                smtp = new SmtpClient
                {
                    Host = configuration.GetValue<string>("AppSettings:MailServer"),
                    Credentials = new NetworkCredential(configuration.GetValue<string>("AppSettings:MailId"), configuration.GetValue<string>("AppSettings:MailPwd"), configuration.GetValue<string>("AppSettings:MailDomain")),
                    UseDefaultCredentials = false,
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;//|SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string mailId = Guid.NewGuid().ToString();
                smtp.SendAsync(mail, mailId);

                isSend = true;
            }
            catch (Exception ex)
            {
                isSend = false;
            }
            return isSend;
        }

    }

}
