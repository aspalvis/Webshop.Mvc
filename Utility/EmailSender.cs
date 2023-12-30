using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Utility
{
    public class EmailSender : IEmailSender
    {

        public MailJetSettings _mailJetSettings { get; set; }

        public EmailSender(IOptions<MailJetSettings> mailJetSettings)
        {
            _mailJetSettings = mailJetSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {

            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.ApiSecret);

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            };

            var transactionEmail = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_mailJetSettings.SenderEmail, _mailJetSettings.SenderName))
                .WithSubject(subject)
                .WithHtmlPart(body)
                .WithTo(new SendContact(email))
                .Build();

            var response = await client.SendTransactionalEmailAsync(transactionEmail);
        }
    }
}
