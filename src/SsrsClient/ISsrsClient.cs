using SsrsClient.Rest.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SsrsClient
{
    /// <summary>Provides operations to manage reports and folders in SSRS.</summary>
    public interface ISsrsClient
    {
        // Reports
        Task<IReadOnlyList<CatalogItem>> ListReportsAsync(string folderPath, CancellationToken cancellationToken = default);
        Task<CatalogItem> GetReportAsync(string reportPath, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadReportAsync(string reportPath, CancellationToken cancellationToken = default);
        Task<CatalogItem> UploadReportAsync(string folderPath, string reportName, byte[] rdlContent, bool overwrite = false, CancellationToken cancellationToken = default);
        Task DeleteReportAsync(string reportPath, CancellationToken cancellationToken = default);

        // Folders
        Task<IReadOnlyList<CatalogItem>> ListFoldersAsync(string folderPath, CancellationToken cancellationToken = default);
        Task<CatalogItem> CreateFolderAsync(string folderPath, CancellationToken cancellationToken = default);
        Task DeleteFolderAsync(string folderPath, CancellationToken cancellationToken = default);
    }
}
