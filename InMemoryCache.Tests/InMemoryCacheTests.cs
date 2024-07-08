using Newtonsoft.Json.Linq;

namespace InMemoryCache.Tests
{
    public class Tests
    {
        public InMemoryCache<int, string> InMemoryCache;

        [SetUp]
        public void Setup()
        {
            InMemoryCache?.Dispose();
        }

        [Test]
        public void Initialize_WhenALreadyInitialize_ThrowInvalidOperationException()
        {
            //Arrange
            InMemoryCache<int, string>.Initialize(3);

            //Act - Assert
            Assert.Throws<InvalidOperationException>(() => InMemoryCache<int, string>.Initialize(6));
        }

        [Test]
        public void Add_NewItem_AddedSuccesfully()
        {
            //Arrange
            int capacity = 1;
            InMemoryCache<int, string>.Initialize(capacity);
            InMemoryCache = InMemoryCache<int,string>.Instance;

            int key = 1;
            string value = "Test";

            //Act
            InMemoryCache.Add(key, value);

            //Assert
            string itemAdded = InMemoryCache.Get(key);
            Assert.IsNotNull(itemAdded);
            Assert.AreEqual(itemAdded, value);
        }

        [Test]
        public void Add_NewItemInvalidKey_ThrowsArgumentNullException()
        {
            //Arrange
            int capacity = 1;
            InMemoryCache<string, string>.Initialize(capacity);
            InMemoryCache<string, string> InMemoryCache2 = InMemoryCache<string, string>.Instance;

            string key = null;
            string value = "Test";

            //Act
            Assert.Throws<ArgumentNullException>(() => InMemoryCache2.Add(key, value));
        }

        [Test]
        public void Add_ItemDuplicated_UpdateItemValue()
        {
            //Arrange
            int capacity = 2;
            InMemoryCache<int, string>.Initialize(capacity);
            InMemoryCache = InMemoryCache<int, string>.Instance;

            InMemoryCache.Add(1, "Test");
            InMemoryCache.Add(2, "Test2");

            //Act
            InMemoryCache.Add(1, "TestUpdated");

            //Assert
            string itemUpdated = InMemoryCache.Get(1);
            string item2 = InMemoryCache.Get(2);
            Assert.IsNotNull(itemUpdated);
            Assert.AreEqual(itemUpdated, "TestUpdated");
            Assert.IsNotNull(item2);
            Assert.AreEqual(item2, "Test2");
        }

        [Test]
        public void Add_ExceedingCapacity1_EvictLeastRecentlyUsed()
        {
            //Arrange
            int capacity = 1;
            InMemoryCache<int, string>.Initialize(capacity);
            InMemoryCache = InMemoryCache<int, string>.Instance;

            InMemoryCache.Add(1, "Test1");
           
            //Act
            InMemoryCache.Add(2, "Test2");

            //Assert
            string itemEvicted = InMemoryCache.Get(1);
            string item2 = InMemoryCache.Get(2);
           
            Assert.IsNull(itemEvicted);
            Assert.IsNotNull(item2);
            Assert.AreEqual(item2, "Test2");
          
        }

        [Test]
        public void Add_ExceedingCapacityMoreThan1_EvictLeastRecentlyUsed()
        {
            //Arrange
            int capacity = 3;
            InMemoryCache<int, string>.Initialize(capacity);
            InMemoryCache = InMemoryCache<int, string>.Instance;
            
            InMemoryCache.Add(1, "Test1");
            InMemoryCache.Add(2, "Test2");
            InMemoryCache.Add(3, "Test3");

            //Act
            InMemoryCache.Add(4, "Test4");

            //Assert
            string itemEvicted = InMemoryCache.Get(1);
            string item2 = InMemoryCache.Get(2);
            string item3 = InMemoryCache.Get(3);
            string item4 = InMemoryCache.Get(4);

            Assert.IsNull(itemEvicted);
            Assert.IsNotNull(item2);
            Assert.AreEqual(item2, "Test2");
            Assert.IsNotNull(item3);
            Assert.AreEqual(item3, "Test3");
            Assert.IsNotNull(item4);
            Assert.AreEqual(item4, "Test4");
        }

        [Test]
        public void Get_ExistingItem_ReturnValue()
        {
            // Arrange
            InMemoryCache<int, string>.Initialize(3);
            var cache = InMemoryCache<int, string>.Instance;
            cache.Add(1, "Test1");

            // Act
            var result = cache.Get(1);

            // Assert
            Assert.AreEqual("Test1", result);
        }

        [Test]
        public void Get_NonExistingItem_ReturnNull()
        {
            // Arrange
            InMemoryCache<int, string>.Initialize(3);
            var cache = InMemoryCache<int, string>.Instance;
            cache.Add(1, "Test1");

            // Act
            var result = cache.Get(2);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Initialize_WhenCacheCapacityIsZero_ThrowsException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => InMemoryCache<int, string>.Initialize(0));
        }
    }

   
}