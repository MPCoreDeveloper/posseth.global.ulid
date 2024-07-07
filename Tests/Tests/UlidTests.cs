using Posseth.UlidFactory;
namespace Posseth.Global.UlidFactory.Tests
{
    
        [TestClass]
        public class UlidTests
        {
        [TestMethod]
        public void NewUlid_ShouldReturnValidUlid()
            {
                // Arrange

                // Act
                Ulid ulid = Ulid.NewUlid();

                // Assert
                Assert.IsNotNull(ulid);
                Assert.IsTrue(ulid.HasValue());
            }

        [TestMethod]
        public void NewUlid_WithTimestamp_ShouldReturnValidUlid()
            {
                // Arrange
                var timestamp = new System.DateTimeOffset(2024, 1, 1, 0, 0, 0, System.TimeSpan.Zero);

                // Act
                Ulid ulid = Ulid.NewUlid(timestamp);

                // Assert
                Assert.IsNotNull(ulid);
                Assert.IsTrue(ulid.HasValue());
            }

        [TestMethod]
        public void NewUlid_WithDateTime_ShouldReturnValidUlid()
            {
                // Arrange
                var dateTime = new System.DateTime(2024, 1, 1, 0, 0, 0);

                // Act
                Ulid ulid = Ulid.NewUlid(dateTime);

                // Assert
                Assert.IsNotNull(ulid);
                Assert.IsTrue(ulid.HasValue());
            }

        [TestMethod]
        public void NewUlid_WithTimestampLong_ShouldReturnValidUlid()
            {
            // Arrange
                                 
                long timestamp = 1704067200000; // January 1, 2024 00:00:00 UTC

                // Act
                Ulid ulid = Ulid.NewUlid(timestamp);

                // Assert
                Assert.IsNotNull(ulid);
                Assert.IsTrue(ulid.HasValue());
            }

        [TestMethod]
        public void GetTimestampFromUlid_ShouldReturnCorrectTimestamp()
            {
                // Arrange
                var ulid =  Ulid.Parse("066C4MFM00EHYBTA0C9CEEBJMW");

                // Act
                var timestamp = Ulid.GetTimestampFromUlid(ulid);

                // Assert
                Assert.AreEqual(new System.DateTime(2024, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), timestamp);
            }

        [TestMethod]
        public void TryParse_ValidUlidString_ShouldReturnTrueAndValidUlid()
            {
                // Arrange
                var ulidString = "01F9Z3N2F2VX4XG00000000000";

                // Act
                bool result = Ulid.TryParse(ulidString, out Ulid? ulid);

                // Assert
                Assert.IsTrue(result);
                Assert.IsNotNull(ulid);
                Assert.IsTrue(ulid.HasValue());
            }

        [TestMethod]
        public void TryParse_InvalidUlidString_ShouldReturnFalseAndNullUlid()
            {
                // Arrange
                var ulidString = "invalidulid";

                // Act
                bool result = Ulid.TryParse(ulidString, out Ulid? ulid);

                // Assert
                Assert.IsFalse(result);
                Assert.IsNull(ulid);
            }
        }
    }


