using System.Threading;
using System.Threading.Tasks;

namespace SmartyStreets
{
	public class MockSender : ISender
	{
		private readonly Response response;
		public Request Request { get; private set; }

		public MockSender(Response response)
		{
			this.response = response;
		}

		public Task<Response> SendAsync(Request request, CancellationToken token)
		{
			this.Request = request;
			return Task.FromResult(this.response);
		}
	}
}