using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using KatlaSport.DataAccess.ProductCatalogue;
using KatlaSport.Services.ProductManagement;
using Moq;
using Xunit;
using DbProductCategory = KatlaSport.DataAccess.ProductCatalogue.ProductCategory;

namespace KatlaSport.Services.Tests.ProductManagement
{
    /// <summary>
    /// Tests methods ProductCategoryService
    /// </summary>
    public class ProductCategoryServiceTests
    {
        public ProductCategoryServiceTests()
        {
            var mapper = AutoMapperInitialize.Instance;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetCategoriesAsync_IntStartAndAmount_ListProductCategoryListItems(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbCategories = fixture.CreateMany<DbProductCategory>(2).OrderBy(s => s.Id).ToList();

            var dbProducts = fixture.CreateMany<CatalogueProduct>(5).ToList();

            dbProducts[0].CategoryId = dbCategories[0].Id;
            dbProducts[1].CategoryId = dbCategories[0].Id;
            dbProducts[2].CategoryId = dbCategories[1].Id;
            dbProducts[3].CategoryId = dbCategories[1].Id;
            dbProducts[4].CategoryId = dbCategories[1].Id;

            int start = 0, amount = 2;

            var categories = context.Setup(s => s.Categories).ReturnsEntitySet(dbCategories);

            var products = context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // act
            var productCategoryListItems = await service.GetCategoriesAsync(start, amount);

            // assert
            Assert.Equal(2, productCategoryListItems.Count);

            Assert.Collection(
                productCategoryListItems,
                item => Assert.Equal(2, productCategoryListItems[0].ProductCount),
                item => Assert.Equal(3, productCategoryListItems[1].ProductCount));
        }

        /// <summary>
        /// Get categorie by categoryId
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetCategoryAsync_CategoryId_ProductCategoryInstance(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbCategories = fixture.CreateMany<DbProductCategory>(5).ToList();

            var categoryId = dbCategories[3].Id;

            var categories = context.Setup(s => s.Categories).ReturnsEntitySet(dbCategories);

            // act
            var category = await service.GetCategoryAsync(categoryId);

            // assert
            Assert.NotNull(category);

            Assert.Equal(categoryId, category.Id);
        }

        /// <summary>
        /// Get categorie by categoryId expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetCategoryAsync_CategoryId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbCategories = fixture.CreateMany<DbProductCategory>(3).ToList();

            var categoryId = dbCategories[0].Id + dbCategories[1].Id + dbCategories[2].Id;

            var categories = context.Setup(s => s.Categories).ReturnsEntitySet(dbCategories);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryAsync(categoryId));
        }

        /// <summary>
        /// Create new categorie
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateCategoryAsync_UpdateProductCategoryRequest_ProductCategoryInstance(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbSections = fixture.CreateMany<DbProductCategory>(1).ToList();

            context.Setup(s => s.Categories).ReturnsEntitySet(dbSections);

            var updateProductCategoryRequest = fixture.Create<UpdateProductCategoryRequest>();

            updateProductCategoryRequest.Code = $"{dbSections[0].Code}1";

            // act
            var newSection = await service.CreateCategoryAsync(updateProductCategoryRequest);

            // Assert
            Assert.Equal(updateProductCategoryRequest.Name, newSection.Name);
            Assert.Equal(updateProductCategoryRequest.Code, newSection.Code);
        }

        /// <summary>
        /// Create new categorie expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateCategoryAsync_UpdateProductCategoryRequest_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbSections = fixture.CreateMany<DbProductCategory>(1).ToList();

            var updateProductCategoryRequest = fixture.Create<UpdateProductCategoryRequest>();

            updateProductCategoryRequest.Code = dbSections[0].Code;

            context.Setup(s => s.Categories).ReturnsEntitySet(dbSections);

            // assert
            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.CreateCategoryAsync(updateProductCategoryRequest));
        }

        /// <summary>
        /// Update categorie
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateCategoryAsync_CategoryIdAndUpdateProductCategoryRequest_ProductCategoryInstance(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // Arrange
            var categoryId = 1;

            var dbUpdateRequest = fixture.Create<UpdateProductCategoryRequest>();

            var dbSections = fixture.CreateMany<DbProductCategory>(1).ToList();

            dbSections[0].Id = categoryId;

            dbSections[0].Code = $"{dbUpdateRequest.Code} + 1";

            context.Setup(s => s.Categories).ReturnsEntitySet(dbSections);

            // Act
            var updateCategory = await service.UpdateCategoryAsync(categoryId, dbUpdateRequest);

            // Assert
            Assert.Equal(dbUpdateRequest.Name, updateCategory.Name);
            Assert.Equal(dbUpdateRequest.Code, updateCategory.Code);
        }

        /// <summary>
        /// Update categorie expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task
            UpdateCategoryAsync_CategoryIdAndUpdateProductCategoryRequest_RequestedResourceHasConflictException(
                [Frozen] Mock<IProductCatalogueContext> context,
                ProductCategoryService service,
                IFixture fixture)
        {
            // Arrange
            var categoryId = 1;

            var dbUpdateRequest = fixture.Create<UpdateProductCategoryRequest>();

            var dbSections = fixture.CreateMany<DbProductCategory>(1).ToList();

            dbSections[0].Id = categoryId + 1;

            dbSections[0].Code = dbUpdateRequest.Code;

            context.Setup(s => s.Categories).ReturnsEntitySet(dbSections);

            // Assert
            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.UpdateCategoryAsync(categoryId, dbUpdateRequest));
        }

        /// <summary>
        /// Update categorie expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateCategoryAsync_CategoryIdAndUpdateProductCategoryRequest_RequestedResourceNotFoundException(
                [Frozen] Mock<IProductCatalogueContext> context,
                ProductCategoryService service,
                IFixture fixture)
        {
            // Arrange
            var categoryId = 1;

            var dbUpdateRequest = fixture.Create<UpdateProductCategoryRequest>();

            var dbSections = fixture.CreateMany<DbProductCategory>(1).ToList();

            dbSections[0].Id = categoryId + 1;

            dbSections[0].Code = $"{dbUpdateRequest.Code} + 1";

            context.Setup(s => s.Categories).ReturnsEntitySet(dbSections);

            // Assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.UpdateCategoryAsync(categoryId, dbUpdateRequest));
        }

        /// <summary>
        /// Delete categorie
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteCategoryAsync_CategoryId_Success(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbCategory = fixture.CreateMany<DbProductCategory>(1).ToList();

            var categoryId = 1;

            dbCategory[0].Id = categoryId;

            dbCategory[0].IsDeleted = true;

            context.Setup(s => s.Categories).ReturnsEntitySet(dbCategory);

            // act
            await service.DeleteCategoryAsync(categoryId);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryAsync(categoryId));
        }

        /// <summary>
        /// Delete categorie expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteCategoryAsync_CategoryId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbCategory = fixture.CreateMany<DbProductCategory>(1).ToList();

            var categoryId = 1;

            dbCategory[0].Id = categoryId + 1;

            dbCategory[0].IsDeleted = true;

            context.Setup(s => s.Categories).ReturnsEntitySet(dbCategory);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.DeleteCategoryAsync(categoryId));
        }

        /// <summary>
        /// Delete categorie expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product category</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteCategoryAsync_CategoryId_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCategoryService service,
            IFixture fixture)
        {
            // arrange
            var dbCategory = fixture.CreateMany<DbProductCategory>(1).ToList();

            var categoryId = 1;

            dbCategory[0].Id = categoryId;

            dbCategory[0].IsDeleted = false;

            context.Setup(s => s.Categories).ReturnsEntitySet(dbCategory);

            // assert
            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(
                () => service.DeleteCategoryAsync(categoryId));
        }

        /// <summary>
        /// Set Status Category IsDelete
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="categoryStatusBefor">status hive section befor update</param>
        /// <param name="categoryStatusAfter">status hive section after update</param>
        /// <returns>Task</returns>
        [Theory]
        [InlineData(1, false, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, false)]
        [InlineData(1, true, true)]
        public async Task SetStatusAsync_SetHiveSectionStatus(int categoryId, bool categoryStatusBefor, bool categoryStatusAfter)
        {
            // arrange
            var dbProductCategory = new DbProductCategory();

            dbProductCategory.Id = categoryId;

            dbProductCategory.IsDeleted = categoryStatusBefor;

            Assert.Equal(categoryStatusBefor, dbProductCategory.IsDeleted);

            var dbProductCategores = new[] { dbProductCategory };

            var context = new Mock<IProductCatalogueContext>();

            context.Setup(s => s.Categories).ReturnsEntitySet(dbProductCategores);

            var userContext = new Mock<IUserContext>();

            userContext.Setup(s => s.UserId).Returns(1);

            var service = new ProductCategoryService(context.Object, userContext.Object);

            // act
            await service.SetStatusAsync(categoryId, categoryStatusAfter);

            // assert
            Assert.Equal(categoryStatusAfter, dbProductCategory.IsDeleted);
        }
    }
}
