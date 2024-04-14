namespace FirstWeb.AspNetCore.SendMail
{
	public interface ISendMail
	{
		public Task<string> SendMailAsync(string _to, string _subject, string _body);
	}
}
