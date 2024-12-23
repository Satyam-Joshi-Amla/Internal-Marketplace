using System.Net;
using System.Net.Mail;

namespace AmlaMarketPlace.ConfigurationManager.UtilityMethods
{
    public static class MailUtility
    {
        public static void SendMessageOnMail(string toEmail, string mailSubject, string mailMessage)
        {
            try
            {
                var fromAddress = new MailAddress("rockervc7@gmail.com", "Amla Marketplace");
                var toAddress = new MailAddress(toEmail);
                const string fromAppPassword = "cxml addl yidq sybs"; // not to be touched.
                string subject = mailSubject;
                string body = $"{mailMessage}";

                var smtp = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromAppPassword)
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to send email: " + ex.Message);
                throw new Exception("Unexpected errors in utility method of Sending mail", ex);
            }
        }


    }
}
