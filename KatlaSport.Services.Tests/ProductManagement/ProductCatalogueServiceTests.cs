using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using KatlaSport.DataAccess.ProductCatalogue;
using KatlaSport.Services.ProductManagement;
using Moq;
using Xunit;
using DbProduct = KatlaSport.DataAccess.ProductCatalogue.CatalogueProduct;
using DbProductCategory = KatlaSport.DataAccess.ProductCatalogue.ProductCategory;

namespace KatlaSport.Services.Tests.ProductManagement
{
    /// <summary>
    /// Test methods ProductCatalogueService
    /// </summary>
    public class ProductCatalogueServiceTests
    {
        public ProductCatalogueServiceTests()
        {
            var mapper = AutoMapperInitialize.Instance;
        }

        /// <summary>
        /// Test method GetProductsAsync return list ProductListItems
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetProductsAsync_IntStartAndAmount_ListProductListItems(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var dbProducts = fixture.CreateMany<DbProduct>(6).OrderBy(s => s.Id).ToList();

            int start = 0, amount = 6;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // act
            var productListItems = await service.GetProductsAsync(start, amount);

            // assert
            Assert.Equal(6, productListItems.Count);
        }

        /// <summary>
        /// Test method GetCategoryProductsAsync return list ProductCategoryProductListItem
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetCategoryProductsAsync_ProductCategoryId_ListProductCategoryProductListItems(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var dbCategorys = fixture.CreateMany<DbProductCategory>(1).ToList();

            var productCategoryId = dbCategorys[0].Id;

            var dbProducts = fixture.CreateMany<DbProduct>(5).OrderBy(s => s.Id).ToList();

            dbProducts[1].CategoryId = productCategoryId;
            dbProducts[3].CategoryId = productCategoryId;
            dbProducts[4].CategoryId = productCategoryId;

            context.Setup(s => s.Categories).ReturnsEntitySet(dbCategorys);

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // act
            var productCategoryProductListItems = await service.GetCategoryProductsAsync(productCategoryId);

            // assert
            Assert.Equal(3, productCategoryProductListItems.Count);
        }

        /// <summary>
        /// Test method GetProductsAsync expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetCategoryProductsAsync_ProductCategoryId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var dbCategorys = fixture.CreateMany<DbProductCategory>(1).ToList();

            var productCategoryId = dbCategorys[0].Id + 1;

            context.Setup(s => s.Categories).ReturnsEntitySet(dbCategorys);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryProductsAsync(productCategoryId));
        }

        /// <summary>
        /// Test method GetProductAsync return instance type Product
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetProductAsync_ProductId_Product(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var dbProducts = fixture.CreateMany<DbProduct>(5).ToList();

            var productId = dbProducts[2].Id;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // act
            var product = await service.GetProductAsync(productId);

            // assert
            Assert.Equal(dbProducts[2].Code, product.Code);
        }

        /// <summary>
        /// Test method GetProductAsync expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task GetProductAsync_ProductId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var dbProducts = fixture.CreateMany<DbProduct>(3).ToList();

            var productId = dbProducts[0].Id + dbProducts[1].Id + dbProducts[2].Id;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetProductAsync(productId));
        }

        /// <summary>
        /// Test method CreateProductAsync with valid data
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateProductAsync_UpdateProductRequest_Product(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var dbProducts = fixture.CreateMany<DbProduct>(3).ToList();

            var updateRequest = fixture.Create<UpdateProductRequest>();

            updateRequest.Code = dbProducts[1].Code;

            dbProducts[1].Code = dbProducts[0].Code;
            dbProducts[2].Code = dbProducts[0].Code;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // act
            var product = await service.CreateProductAsync(updateRequest);

            // assert
            Assert.Equal(updateRequest.Code, product.Code);
        }

        /// <summary>
        /// Test method CreateProductAsync expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task CreateProductAsync_UpdateProductRequest_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var dbProducts = fixture.CreateMany<DbProduct>(3).ToList();

            var updateRequest = fixture.Create<UpdateProductRequest>();

            updateRequest.Code = dbProducts[1].Code;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // assert
            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateProductAsync(updateRequest));
        }

        /// <summary>
        /// Test method UpdateProductAsync return Products instance after update
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateProductAsync_UpdateProductRequest_Product(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var updateRequest = fixture.Create<UpdateProductRequest>();

            var dbProducts = fixture.CreateMany<DbProduct>(2).ToList();

            var productId = 1;

            dbProducts[0].Id = productId + 1;
            dbProducts[0].Code = $"{updateRequest.Code}1";
            dbProducts[1].Id = productId;
            dbProducts[1].Code = updateRequest.Code;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // act
            var updateProduct = await service.UpdateProductAsync(productId, updateRequest);

            // assert
            Assert.Equal(updateRequest.Code, updateProduct.Code);
            Assert.Equal(updateRequest.Name, updateProduct.Name);
        }

        /// <summary>
        /// Test method UpdateProductAsync expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateProductAsync_UpdateProductRequest_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var updateRequest = fixture.Create<UpdateProductRequest>();

            var dbProducts = fixture.CreateMany<DbProduct>(2).ToList();

            var productId = 1;

            dbProducts[0].Id = productId + 1;
            dbProducts[0].Code = $"{updateRequest.Code}1";
            dbProducts[1].Id = productId + 1;
            dbProducts[1].Code = updateRequest.Code;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // assert
            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateProductAsync(productId, updateRequest));
        }

        /// <summary>
        /// Test method UpdateProductAsync expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task UpdateProductAsync_UpdateProductRequest_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var updateRequest = fixture.Create<UpdateProductRequest>();

            var dbProducts = fixture.CreateMany<DbProduct>(2).ToList();

            var productId = 1;

            dbProducts[0].Id = productId + 1;
            dbProducts[0].Code = $"{updateRequest.Code}1";
            dbProducts[1].Id = productId + 1;
            dbProducts[1].Code = $"{updateRequest.Code}1";

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateProductAsync(productId, updateRequest));
        }

        /// <summary>
        /// Test method DeleteProductAsync - delete success
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteProductAsync_ProductId_DeleteSuccess(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var productId = 1;

            var dbProducts = fixture.CreateMany<DbProduct>(3).ToList();

            dbProducts[0].Id = productId + 1;
            dbProducts[1].Id = productId;
            dbProducts[1].IsDeleted = true;
            dbProducts[2].Id = productId + 1;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // act
            await service.DeleteProductAsync(productId);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetProductAsync(productId));
        }

        /// <summary>
        /// Test method DeleteProductAsync - expected RequestedResourceNotFoundException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteProductAsync_ProductId_RequestedResourceNotFoundException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var productId = 1;

            var dbProducts = fixture.CreateMany<DbProduct>(3).ToList();

            dbProducts[0].Id = productId + 1;
            dbProducts[1].Id = productId + 1;
            dbProducts[1].IsDeleted = true;
            dbProducts[2].Id = productId + 1;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // assert
            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteProductAsync(productId));
        }

        /// <summary>
        /// Test method DeleteProductAsync - expected RequestedResourceHasConflictException
        /// </summary>
        /// <param name="context">mock object type IProductCatalogueContext</param>
        /// <param name="service">service for work with product</param>
        /// <param name="fixture">IFixture</param>
        /// <returns>Task</returns>
        [Theory]
        [AutoMoqData]
        public async Task DeleteProductAsync_ProductId_RequestedResourceHasConflictException(
            [Frozen] Mock<IProductCatalogueContext> context,
            ProductCatalogueService service,
            IFixture fixture)
        {
            // arrange
            var productId = 1;

            var dbProducts = fixture.CreateMany<DbProduct>(3).ToList();

            dbProducts[0].Id = productId + 1;
            dbProducts[1].Id = productId;
            dbProducts[1].IsDeleted = false;
            dbProducts[2].Id = productId + 1;

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            // assert
            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteProductAsync(productId));
        }

        /// <summary>
        /// Set Status Product IsDelete
        /// </summary>
        /// <param name="productId">products id</param>
        /// <param name="productStatusBefor">status product befor update</param>
        /// <param name="productStatusAfter">status product after update</param>
        /// <returns>Task</returns>
        [Theory]
        [InlineData(1, false, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, false)]
        [InlineData(1, true, true)]
        public async Task SetStatusAsync_SetProductStatus(int productId, bool productStatusBefor, bool productStatusAfter)
        {
            // arrange
            var dbProduct = new DbProduct();

            dbProduct.Id = productId;

            dbProduct.IsDeleted = productStatusBefor;

            Assert.Equal(productStatusBefor, dbProduct.IsDeleted);

            var dbProducts = new[] { dbProduct };

            var context = new Mock<IProductCatalogueContext>();

            context.Setup(s => s.Products).ReturnsEntitySet(dbProducts);

            var userContext = new Mock<IUserContext>();

            userContext.Setup(s => s.UserId).Returns(1);

            var service = new ProductCatalogueService(context.Object, userContext.Object);

            // act
            await service.SetStatusAsync(productId, productStatusAfter);

            // assert
            Assert.Equal(productStatusAfter, dbProduct.IsDeleted);
        }
    }
}
