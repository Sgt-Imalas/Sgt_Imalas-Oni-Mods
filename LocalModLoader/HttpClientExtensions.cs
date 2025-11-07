using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalModLoader
{
	internal static class HttpClientExtensions
	{
		public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination)
		{
			using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
			{
				using (var download = await response.Content.ReadAsStreamAsync())
				{
					await download.CopyToAsync(destination);
				}
			}
		}
	}
}
