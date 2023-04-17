using Azure.Core;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace ProiectLicenta.Email
{
    public class EmailSender:IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public async Task Send(string email,string subject, string body)
        {
            var server = _configuration["EmailSettings:Server"];
            var port = _configuration["EmailSettings:Port"];
            var fromMail = _configuration["EmailSettings:FromMail"];
            var password = _configuration["EmailSettings:Password"];
            var userName = _configuration["EmailSettings:UserName"];
            if (server != null && port != null && fromMail != null && password != null && userName != null)
            {

                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(fromMail),
                    To = { new MailAddress(email) },
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };


                var client = new SmtpClient(server, int.Parse(port));
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = true;



                await client.SendMailAsync(mailMessage);
            }
            else throw new Exception("EmailSender.cs improper configurated!");
        }
    }
}
