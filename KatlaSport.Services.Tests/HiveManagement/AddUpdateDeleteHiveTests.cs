using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    /// <summary>
    /// Tests create, update and delete Hive
    /// </summary>
    public class AddUpdateDeleteHiveTests
    {
        public AddUpdateDeleteHiveTests()
        {
            Mapper.Reset();
            Mapper.Initialize(x =>
            {
                x.AddProfile<HiveManagementMappingProfile>();
            });
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
    }
}
