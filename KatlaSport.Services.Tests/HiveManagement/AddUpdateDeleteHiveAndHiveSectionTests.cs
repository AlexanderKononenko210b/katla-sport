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
    public class AddUpdateDeleteHiveAndHiveSectionTests
    {
        public AddUpdateDeleteHiveAndHiveSectionTests()
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

        /// <summary>
        /// Test create new Hive Section
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateHiveSectionAsync_UpdateRequest_Success(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(0).ToList();

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            var dbHives = fixture.CreateMany<StoreHive>(1).ToList();

            var hiveId = 1;

            dbHives[0].Id = hiveId;

            context.Setup(s => s.Hives).ReturnsEntitySet(dbHives);

            var dbUpdateRequest = fixture.Create<UpdateHiveSectionRequest>();

            var createdHiveSection = await service.CreateHiveSectionAsync(hiveId, dbUpdateRequest);

            Assert.Equal(dbUpdateRequest.Name, createdHiveSection.Name);
            Assert.Equal(dbUpdateRequest.Code, createdHiveSection.Code);
        }

        /// <summary>
        /// Create new hive section expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive Section</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateHiveSectionAsync_HiveIdAndUpdateRequest_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var dbUpdateRequest = fixture.Create<UpdateHiveSectionRequest>();

            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(1).ToList();

            dbHiveSections[0].Code = dbUpdateRequest.Code;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            var hiveId = 1;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.CreateHiveSectionAsync(hiveId, dbUpdateRequest));
        }

        /// <summary>
        /// Test update Hive Section
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive Section</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveSectionAsync_HiveSectionId_UpdateHiveSectionRequest_Success(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var hiveId = 1;

            var dbUpdateRequest = fixture.Create<UpdateHiveSectionRequest>();

            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(1).ToList();

            dbHiveSections[0].Id = hiveId;

            dbHiveSections[0].Code = $"{dbUpdateRequest.Code} + 1";

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            var updateHiveSection = await service.UpdateHiveSectionAsync(hiveId, dbUpdateRequest);

            Assert.Equal(dbUpdateRequest.Name, updateHiveSection.Name);
            Assert.Equal(dbUpdateRequest.Code, updateHiveSection.Code);
        }

        /// <summary>
        /// Test update Hive Section expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveSectionAsync_HiveSectionId_UpdateRequest_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var hiveSectionId = 1;

            var dbUpdateRequest = fixture.Create<UpdateHiveSectionRequest>();

            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(1).ToList();

            dbHiveSections[0].Id = hiveSectionId + 1;

            dbHiveSections[0].Code = dbUpdateRequest.Code;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.UpdateHiveSectionAsync(hiveSectionId, dbUpdateRequest));
        }

        /// <summary>
        /// Test update Hive Section expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveSectionAsync_HiveId_UpdateHiveRequest_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var hiveSectionId = 1;

            var dbUpdateRequest = fixture.Create<UpdateHiveSectionRequest>();

            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(1).ToList();

            dbHiveSections[0].Id = hiveSectionId + 1;

            dbHiveSections[0].Code = $"{dbUpdateRequest.Code}1";

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.UpdateHiveSectionAsync(hiveSectionId, dbUpdateRequest));
        }

        /// <summary>
        /// Test delete Hive Section
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveSectionAsync_HiveSectionId_Success(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var hiveSectionId = 1;

            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(1).ToList();

            dbHiveSections[0].Id = hiveSectionId;

            dbHiveSections[0].IsDeleted = true;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            await service.DeleteHiveSectionAsync(hiveSectionId);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.GetHiveSectionAsync(hiveSectionId));
        }

        /// <summary>
        /// Test delete Hive Section expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive Section</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveSectionAsync_HiveSectionId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var hiveSectionId = 1;

            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(1).ToList();

            dbHiveSections[0].Id = hiveSectionId + 1;

            dbHiveSections[0].IsDeleted = true;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.DeleteHiveSectionAsync(hiveSectionId));
        }

        /// <summary>
        /// Test delete Hive Section expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductStoreHiveContext</param>
        /// <param name="service">service for work with Hive Section</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveSectionAsync_HiveSectionId_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            var hiveSectionId = 1;

            var dbHiveSections = fixture.CreateMany<StoreHiveSection>(1).ToList();

            dbHiveSections[0].Id = hiveSectionId;

            dbHiveSections[0].IsDeleted = false;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbHiveSections);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.DeleteHiveSectionAsync(hiveSectionId));
        }
    }
}
