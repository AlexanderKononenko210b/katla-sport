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
    public class HiveSectionServiceTests
    {
        public HiveSectionServiceTests()
        {
            var mapper = AutoMapperInitialize.Instance;
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

        /// <summary>
        /// Get all sections from database
        /// </summary>
        /// <param name="context">Mock{IProductStoreHiveContext}</param>
        /// <param name="service">Hive section service</param>
        /// <param name="fixture">instance type IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionsAsync_WithoutParams_ListHiveSectionListItems(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            // arrange
            var dbSections = fixture.CreateMany<StoreHiveSection>(5).OrderBy(s => s.Id).ToList();

            context.Setup(s => s.Sections).ReturnsEntitySet(dbSections);

            // act
            var hiveSectionListItems = await service.GetHiveSectionsAsync();

            // assert
            Assert.Collection(
                hiveSectionListItems,
                item => Assert.Equal(dbSections[0].Id, item.Id),
                item => Assert.Equal(dbSections[1].Id, item.Id),
                item => Assert.Equal(dbSections[2].Id, item.Id),
                item => Assert.Equal(dbSections[3].Id, item.Id),
                item => Assert.Equal(dbSections[4].Id, item.Id));
        }

        /// <summary>
        /// Get all sections from database by hiveId
        /// </summary>
        /// <param name="context">Mock{IProductStoreHiveContext}</param>
        /// <param name="service">Hive section service</param>
        /// <param name="fixture">instance type IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionsAsync_HiveId_ListHiveSectionsListItems(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            // arrange
            var dbSections = fixture.CreateMany<StoreHiveSection>(5).ToList();

            var hiveId = dbSections[0].StoreHiveId;

            dbSections[3].StoreHiveId = hiveId;
            dbSections[4].StoreHiveId = hiveId;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbSections);

            // act
            var hiveSectionsListItems = await service.GetHiveSectionsAsync(hiveId);

            // assert
            Assert.Equal(3, hiveSectionsListItems.Count);

            Assert.Collection(
                hiveSectionsListItems,
                item => Assert.Equal(dbSections[0].Id, item.Id),
                item => Assert.Equal(dbSections[3].Id, item.Id),
                item => Assert.Equal(dbSections[4].Id, item.Id));
        }

        /// <summary>
        /// Get section from database by sectionId
        /// </summary>
        /// <param name="context">Mock{IProductStoreHiveContext}</param>
        /// <param name="service">Hive section service</param>
        /// <param name="fixture">instance type IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionAsync_SectionId_Hive(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            // arrange
            var dbSections = fixture.CreateMany<StoreHiveSection>(5).ToList();

            var sectionId = dbSections[3].Id;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbSections);

            // act
            var section = await service.GetHiveSectionAsync(sectionId);

            // assert
            Assert.Equal(dbSections[3].Id, section.Id);
        }

        /// <summary>
        /// Get section from database by sectionId id expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">Mock{IProductStoreHiveContext}</param>
        /// <param name="service">Hive section service</param>
        /// <param name="fixture">instance type IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionAsync_SectionId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductStoreHiveContext> context,
            HiveSectionService service,
            IFixture fixture)
        {
            // arrange
            var dbSections = fixture.CreateMany<StoreHiveSection>(2).ToList();

            var sectionId = dbSections[0].Id + dbSections[1].Id;

            context.Setup(s => s.Sections).ReturnsEntitySet(dbSections);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveSectionAsync(sectionId));
        }
    }
}
