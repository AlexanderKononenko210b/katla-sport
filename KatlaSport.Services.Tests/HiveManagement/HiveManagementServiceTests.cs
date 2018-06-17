using System.Threading.Tasks;
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
    public class HiveManagementServiceTests
    {
        private Mock<IProductStoreHiveContext> _context;

        private Mock<IUserContext> _userContext;

        private Mock<IEntitySet<StoreHive>> _mockSet;

        public HiveManagementServiceTests()
        {
            _context = new Mock<IProductStoreHiveContext>();

            _userContext = new Mock<IUserContext>();

            _mockSet = new Mock<IEntitySet<StoreHive>>();
        }

        /// <summary>
        /// Set Status Hive IsDelete
        /// </summary>
        /// <param name="userId">users id</param>
        /// <param name="hiveStatusBefor">status hive befor update</param>
        /// <param name="hiveStatusAfter">status hive after update</param>
        /// <returns>Task</returns>
        [Theory]
        [InlineData(1, false, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, false)]
        [InlineData(1, true, true)]
        public async Task SetStatusAsync_SetHiveStatus(int userId, bool hiveStatusBefor, bool hiveStatusAfter)
        {
            var storeHive = new StoreHive();

            storeHive.Id = userId;

            storeHive.IsDeleted = hiveStatusBefor;

            Assert.Equal(hiveStatusBefor, storeHive.IsDeleted);

            var storeHives = new[] { storeHive };

            _context.Setup(s => s.Hives).ReturnsEntitySet(storeHives);

            var service = new HiveService(_context.Object, _userContext.Object);

            await service.SetStatusAsync(userId, hiveStatusAfter);

            Assert.Equal(hiveStatusAfter, storeHive.IsDeleted);
        }

        /// <summary>
        /// Set Status HiveSection IsDelete
        /// </summary>
        /// <param name="userId">users id</param>
        /// <param name="hiveSectionStatusBefor">status hive section befor update</param>
        /// <param name="hiveSectionStatusAfter">status hive section after update</param>
        /// <returns>Task</returns>
        [Theory]
        [InlineData(1, false, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, false)]
        [InlineData(1, true, true)]
        public async Task SetStatusAsync_SetHiveSectionStatus(int userId, bool hiveSectionStatusBefor, bool hiveSectionStatusAfter)
        {
            var storeHiveSection = new StoreHiveSection();

            storeHiveSection.Id = userId;

            storeHiveSection.IsDeleted = hiveSectionStatusBefor;

            Assert.Equal(hiveSectionStatusBefor, storeHiveSection.IsDeleted);

            var storeSectionHives = new[] { storeHiveSection };

            _context.Setup(s => s.Sections).ReturnsEntitySet(storeSectionHives);

            var service = new HiveSectionService(_context.Object, _userContext.Object);

            await service.SetStatusAsync(userId, hiveSectionStatusAfter);

            Assert.Equal(hiveSectionStatusAfter, storeHiveSection.IsDeleted);
        }
    }
}