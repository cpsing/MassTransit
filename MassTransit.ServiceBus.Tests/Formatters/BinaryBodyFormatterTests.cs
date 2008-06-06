 namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System.IO;
    using System.Text;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    public class BinaryBodyFormatterTests :
        Specification
    {
        private MockRepository mocks;
        private BinaryBodyFormatter formatter;
        private IFormattedBody mockBody;

        private readonly byte[] _serializedMessages = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 12, 2, 0, 0, 0, 83, 77, 97, 115, 115, 84, 114, 97, 110, 115, 105, 116, 46, 83, 101, 114, 118, 105, 99, 101, 66, 117, 115, 46, 84, 101, 115, 116, 115, 44, 32, 86, 101, 114, 115, 105, 111, 110, 61, 49, 46, 48, 46, 48, 46, 48, 44, 32, 67, 117, 108, 116, 117, 114, 101, 61, 110, 101, 117, 116, 114, 97, 108, 44, 32, 80, 117, 98, 108, 105, 99, 75, 101, 121, 84, 111, 107, 101, 110, 61, 110, 117, 108, 108, 5, 1, 0, 0, 0, 40, 77, 97, 115, 115, 84, 114, 97, 110, 115, 105, 116, 46, 83, 101, 114, 118, 105, 99, 101, 66, 117, 115, 46, 84, 101, 115, 116, 115, 46, 80, 105, 110, 103, 77, 101, 115, 115, 97, 103, 101, 0, 0, 0, 0, 2, 0, 0, 0, 11};
        private readonly byte[] _serializedMessagesWithValue = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 12, 2, 0, 0, 0, 83, 77, 97, 115, 115, 84, 114, 97, 110, 115, 105, 116, 46, 83, 101, 114, 118, 105, 99, 101, 66, 117, 115, 46, 84, 101, 115, 116, 115, 44, 32, 86, 101, 114, 115, 105, 111, 110, 61, 49, 46, 48, 46, 48, 46, 48, 44, 32, 67, 117, 108, 116, 117, 114, 101, 61, 110, 101, 117, 116, 114, 97, 108, 44, 32, 80, 117, 98, 108, 105, 99, 75, 101, 121, 84, 111, 107, 101, 110, 61, 110, 117, 108, 108, 5, 1, 0, 0, 0, 42, 77, 97, 115, 115, 84, 114, 97, 110, 115, 105, 116, 46, 83, 101, 114, 118, 105, 99, 101, 66, 117, 115, 46, 84, 101, 115, 116, 115, 46, 67, 108, 105, 101, 110, 116, 77, 101, 115, 115, 97, 103, 101, 1, 0, 0, 0, 5, 95, 110, 97, 109, 101, 1, 2, 0, 0, 0, 6, 3, 0, 0, 0, 4, 116, 101, 115, 116, 11};

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            formatter = new BinaryBodyFormatter();
            mockBody = mocks.CreateMock<IFormattedBody>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            formatter = null;
            mockBody = null;
        }

        [Test]
        public void Serialize()
        {
            PingMessage msg = new PingMessage();
            MemoryStream ms = new MemoryStream();

            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);
            }
            
            Assert.That(ms.Length, Is.EqualTo(161));
            Assert.AreEqual(_serializedMessages, Convert(ms));
        }

        [Test]
        public void Deserialize()
        {
            MemoryStream ms = new MemoryStream(_serializedMessages);
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (mocks.Playback())
            {
                PingMessage msg = formatter.Deserialize<PingMessage>(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof(PingMessage)));
            }
        }

        [Test]
        public void DeserializeWithGenerics()
        {
            MemoryStream ms = new MemoryStream(_serializedMessages);
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (mocks.Playback())
            {
                PingMessage msg = formatter.Deserialize<PingMessage>(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof(PingMessage)));
            }
        }

        [Test]
        public void SerializeObjectWithValues()
        {

            ClientMessage msg = new ClientMessage();
            msg.Name = "test";

            MemoryStream ms = new MemoryStream();
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);
            }
            
            Assert.AreEqual(_serializedMessagesWithValue.Length, ms.Length);
            Assert.AreEqual(_serializedMessagesWithValue, Convert(ms));
            

        }

        [Test]
        public void DeserializeObjectWithValues()
        {
            MemoryStream ms = new MemoryStream(_serializedMessagesWithValue);

            object actual;
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }

            using (mocks.Playback())
            {
                actual = formatter.Deserialize(mockBody);
            }

            Assert.IsInstanceOfType(typeof(ClientMessage), actual);
            

        }

        [Test]
        public void DeserializeObjectWithValuesWithGenerics()
        {
            MemoryStream ms = new MemoryStream(_serializedMessagesWithValue);

            ClientMessage actual;
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }

            using (mocks.Playback())
            {
                actual = formatter.Deserialize<ClientMessage>(mockBody);
            }

            Assert.IsInstanceOfType(typeof(ClientMessage), actual);
            Assert.AreEqual("test", actual.Name);

        }

        private byte[] Convert(MemoryStream str)
        {
            byte[] buffer = new byte[str.Length];
            str.Position = 0;
            str.Read(buffer, 0, buffer.Length);
            return buffer;
        }
        private void BytesToString(MemoryStream str)
        {
            byte[] buffer = Convert(str);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
            {
                sb.AppendFormat("{0},", b);
            }

            string s = sb.ToString();
        }
    }
}