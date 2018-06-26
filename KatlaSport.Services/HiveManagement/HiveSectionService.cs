using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KatlaSport.DataAccess;
using KatlaSport.DataAccess.ProductStoreHive;
using DbHiveSection = KatlaSport.DataAccess.ProductStoreHive.StoreHiveSection;

namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// Represents a hive section service.
    /// </summary>
    public class HiveSectionService : IHiveSectionService
    {
        private readonly IProductStoreHiveContext _context;
        private readonly IUserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiveSectionService"/> class with specified <see cref="IProductStoreHiveContext"/> and <see cref="IUserContext"/>.
        /// </summary>
        /// <param name="context">A <see cref="IProductStoreHiveContext"/>.</param>
        /// <param name="userContext">A <see cref="IUserContext"/>.</param>
        public HiveSectionService(IProductStoreHiveContext context, IUserContext userContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync()
        {
            var dbHiveSections = await _context.Sections
                .OrderBy(s => s.Id)
                .ToArrayAsync().ConfigureAwait(false);

            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();
            return hiveSections;
        }

        /// <inheritdoc/>
        public async Task<HiveSection> GetHiveSectionAsync(int hiveSectionId)
        {
            var dbHiveSection = await _context.Sections
                .FirstOrDefaultAsync(s => s.Id == hiveSectionId)
                .ConfigureAwait(false);

            if (dbHiveSection == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return Mapper.Map<DbHiveSection, HiveSection>(dbHiveSection);
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync(int hiveId)
        {
            var dbHiveSections = await _context.Sections
                .Where(s => s.StoreHiveId == hiveId)
                .OrderBy(s => s.Id)
                .ToListAsync().ConfigureAwait(false);

            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();

            return hiveSections;
        }

        /// <inheritdoc/>
        public async Task<HiveSection> CreateHiveSectionAsync(int hiveId, UpdateHiveSectionRequest createRequest)
        {
            var dbHiveSection = await _context.Sections
                .FirstOrDefaultAsync(h => h.Code == createRequest.Code)
                .ConfigureAwait(false);

            if (dbHiveSection != null)
            {
                throw new RequestedResourceHasConflictException("code");
            }

            var dbHive = await _context.Hives
                .FirstOrDefaultAsync(x => x.Id == hiveId)
                .ConfigureAwait(false);

            if (dbHive == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            var hiveSection = Mapper.Map<UpdateHiveSectionRequest, DbHiveSection>(createRequest);
            hiveSection.StoreHiveId = hiveId;
            hiveSection.CreatedBy = _userContext.UserId;
            hiveSection.LastUpdatedBy = _userContext.UserId;
            hiveSection.Created = DateTime.Now;
            hiveSection.LastUpdated = DateTime.Now;

            _context.Sections.Add(hiveSection);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Mapper.Map<HiveSection>(hiveSection);
        }

        /// <inheritdoc/>
        public async Task<HiveSection> UpdateHiveSectionAsync(int hiveSectionId, UpdateHiveSectionRequest updateRequest)
        {
            var dbHiveSection = await _context.Sections
                .FirstOrDefaultAsync(p => p.Code == updateRequest.Code && p.Id != hiveSectionId)
                .ConfigureAwait(false);

            if (dbHiveSection != null)
            {
                throw new RequestedResourceHasConflictException("code");
            }

            var hiveSection = await _context.Sections
                .FirstOrDefaultAsync(p => p.Id == hiveSectionId)
                .ConfigureAwait(false);

            if (hiveSection == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            Mapper.Map(updateRequest, hiveSection);
            hiveSection.LastUpdatedBy = _userContext.UserId;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Mapper.Map<HiveSection>(hiveSection);
        }

        /// <inheritdoc/>
        public async Task DeleteHiveSectionAsync(int hiveSectionId)
        {
            var dbHiveSection = await _context.Sections
                .FirstOrDefaultAsync(p => p.Id == hiveSectionId)
                .ConfigureAwait(false);

            if (dbHiveSection == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            if (dbHiveSection.IsDeleted == false)
            {
                throw new RequestedResourceHasConflictException();
            }

            _context.Sections.Remove(dbHiveSection);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SetStatusAsync(int hiveSectionId, bool deletedStatus)
        {
            var dbHiveSection = await _context.Sections
                .FirstOrDefaultAsync(c => hiveSectionId == c.Id)
                .ConfigureAwait(false);

            if (dbHiveSection == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            if (dbHiveSection.IsDeleted != deletedStatus)
            {
                dbHiveSection.IsDeleted = deletedStatus;
                dbHiveSection.LastUpdated = DateTime.UtcNow;
                dbHiveSection.LastUpdatedBy = _userContext.UserId;

                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
