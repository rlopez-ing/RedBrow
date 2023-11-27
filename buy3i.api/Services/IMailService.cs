using buy3i.api.Models;

namespace buy3i.api.Services
{
	public interface IMailService
	{
		Task<string> SendAsync(MailData mailData, CancellationToken ct);
	}
}
