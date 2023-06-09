﻿using Microsoft.AspNet.Identity;
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
            if (ConfigurationManager.AppSettings["USE_SMTP"] == "Y")
            {
                await configSendSmtpasync(message); 
            }
            else
            {
                await configSendGridasync(message);
            }

        }

        private async Task configSendGridasync(IdentityMessage message)
        {
            EmailSendGridApiService service = new EmailSendGridApiService();

            EmailSendGridApiService.SendMailRequest request = new EmailSendGridApiService.SendMailRequest();

            request.personalizations = new List<EmailSendGridApiService.Personalization>();
            EmailSendGridApiService.Personalization personalization = new EmailSendGridApiService.Personalization();
            personalization.subject = message.Subject;
            EmailSendGridApiService.Email email = new EmailSendGridApiService.Email();
            email.email = message.Destination;
            personalization.to = new List<EmailSendGridApiService.Email>();
            personalization.to.Add(email);
            request.personalizations.Add(personalization);

            EmailSendGridApiService.Email emailFrom = new EmailSendGridApiService.Email();
            emailFrom.email = ConfigurationManager.AppSettings["EMAIL_FROM"];
            request.from = emailFrom;

            request.content = new List<EmailSendGridApiService.Content>();

            EmailSendGridApiService.Content content = new EmailSendGridApiService.Content();
            content.type = "text/html";
            content.value = message.Body;
            request.content.Add(content);

            service.methodSendMail(request);
        }

        private async Task configSendSmtpasync(IdentityMessage message)
        {
            // Command-line argument must be the SMTP host.
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SMTP_HOST"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_PORT"]));
            
            client.EnableSsl = (ConfigurationManager.AppSettings["EMAIL_SSL"] == "Y");

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTP_USER"], ConfigurationManager.AppSettings["SMTP_PASSWORD"]);

            // Specify the email sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"], ConfigurationManager.AppSettings["EMAIL_DISPLAY_NAME"], System.Text.Encoding.UTF8);
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