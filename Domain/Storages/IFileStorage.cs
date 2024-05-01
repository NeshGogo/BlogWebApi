
namespace Domain.Storages
{
    public interface IFileStorage
    {
        Task<string> EditFileAsync(byte[] content, string extension, string container, string route, string contentType, CancellationToken cancellation = default);
        Task RemoveFileAsync(string container, string route, CancellationToken cancellation = default);
        Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType, CancellationToken cancellation = default);
    }
}
