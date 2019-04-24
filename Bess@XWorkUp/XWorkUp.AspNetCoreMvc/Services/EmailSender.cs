using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XWorkUp.AspNetCoreMvc.Services
{
	public class EmailSender : IEmailSender
	{
		public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
		{
			Options = optionsAccessor.Value;
		}

		public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

		public Task SendEmailAsync(string email, string subject, string message)
		{
			return Execute(Options.Key, subject, message, email);
		}

		public Task Execute(string apiKey, string subject, string message, string email)
		{
			//apiKey = "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";
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("xworkup.com", "XWorkUp freelancer community"),
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
