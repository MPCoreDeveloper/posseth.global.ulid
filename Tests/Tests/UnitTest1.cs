using Posseth.Global.UlidFactory;

namespace Posseth.Global.UlidFactory.Tests
{
    [TestClass]
    public class UnitTestUlid
    {
        [TestMethod]
        public void ReturnsNewUlid()
        {
            string ulid = Ulid.NewUlid();
            Assert.AreEqual(26, ulid.Length);
        }

        [TestMethod]
        public void ReturnsTimestampFromUlid()
        {
            string ulid = Ulid.NewUlid();
            DateTime timestamp = Ulid.GetTimestampFromUlid(ulid);
            Assert.AreEqual(DateTimeKind.Utc, timestamp.Kind);
        }

    }
}