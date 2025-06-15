using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Extensions
{
	public static class HttpClientExtensions
	{
		public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default, Action<long> getTotalBytes = null)
		{
			// Get the http headers first to examine the content length
			using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
			{
				var contentLength = response.Content.Headers.ContentLength;

				if (getTotalBytes != null && contentLength.HasValue)
					getTotalBytes(contentLength.Value);

				using (var download = await response.Content.ReadAsStreamAsync(cancellationToken))
				{

					// Ignore progress reporting when no progress reporter was 
					// passed or when the content length is unknown
					if (progress == null || !contentLength.HasValue)
					{
						await download.CopyToAsync(destination);
						return;
					}

					// Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
					var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
					// Use extension method to report progress while downloading
					await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
					progress.Report(1);
				}
			}
		}
	}
}
