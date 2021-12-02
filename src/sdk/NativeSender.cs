namespace SmartyStreets
{
	using System;
	using System.IO;
	using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class NativeSender : ISender
	{
		private static readonly Version AssemblyVersion = typeof(NativeSender).Assembly.GetName().Version;

		private static readonly string UserAgent = string.Format("smartystreets-dm (sdk:dotnet@{0}.{1}.{2})",
			AssemblyVersion.Major, AssemblyVersion.Minor, AssemblyVersion.Build);

		private HttpClient httpClient;
		public NativeSender(HttpClient httpClient)
		{
            this.httpClient = httpClient;
        }
		public async Task<Response> SendAsync(Request request, CancellationToken token = default(CancellationToken))
		{
			HttpRequestMessage httprequest = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(request.GetUrl()));
			CopyHeaders(request, httprequest.Headers);
			if (!(request.Method != "POST" || request.Payload == null))
				httprequest.Content = new ByteArrayContent(request.Payload);
			var response = await httpClient.SendAsync(httprequest, HttpCompletionOption.ResponseContentRead, token);
			var statusCode = (int)response.StatusCode;
			var payload = await response.Content.ReadAsByteArrayAsync();
			return new Response(statusCode, payload);
		}

		private static void CopyHeaders(Request request, HttpRequestHeaders frameworkRequest)
		{
			foreach (var item in request.Headers)
				if (item.Key == "Referer")
					frameworkRequest.Referrer = new Uri(item.Value);
				else
					frameworkRequest.TryAddWithoutValidation(item.Key, item.Value);
			frameworkRequest.TryAddWithoutValidation("Content-Type", request.ContentType);
			frameworkRequest.TryAddWithoutValidation("User-Agent", UserAgent);
		}
	}
}