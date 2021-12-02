namespace Examples
{
	internal static class Program
	{
		private static void Main()
		{
			USStreetSingleAddressExample.RunAsync().Wait();
			USStreetLookupsWithMatchStrategyExamples.RunAsync().Wait();
			USStreetMultipleAddressesExample.RunAsync().Wait();
			USZipCodeSingleLookupExample.RunAsync().Wait();
			USZipCodeMultipleLookupsExample.RunAsync().Wait();
			InternationalStreetExample.RunAsync().Wait();
			InternationalAutocompleteExample.RunAsync().Wait();
			USExtractExample.RunAsync().Wait();
			USAutocompleteProExample.RunAsync().Wait();
			USReverseGeoExample.RunAsync().Wait();
		}
	}
}