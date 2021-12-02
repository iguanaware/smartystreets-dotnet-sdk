using System.Threading;
using System.Threading.Tasks;

namespace SmartyStreets
{
	public class RequestCapturingSender : ISender
	{
		public Request Request { get; private set; }

		public Task<Response> SendAsync(Request request, CancellationToken token)
		{
			this.Request = request;

			return Task.FromResult(new Response(200, new byte[0]));
		}
	}
}