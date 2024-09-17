namespace Contracts;

public interface IEmailRepository
{
    Task SendAsync(string[] to, string subject, string body, bool isBodyHtml = true, CancellationToken cancellation = default);
}
