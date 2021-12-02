namespace SmartyStreets.InternationalAutocompleteApi
{
	using System;
	using System.Collections;
	using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    ///     This client sends lookups to the SmartyStreets US Autocomplete API,
    ///     and attaches the results to the appropriate Lookup objects.
    /// </summary>
    public class Client : IInternationalAutoCompleteClient
	{
		private readonly ISender sender;
		private readonly ISerializer serializer;

		public Client(ISender sender, ISerializer serializer)
		{
			this.sender = sender;
			this.serializer = serializer;
		}

		public async Task SendAsync(Lookup lookup)
		{
			if (lookup == null)
				throw new ArgumentNullException("lookup");

			if (string.IsNullOrEmpty(lookup?.Search))
				throw new SmartyException("Send() must be passed a Lookup with the prefix field set.");

			var request = BuildRequest(lookup);

			var response = await this.sender.SendAsync(request);

			using (var payloadStream = new MemoryStream(response.Payload))
			{
				var result = this.serializer.Deserialize<Result>(payloadStream) ?? new Result();
				var candidates = result.Candidates;
				lookup.Result = candidates;
			}
		}

		private static Request BuildRequest(Lookup lookup)
		{
			var request = new Request();
			
			request.SetParameter("search", lookup.Search);
			request.SetParameter("country", lookup.Country);
			request.SetParameter("max_results", lookup.GetMaxSuggestionsStringIfSet());
			request.SetParameter("include_only_administrative_area", lookup.AdministrativeArea);
			request.SetParameter("include_only_locality", lookup.Locality);
			request.SetParameter("include_only_postal_code", lookup.PostalCode);
			
			return request;
		}
	}
}