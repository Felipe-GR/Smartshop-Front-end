using System;
using System.Net;
using System.Net.Mail;

namespace Send_Email
{
    class Program
    {
        static void Main(string[] args)
        {
            var fromAddress = new MailAddress("demosmartshop@gmail.com", "smartshop");
            var toAddress = new MailAddress("luisguerron@hotmail.com", "Felipe");
            const string fromPassword = "1234ABcD*";
            const string subject = "Subject";
            const string body = "Body";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
