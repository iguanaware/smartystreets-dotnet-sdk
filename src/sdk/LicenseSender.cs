namespace SmartyStreets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LicenseSender : ISender
    {
        private readonly List<string> licenses;
        private readonly ISender inner;

        public LicenseSender(List<string> licenses, ISender inner)
        {
            this.licenses = licenses;
            this.inner = inner;
        }

        public async Task<Response> SendAsync(Request request, CancellationToken token = default(CancellationToken))
        {
            request.SetParameter("license", String.Join(",", this.licenses));
            return await this.inner.SendAsync(request, token);
        }
    }
}