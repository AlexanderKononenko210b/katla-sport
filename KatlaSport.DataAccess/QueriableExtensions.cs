using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Extensions = System.Data.Entity.QueryableExtensions;

namespace KatlaSport.DataAccess
{
    /// <summary>
    /// Extension class for IQueryable{T}
    /// </summary>
    public static class QueriableExtensions
    {
        /// <summary>
        /// Get List{T} from database as asynchron operation
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">IQueryable{T}</param>
        /// <returns>Task{List{T}}</returns>
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
            where T : class
        {
            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            return Extensions.ToListAsync(source);
        }

        /// <summary>
        /// Get T[] from database as asynchron operation
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">IQueryable{T}</param>
        /// <returns>Task{T[]}</returns>
        public static Task<T[]> ToArrayAsync<T>(this IQueryable<T> source)
            where T : class
        {
            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            return Extensions.ToArrayAsync(source);
        }

        /// <summary>
        /// Get count item from database as asynchron operation
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">IQueryable{T}</param>
        /// <returns>Task{int}</returns>
        public static Task<int> CountAsync<T>(this IQueryable<T> source)
            where T : class
        {
            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            return Extensions.CountAsync(source);
        }

        /// <summary>
        /// Get first or default item from database as asynchron operation
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">IQueryable{T}</param>
        /// <param name="predicat">Expression{Func{T, bool}}</param>
        /// <returns>Task{T}</returns>
        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicat)
            where T : class
        {
            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            return Extensions.FirstOrDefaultAsync(source, predicat);
        }
    }
}
