using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private MiniETicaretDbContext _context { get; }
        private DbSet<T> Table { get; }
        public Repository(MiniETicaretDbContext context)
        {
            _context = context;
            Table = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await Table.AddAsync(entity);
        }
        public void Update(T entity)
        {
            Table.Update(entity);
        }
        public void Delete(T entity)
        {
            Table.Remove(entity);
        }
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await Table.FindAsync(id);
        }
        public IQueryable<T> GetByFiltered(Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>[]? include = null,
            bool IsTracking = false)
        {
            IQueryable<T> query = Table;
            if (predicate is not null)
                query = query.Where(predicate);

            if (include is not null)
            {
                foreach (var includeExpression in include)
                    query = query.Include(includeExpression);
            }

            if (IsTracking)
                query = query.AsNoTracking();

            return query;
        }

        public IQueryable<T> GetAll(bool IsTracking = false)
        {
            if (!IsTracking)
                return Table.AsNoTracking();
            return Table;
        }

        public IQueryable<T> GetAllFiltered(Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>[]? include = null,
            Expression<Func<T, object>>? orderBy = null,
            bool IsOrderByAsc = true,
            bool IsTracking = false)
        {
            IQueryable<T> query = Table;

            if (predicate is not null)
                query = query.Where(predicate);

            if (include is not null)
                foreach (var includeExpression in include)
                {
                    query = query.Include(includeExpression);
                }

            if (orderBy is not null)
            {
                if (IsOrderByAsc)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            if (IsTracking)
                query = query.AsNoTracking();

            return query;

        }



        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }


    }

}

