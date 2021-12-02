using System.Threading;
using System.Threading.Tasks;

namespace SmartyStreets
{
	public class MockStatusCodeSender : ISender
	{
		private readonly int statusCode;

		public MockStatusCodeSender(int statusCode)
		{
			this.statusCode = statusCode;
		}

		public Task<Response> SendAsync(Request request, CancellationToken token)
		{
			if (this.statusCode == 0)
				return null;

			return Task.FromResult(new Response(this.statusCode, null));
		}
	}
}