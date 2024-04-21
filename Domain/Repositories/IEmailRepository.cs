namespace Domain.Repositories
{
    public interface IEmailRepository
    {
        Task SendAsync(string[] to, string subject, string body, CancellationToken cancellation = default);
    }
}
