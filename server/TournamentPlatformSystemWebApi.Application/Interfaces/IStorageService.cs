using System.IO;
using System.Threading.Tasks;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public record StorageUploadResult(string FileId, string Url);

public interface IStorageService
{
    /// <summary>
    /// Uploads a stream and returns a public URL and file id.
    /// </summary>
    Task<StorageUploadResult> UploadAsync(Stream stream, string fileName, string contentType);
}
