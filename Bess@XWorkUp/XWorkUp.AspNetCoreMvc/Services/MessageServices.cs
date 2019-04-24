using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace XWorkUp.AspNetCoreMvc.Services
{
	// This class is used by the application to send Email and SMS
	// when you turn on two-factor authentication in ASP.NET Identity.
	// For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
	public class AuthMessageSender : IEmailSender, ISmsSender
    {
		public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

		public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
		{
			Options = optionsAccessor.Value;
		}

        public Task SendEmailAsync(string email, string subject, string message)
        {
			// Plug in your email service here to send an email.
			return Execute(Options.Key, subject, message, email);
		}

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

		public Task Execute(string apiKey, string subject, string message, string email)
		{
			 apiKey = Environment.GetEnvironmentVariable("AuthMessageSenderOptions");
				//apiKey = "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";
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("Joe@contoso.com", "Joe Smith"),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};
			msg.AddTo(new EmailAddress(email));

			// Disable click tracking.
			// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
			msg.SetClickTracking(false, false);

			return client.SendEmailAsync(msg);
		}
	}
}
