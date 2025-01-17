﻿namespace SmartyStreets
{
	using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RetrySender : ISender
	{
		private readonly int maxRetries;
		private readonly ISender inner;

		public RetrySender(int maxRetries, ISender inner)
		{
			this.maxRetries = maxRetries;
			this.inner = inner;
		}

		public async Task<Response> SendAsync(Request request, CancellationToken token = default(CancellationToken))
		{
			for (var i = 0; i <= this.maxRetries; i++)
			{
				var response = await this.TrySendAsync(request, i, token);
				if (response != null)
					return response;
			}

			return null;
		}

		private async Task<Response> TrySendAsync(Request request, int attempt, CancellationToken token)
		{
			try
			{
				return await this.inner.SendAsync(request, token);
			}
			catch (Exception) // TODO: catch HTTP 400, 413, 422 and just throw.
			{
				if (attempt >= this.maxRetries)
					throw;
			}

			return null;
		}
	}
}