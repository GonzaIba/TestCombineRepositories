using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            // Command-line argument must be the SMTP host.
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SMTP_HOST"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_PORT"]));
            if (ConfigurationManager.AppSettings["EMAIL_SSL"] == "Y")
            {
                client.EnableSsl = true;
            }
            else
            {
                client.EnableSsl = false;
            }

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTP_USER"], ConfigurationManager.AppSettings["SMTP_PASSWORD"]);

            // Specify the email sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress("jane@contoso.com", "Jane " + (char)0xD8 + " Clayton", System.Text.Encoding.UTF8);
            // Set destinations for the email message.
            MailAddress to = new MailAddress(message.Destination);
            // Specify the message content.
            MailMessage messageToSend = new MailMessage(from, to);
            messageToSend.Body = "This is a test email message sent by an application. ";


            messageToSend.Body = message.Body;
            messageToSend.BodyEncoding = System.Text.Encoding.UTF8;
            messageToSend.IsBodyHtml = true;
            messageToSend.Subject = message.Subject;
            messageToSend.SubjectEncoding = System.Text.Encoding.UTF8;

            messageToSend.Priority = MailPriority.High;




            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            // The userState can be any object that allows your callback 
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = "test message1";
            client.SendAsync(messageToSend, userState);

            // Clean up.
            //messageToSend.Dispose();
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                
            }
            if (e.Error != null)
            {
                
            }
            else
            {
                
            }            
        }
    }
}