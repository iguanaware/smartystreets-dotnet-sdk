namespace SmartyStreets.InternationalAutocompleteApi
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class ClientTests
    {
        private RequestCapturingSender capturingSender;
        private URLPrefixSender urlSender;

        [SetUp]
        public void Setup()
        {
            this.capturingSender = new RequestCapturingSender();
            this.urlSender = new URLPrefixSender("http://localhost/", this.capturingSender);
        }

        #region [ Single Lookup ]

        [Test]
        public async Task TestSendingSinglePrefixOnlyLookup()
        {
            var serializer = new FakeSerializer(new byte[0]);
            var client = new Client(this.urlSender, serializer);

            await client.SendAsync(new Lookup("1"));

            Assert.AreEqual("http://localhost/?search=1",
                this.capturingSender.Request.GetUrl());
        }

        [Test]
        public async Task TestSendingSingleFullyPopulatedLookup()
        {
            var serializer = new FakeSerializer(new byte[0]);
            var client = new Client(this.urlSender, serializer);
            const string expectedURL =
                "http://localhost/?search=1&country=2&max_results=3&include_only_administrative_area=4&include_only_locality=5&include_only_postal_code=6";
            var lookup = new Lookup
            {
                Search = "1",
                Country = "2",
                MaxResults = 3,
                AdministrativeArea = "4",
                Locality = "5",
                PostalCode = "6",
            };

            await client.SendAsync(lookup);

            Assert.AreEqual(expectedURL, this.capturingSender.Request.GetUrl());
        }

        #endregion

        #region [ Response Handling ]

        [Test]
        public async Task TestDeserializeCalledWithResponseBody()
        {
            var response = new Response(0, Encoding.ASCII.GetBytes("Hello, World!"));
            var mockSender = new MockSender(response);
            var sender = new URLPrefixSender("http://localhost/", mockSender);
            var deserializer = new FakeDeserializer(new Result());
            var client = new Client(sender, deserializer);

            await client.SendAsync(new Lookup("1"));

            Assert.AreEqual(response.Payload, deserializer.Payload);
        }

        [Test]
        public void TestRejectNullLookup()
        {
            var serializer = new FakeSerializer(null);
            var client = new Client(this.urlSender, serializer);

            Assert.Throws<ArgumentNullException>(() => client.SendAsync(null).GetAwaiter().GetResult());
        }

        [Test]
        public void TestRejectNullPrefix()
        {
            var serializer = new FakeSerializer(null);
            var client = new Client(this.urlSender, serializer);

            Assert.Throws<SmartyException>(() => client.SendAsync(new Lookup()).GetAwaiter().GetResult());
        }

        [Test]
        public void TestRejectEmptyPrefix()
        {
            var serializer = new FakeSerializer(null);
            var client = new Client(this.urlSender, serializer);

            Assert.Throws<SmartyException>(() => client.SendAsync(new Lookup("")).GetAwaiter().GetResult());
        }


        [Test]
        public async Task TestResultCorrectlyAssignedToLookup()
        {
            var lookup = new Lookup("1");
            var expectedResult = new Result();

            var mockSender = new MockSender(new Response(0, Encoding.ASCII.GetBytes("{[]}")));
            var sender = new URLPrefixSender("http://localhost/", mockSender);
            var deserializer = new FakeDeserializer(expectedResult);
            var client = new Client(sender, deserializer);

            await client.SendAsync(lookup);

            Assert.AreEqual(expectedResult.Candidates, lookup.Result);
        }

        #endregion
    }
}