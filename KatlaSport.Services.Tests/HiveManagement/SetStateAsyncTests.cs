using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using KatlaSport.DataAccess;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    /// <summary>
    /// Tests class for test HiveService
    /// </summary>
    public class SetStateAsyncTests
    {
        private Mock<IProductStoreHiveContext> _context;

        private Mock<IUserContext> _userContext;

        private Mock<IEntitySet<StoreHive>> _mockSet;

        public SetStateAsyncTests()
        {
            _context = new Mock<IProductStoreHiveContext>();

            _userContext = new Mock<IUserContext>();

            _mockSet = new Mock<IEntitySet<StoreHive>>();
        }

        /// <summary>
        /// Set Status Hive IsDelete
        /// </summary>
        /// <param name="hiveId">hives id</param>
        /// <param name="hiveStatusBefor">status hive befor update</param>
        /// <param name="hiveStatusAfter">status hive after update</param>
        /// <returns>Task</returns>
        [Theory]
        [InlineData(1, false, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, false)]
        [InlineData(1, true, true)]
        public async Task SetStatusAsync_SetHiveStatus(int hiveId, bool hiveStatusBefor, bool hiveStatusAfter)
        {
            var storeHive = new StoreHive();

            storeHive.Id = hiveId;

            storeHive.IsDeleted = hiveStatusBefor;

            Assert.Equal(hiveStatusBefor, storeHive.IsDeleted);

            var storeHives = new[] { storeHive };

            _context.Setup(s => s.Hives).ReturnsEntitySet(storeHives);

            var service = new HiveService(_context.Object, _userContext.Object);

            await service.SetStatusAsync(hiveId, hiveStatusAfter);

            Assert.Equal(hiveStatusAfter, storeHive.IsDeleted);
        }

        /// <summary>
        /// Set Status HiveSection IsDelete
        /// </summary>
        /// <param name="sectionId">sections id</param>
        /// <param name="hiveSectionStatusBefor">status hive section befor update</param>
        /// <param name="hiveSectionStatusAfter">status hive section after update</param>
        /// <returns>Task</returns>
        [Theory]
        [InlineData(1, false, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, false)]
        [InlineData(1, true, true)]
        public async Task SetStatusAsync_SetHiveSectionStatus(int sectionId, bool hiveSectionStatusBefor, bool hiveSectionStatusAfter)
        {
            var storeHiveSection = new StoreHiveSection();

            storeHiveSection.Id = sectionId;

            storeHiveSection.IsDeleted = hiveSectionStatusBefor;

            Assert.Equal(hiveSectionStatusBefor, storeHiveSection.IsDeleted);

            var storeSectionHives = new[] { storeHiveSection };

            _context.Setup(s => s.Sections).ReturnsEntitySet(storeSectionHives);

            var service = new HiveSectionService(_context.Object, _userContext.Object);

            await service.SetStatusAsync(sectionId, hiveSectionStatusAfter);

            Assert.Equal(hiveSectionStatusAfter, storeHiveSection.IsDeleted);
        }

        /// <summary>
        /// Not found hive for set state
        /// </summary>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_Hive_RequestedResourceNotFoundException(IFixture fixture)
        {
            var storeHives = fixture.CreateMany<StoreHive>(0).ToArray();

            _context.Setup(s => s.Hives).ReturnsEntitySet(storeHives);

            var service = new HiveService(_context.Object, _userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(1, true));
        }

        /// <summary>
        /// Not found hiveSection for set state
        /// </summary>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_HiveSection_RequestedResourceNotFoundException(IFixture fixture)
        {
            var storeSectionHives = fixture.CreateMany<StoreHiveSection>(0).ToArray();

            _context.Setup(s => s.Sections).ReturnsEntitySet(storeSectionHives);

            var service = new HiveSectionService(_context.Object, _userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(1, true));
        }
    }
}