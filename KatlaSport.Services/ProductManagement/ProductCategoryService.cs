using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KatlaSport.DataAccess;
using KatlaSport.DataAccess.ProductCatalogue;
using DbProductCategory = KatlaSport.DataAccess.ProductCatalogue.ProductCategory;

namespace KatlaSport.Services.ProductManagement
{
    /// <summary>
    /// Represents a product category service.
    /// </summary>
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCatalogueContext _context;
        private readonly IUserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryService"/> class with specified <see cref="IProductCatalogueContext"/>.
        /// </summary>
        /// <param name="context">A <see cref="IProductCatalogueContext"/>.</param>
        /// <param name="userContext">A <see cref="IUserContext"/>.</param>
        public ProductCategoryService(IProductCatalogueContext context, IUserContext userContext)
        {
            _context = context ?? throw new ArgumentNullException();
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        /// <inheritdoc/>
        public async Task<List<ProductCategoryListItem>> GetCategoriesAsync(int start, int amount)
        {
            var dbCategories = await _context.Categories
                .OrderBy(c => c.Id)
                .Skip(start)
                .Take(amount)
                .ToListAsync().ConfigureAwait(false);

            var categories = dbCategories.Select(c => Mapper.Map<ProductCategoryListItem>(c)).ToList();

            foreach (var category in categories)
            {
                category.ProductCount = await _context.Products
                    .Where(p => p.CategoryId == category.Id)
                    .CountAsync();
            }

            return categories;
        }

        /// <inheritdoc/>
        public async Task<ProductCategory> GetCategoryAsync(int categoryId)
        {
            var dbCategory = await _context.Categories
                .FirstOrDefaultAsync(p => p.Id == categoryId)
                .ConfigureAwait(false);

            if (dbCategory == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return Mapper.Map<DbProductCategory, ProductCategory>(dbCategory);
        }

        /// <inheritdoc/>
        public async Task<ProductCategory> CreateCategoryAsync(UpdateProductCategoryRequest createRequest)
        {
            var dbCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Code == createRequest.Code)
                .ConfigureAwait(false);

            if (dbCategory != null)
            {
                throw new RequestedResourceHasConflictException("code");
            }

            var category = Mapper.Map<UpdateProductCategoryRequest, DbProductCategory>(createRequest);
            category.CreatedBy = _userContext.UserId;
            category.LastUpdatedBy = _userContext.UserId;
            _context.Categories.Add(category);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Mapper.Map<ProductCategory>(category);
        }

        /// <inheritdoc/>
        public async Task<ProductCategory> UpdateCategoryAsync(int categoryId, UpdateProductCategoryRequest updateRequest)
        {
            var dbCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Code == updateRequest.Code && c.Id != categoryId)
                .ConfigureAwait(false);

            if (dbCategory != null)
            {
                throw new RequestedResourceHasConflictException("code");
            }

            dbCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId)
                .ConfigureAwait(false);

            if (dbCategory == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            Mapper.Map(updateRequest, dbCategory);
            dbCategory.LastUpdatedBy = _userContext.UserId;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            var updateCategory = await GetCategoryAsync(categoryId).ConfigureAwait(false);

            return updateCategory;
        }

        /// <inheritdoc/>
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var dbCategory = await _context.Categories
                .FirstOrDefaultAsync(c => categoryId == c.Id)
                .ConfigureAwait(false);

            if (dbCategory == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            if (dbCategory.IsDeleted == false)
            {
                throw new RequestedResourceHasConflictException();
            }

            _context.Categories.Remove(dbCategory);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SetStatusAsync(int categoryId, bool deletedStatus)
        {
            var dbCategory = await _context.Categories
                .FirstOrDefaultAsync(c => categoryId == c.Id)
                .ConfigureAwait(false);

            if (dbCategory == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            if (dbCategory.IsDeleted != deletedStatus)
            {
                dbCategory.IsDeleted = deletedStatus;
                dbCategory.LastUpdated = DateTime.UtcNow;
                dbCategory.LastUpdatedBy = _userContext.UserId;

                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
