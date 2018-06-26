using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    /// <summary>
    /// Tests create, update and delete Hive
    /// </summary>
    public class HiveServiceTests
    {
        public HiveServiceTests()
        {
            var mapper = AutoMapperInitialize.Instance;
        }

        /// <summary>
        /// Test create new Hive
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateHiveAsync_UpdateHiveRequest_Success(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var dbHives = fixture.CreateMany<StoreHive>(0).ToList();

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            var dbUpdateHiveRequest = fixture.Create<UpdateHiveRequest>();

            var createdHive = await service.CreateHiveAsync(dbUpdateHiveRequest);

            Assert.Equal(dbUpdateHiveRequest.Name, createdHive.Name);
            Assert.Equal(dbUpdateHiveRequest.Address, createdHive.Address);
            Assert.Equal(dbUpdateHiveRequest.Code, createdHive.Code);
        }

        /// <summary>
        /// Create new Hive expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateHiveAsync_UpdateHiveRequest_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var dbUpdateHiveRequest = fixture.Create<UpdateHiveRequest>();

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            dbHives[0].Code = dbUpdateHiveRequest.Code;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.CreateHiveAsync(dbUpdateHiveRequest));
        }

        /// <summary>
        /// Test update Hive
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveAsync_HiveId_UpdateHiveRequest_Success(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var hiveId = 1;

            var dbUpdateHiveRequest = fixture.Create<UpdateHiveRequest>();

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            dbHives[0].Id = hiveId;

            dbHives[0].Code = $"{dbUpdateHiveRequest.Code} + 1";

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            var updateHive = await service.UpdateHiveAsync(hiveId, dbUpdateHiveRequest);

            Assert.Equal(dbUpdateHiveRequest.Name, updateHive.Name);
            Assert.Equal(dbUpdateHiveRequest.Address, updateHive.Address);
            Assert.Equal(dbUpdateHiveRequest.Code, updateHive.Code);
        }

        /// <summary>
        /// Test update Hive expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveAsync_HiveId_UpdateHiveRequest_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var hiveId = 1;

            var dbUpdateHiveRequest = fixture.Create<UpdateHiveRequest>();

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            dbHives[0].Id = hiveId + 1;

            dbHives[0].Code = dbUpdateHiveRequest.Code;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.UpdateHiveAsync(hiveId, dbUpdateHiveRequest));
        }

        /// <summary>
        /// Test update Hive expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveAsync_HiveId_UpdateHiveRequest_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var hiveId = 1;

            var dbUpdateHiveRequest = fixture.Create<UpdateHiveRequest>();

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            dbHives[0].Id = hiveId + 1;

            dbHives[0].Code = $"{dbUpdateHiveRequest.Code}1";

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.UpdateHiveAsync(hiveId, dbUpdateHiveRequest));
        }

        /// <summary>
        /// Test delete Hive
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveAsync_HiveId_Success(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var hiveId = 1;

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            dbHives[0].Id = hiveId;

            dbHives[0].IsDeleted = true;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            await service.DeleteHiveAsync(hiveId);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.GetHiveAsync(hiveId));
        }

        /// <summary>
        /// Test delete Hive expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveAsync_HiveId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var hiveId = 1;

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            dbHives[0].Id = hiveId + 1;

            dbHives[0].IsDeleted = true;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.DeleteHiveAsync(hiveId));
        }

        /// <summary>
        /// Test delete Hive expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveAsync_HiveId_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            var hiveId = 1;

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            dbHives[0].Id = hiveId;

            dbHives[0].IsDeleted = false;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.DeleteHiveAsync(hiveId));
        }

        /// <summary>
        /// Get all hives from database
        /// </summary>
        /// <param name="context">Mock{IProductStoreHiveContext}</param>
        /// <param name="service">Hive service</param>
        /// <param name="fixture">instance type IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetHivesAsync_WithoutParameters_HiveListItems(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            // arrange

            var dbHives = fixture.CreateMany<StoreHive>(2).ToList();

            var dbSections = fixture.CreateMany<StoreHiveSection>(5).ToList();

            dbSections[0].StoreHiveId = dbHives[0].Id;
            dbSections[1].StoreHiveId = dbHives[0].Id;
            dbSections[2].StoreHiveId = dbHives[1].Id;
            dbSections[3].StoreHiveId = dbHives[1].Id;
            dbSections[4].StoreHiveId = dbHives[1].Id;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            context.Setup(s => s.Sections).ReturnsEntitySet(dbSections);

            // act

            var hiveListItems = await service.GetHivesAsync();

            // assert

            Assert.Equal(2, hiveListItems.Count);

            Assert.Collection(
                hiveListItems,
                item => Assert.Equal(2, item.HiveSectionCount),
                item => Assert.Equal(3, item.HiveSectionCount));
        }

        /// <summary>
        /// Get hive from database by hives id
        /// </summary>
        /// <param name="context">Mock{IProductStoreHiveContext}</param>
        /// <param name="service">Hive service</param>
        /// <param name="fixture">instance type IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetHiveAsync_HiveId_Hive(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            // arrange
            var dbHives = fixture.CreateMany<StoreHive>(2).ToList();

            var hiveId = dbHives[1].Id;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            // act
            var hive = await service.GetHiveAsync(hiveId);

            // assert
            Assert.Equal(hiveId, hive.Id);
        }

        /// <summary>
        /// Get hive from database by hives id expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">Mock{IProductStoreHiveContext}</param>
        /// <param name="service">Hive service</param>
        /// <param name="fixture">instance type IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetHiveAsync_HiveId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveService service,
            IFixture fixture)
        {
            // arrange
            var dbHives = fixture.CreateMany<StoreHive>(2).ToList();

            var hiveId = dbHives[0].Id + dbHives[1].Id;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveAsync(hiveId));
        }
    }
}
