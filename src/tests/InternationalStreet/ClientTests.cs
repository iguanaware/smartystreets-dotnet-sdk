﻿namespace SmartyStreets.InternationalStreet
{
	using System.Collections.Generic;
	using System.Text;
    using System.Threading.Tasks;
    using InternationalStreetApi;
	using NUnit.Framework;

	[TestFixture]
	public class ClientTests
	{
		private RequestCapturingSender capturingSender;
		private URLPrefixSender sender;


		[SetUp]
		public void Setup()
		{
			this.capturingSender = new RequestCapturingSender();
			this.sender = new URLPrefixSender("http://localhost/", this.capturingSender);
		}

		[Test]
		public async Task TestSendingFreeformLookupAsync()
		{
			var serializer = new FakeSerializer(null);
			var client = new Client(this.sender, serializer);
			var lookup = new Lookup("freeform", "USA");

			await client.SendAsync(lookup);

			Assert.AreEqual("http://localhost/?country=USA&freeform=freeform", this.capturingSender.Request.GetUrl());
		}

		[Test]
		public async Task TestSendingSingleFullyPopulatedLookupAsync()
		{
			const string expectedUrl = "http://localhost/?country=0&geocode=true&language=native&freeform=1" +
			                           "&address1=2&address2=3&address3=4&address4=5&organization=6&locality=7&administrative_area=8&postal_code=9";
			var serializer = new FakeSerializer(null);
			var client = new Client(this.sender, serializer);
			var lookup = new Lookup
			{
				Country = "0",
				Geocode = true,
				Language = LanguageMode.NATIVE,
				Freeform = "1",
				Address1 = "2",
				Address2 = "3",
				Address3 = "4",
				Address4 = "5",
				Organization = "6",
				Locality = "7",
				AdministrativeArea = "8",
				PostalCode = "9"
			};

			await client.SendAsync(lookup);

			Assert.AreEqual(expectedUrl, this.capturingSender.Request.GetUrl());
		}

		[Test]
		public void TestEmptyLookupRejected()
		{
			var crashSender = new MockCrashingSender();
			var client = new Client(crashSender, null);

			Assert.ThrowsAsync<UnprocessableEntityException>(async () => await client.SendAsync(new Lookup()));
		}

		[Test]
		public void TestRejectsLookupsWithOnlyCountry()
		{
			var crashSender = new MockCrashingSender();
			var client = new Client(crashSender, null);
			var lookup = new Lookup {Country = "0"};

			Assert.ThrowsAsync<UnprocessableEntityException>(async () => await client.SendAsync(lookup));
		}

		[Test]
		public void TestRejectsLookupsWithOnlyCountryAndAddress1()
		{
			var crashSender = new MockCrashingSender();
			var client = new Client(crashSender, null);
			var lookup = new Lookup
			{
				Country = "0",
				Address1 = "1"
			};

			Assert.ThrowsAsync<UnprocessableEntityException>(async () => await client.SendAsync(lookup));
		}

		[Test]
		public void TestRejectsLookupsWithOnlyCountryAndAddress1AndLocality()
		{
			var crashSender = new MockCrashingSender();
			var client = new Client(crashSender, null);
			var lookup = new Lookup
			{
				Country = "0",
				Address1 = "1",
				Locality = "2"
			};

			Assert.ThrowsAsync<UnprocessableEntityException>(async () => await client.SendAsync(lookup));
		}

		[Test]
		public void TestRejectsLookupsWithOnlyCountryAndAddress1AndAdministrativeArea()
		{
			var crashSender = new MockCrashingSender();
			var client = new Client(crashSender, null);
			var lookup = new Lookup
			{
				Country = "0",
				Address1 = "1",
				AdministrativeArea = "2"
			};

			Assert.ThrowsAsync<UnprocessableEntityException>(async () => await client.SendAsync(lookup));
		}

		[Test]
		public async Task TestAcceptsLookupsWithEnoughInfoAsync()
		{
			var serializer = new FakeSerializer(null);
			var client = new Client(new RequestCapturingSender(), serializer);
			var lookup = new Lookup
			{
				Country = "0",
				Freeform = "1"
			};

			await client.SendAsync(lookup);

			lookup.Freeform = null;
			lookup.Address1 = "1";
			lookup.PostalCode = "2";
			await client.SendAsync(lookup);

			lookup.PostalCode = null;
			lookup.Locality = "3";
			lookup.AdministrativeArea = "4";
			await client.SendAsync(lookup);
		}

		[Test]
		public async Task TestDeserializeCalledWithResponseBodyAsync()
		{
			var response = new Response(0, Encoding.ASCII.GetBytes("Hello, World!"));
			var mockSender = new MockSender(response);
			var deserializer = new FakeDeserializer(null);
			var client = new Client(mockSender, deserializer);

			await client.SendAsync(new Lookup("1", "2"));

			Assert.AreEqual(response.Payload, deserializer.Payload);
		}

		[Test]
		public async Task TestCandidatesCorrectlyAssignedToLookupAsync()
		{
			var expectedCandidates = new List<Candidate> {new Candidate(), new Candidate()};
			var lookup = new Lookup("1", "2");

			var mockSender = new MockSender(new Response(0, Encoding.ASCII.GetBytes("[]")));
			var deserializer = new FakeDeserializer(expectedCandidates);
			var client = new Client(mockSender, deserializer);

			await client.SendAsync(lookup);

			Assert.AreEqual(expectedCandidates[0], lookup.Result[0]);
			Assert.AreEqual(expectedCandidates[1], lookup.Result[1]);
		}
	}
}