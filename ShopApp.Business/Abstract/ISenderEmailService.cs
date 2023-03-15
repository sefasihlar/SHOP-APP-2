namespace ShopApp.Business.Abstract
{
	public interface IEmailSenderService
	{
		Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true);
		Task SendEmailAsync(string[] tos, string subject, string body, bool isBodyHtml = true);
	}
}
