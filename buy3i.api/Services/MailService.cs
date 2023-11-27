using MailKit.Net.Smtp;
using MailKit.Security;
using buy3i.api.Configuration;
using buy3i.api.Models;
using Microsoft.Extensions.Options;
using MimeKit;

namespace buy3i.api.Services
{
	public class MailService : IMailService
	{
		private readonly MailSettings _settings;

		public MailService(IOptions<MailSettings> settings)
		{
			_settings = settings.Value;
		}

		public async Task<string> SendAsync(MailData mailData, CancellationToken ct = default)
		{
			try
			{
				var mail = new MimeMessage();

				#region Sender / Receiver
				mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
				mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

				foreach (string mailAddress in mailData.To)
					mail.To.Add(MailboxAddress.Parse(mailAddress));

				if (!string.IsNullOrEmpty(mailData.ReplyTo)) 
					mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));
				if (mailData.Bcc != null)
				{
					foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
						mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
				}

				if (mailData.Cc != null)
				{
					foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
						mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
				}
				#endregion

				#region Content

				var body = new BodyBuilder();
				mail.Subject = mailData.Subject;
				body.HtmlBody = mailData.Body;
				mail.Body = body.ToMessageBody();

				#endregion

				#region Send Mail

				using var smtp = new SmtpClient();

				if (_settings.UseSSL)
				{
					smtp.CheckCertificateRevocation = false;
					await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
				}
				else if (_settings.UseStartTls)
				{
					smtp.CheckCertificateRevocation = false;
					await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
				}
				await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
				await smtp.SendAsync(mail, ct);
				await smtp.DisconnectAsync(true, ct);

				#endregion

				return "true";
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}
