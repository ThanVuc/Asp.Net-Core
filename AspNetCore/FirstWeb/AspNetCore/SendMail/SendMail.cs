
using FirstWeb.AspNetCore.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FirstWeb.AspNetCore.SendMail
{
	public class SendMail : ISendMail
	{
		MailSettings setting;
		public SendMail(IOptions<MailSettings> _setting)
		{
			setting = _setting.Value;
		}

		public string PrintSetting()
		{
			return $"{setting.Mail} {setting.DisplayName} {setting.Host} {setting.Port}";
		}

		public async Task<string> SendMailAsync(string _to, string _subject, string _body)
		{
			MailContent mailContent = new MailContent(_to, _subject, _body);
			var email = new MimeMessage();
			email.Sender = new MailboxAddress(setting.DisplayName, setting.Mail);
			email.From.Add(new MailboxAddress(setting.DisplayName, setting.Mail));
			email.To.Add(new MailboxAddress(null, mailContent._to));
			email.Subject = mailContent._subject;

			var bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = mailContent._body;

			string fileBase = "wwwroot/Html";

            foreach (string filePath in Directory.EnumerateFiles(fileBase))
            {
				using (var fileStream = File.OpenRead(filePath))
				{
					await bodyBuilder.Attachments.AddAsync(filePath, fileStream, ContentType.Parse("Text/html"));
				}
			}

			email.Body = bodyBuilder.ToMessageBody();

			using var smtp = new MailKit.Net.Smtp.SmtpClient();

			string announce = "";

			try
			{
				smtp.Connect(setting.Host, setting.Port, MailKit.Security.SecureSocketOptions.StartTls);

				Console.WriteLine(setting.Mail + setting.Password);
				smtp.Authenticate(setting.Mail, setting.Password);
				await smtp.SendAsync(email);
				announce = "Send mail successful";

			}
			catch (Exception e)
			{
				announce = $"Error while send, {e.Message}";
			}
			return announce;
		}
	}

	public class MailContent
	{
		public MailContent(string to, string subject, string body)
		{
			_to = to;
			_subject = subject;
			_body = body;
		}

		public string _to { get; }
		public string _subject { get; }
		public string _body { get; }
	}
}
