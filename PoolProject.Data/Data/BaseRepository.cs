using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PoolProject.InterFace.InterFace;
using PoolProject.InterFace.Data;
using PoolProject.Domain;

namespace PoolProject.Data.Data;
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : BaseModel
    {
        private readonly IUnitOfWork _unitOfWork;

        private ClaimsPrincipal user;

        internal DbSet<T> dbSet;

        public SearchQuery<T> Query;

        public IUnitOfWork UnitOfWork => _unitOfWork;

        internal DbContext Database => _unitOfWork.Db;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            _unitOfWork = unitOfWork;
            DbContext db = _unitOfWork.Db;
            dbSet = db.Set<T>();
            Query = new SearchQuery<T>();
        }

        public virtual void Detached(T entity)
        {
            _unitOfWork.Db.Entry(entity).State = EntityState.Detached;
            _unitOfWork.Db.SaveChanges();
        }

        public T Single(object primaryKey)
        {
            T val = dbSet.Find(primaryKey);
            if (val == null || val.IsDeleted)
            {
                return null;
            }

            return val;
        }

        public T Get(int primaryKey, string include = "")
        {
            if (!string.IsNullOrEmpty(include))
            {
                IQueryable<T> source = EntityFrameworkQueryableExtensions.Include(dbSet, include).AsQueryable();
                foreach (Expression<Func<T, bool>> filter in Query.Filters)
                {
                    source = source.Where(filter);
                }

                T val = source.FirstOrDefault((T x) => x.Id == primaryKey);
                if (val == null || val.IsDeleted)
                {
                    return null;
                }

                return val;
            }

            IQueryable<T> source2 = dbSet.AsQueryable();
            foreach (Expression<Func<T, bool>> filter2 in Query.Filters)
            {
                source2 = source2.Where(filter2);
            }

            T val2 = source2.FirstOrDefault((T x) => x.Id == primaryKey);
            if (val2 == null || val2.IsDeleted)
            {
                return null;
            }

            return val2;
        }

        public T Get()
        {
            return dbSet.FirstOrDefault((T x) => !x.IsDeleted);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> filter)
        {
            return dbSet.Where((T x) => !x.IsDeleted).FirstOrDefault(filter);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> filter, string[] includes)
        {
            IQueryable<T> source = dbSet.AsQueryable();
            foreach (string navigationPropertyPath in includes)
            {
                source = EntityFrameworkQueryableExtensions.Include(dbSet, navigationPropertyPath);
            }

            T val = source.Where((T x) => !x.IsDeleted).FirstOrDefault(filter);
            if (val == null)
            {
                return null;
            }

            return val;
        }

        public bool Exists(object primaryKey)
        {
            T val = dbSet.Find(primaryKey);
            if (val == null)
            {
                return false;
            }

            if (val.IsDeleted)
            {
                return false;
            }

            return true;
        }

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return dbSet.Any(filter);
        }

        public virtual T Insert(T entity)
        {
            entity.IsDeleted = false;
            entity.CreateDate = DateTime.UtcNow;
            string text = "";
            try
            {
            }
            catch (Exception)
            {
            }

            object obj = dbSet.Add(entity);
            try
            {
                _unitOfWork.Db.SaveChanges();
            }
            catch (Exception ex2)
            {
                PropertyInfo property = entity.GetType().GetProperty("Description", BindingFlags.Instance | BindingFlags.Public);
                if (null != property && property.CanWrite)
                {
                    if (ex2.InnerException == null)
                    {
                        property.SetValue(entity, ex2.Message, null);
                    }
                    else if (ex2.InnerException.InnerException == null)
                    {
                        property.SetValue(entity, ex2.InnerException.Message, null);
                    }
                    else
                    {
                        property.SetValue(entity, ex2.InnerException.InnerException.Message, null);
                    }
                }
            }

            return entity;
        }

        public virtual void Insert(List<T> entities)
        {
            dbSet.AddRange(entities);
            _unitOfWork.Db.SaveChanges();
        }

        public virtual void SaveChanges()
        {
            _unitOfWork.Db.SaveChanges();
        }

        public virtual bool Update(T entity, string[] included = null, string[] exclude = null)
        {
            try
            {
                if (included != null)
                {
                    included = included.Where((string x) => x != "IsDeleted" && x != "CreateById" && x != "CreateDate").ToArray();
                }
            }
            catch (Exception)
            {
            }

            dbSet.Attach(entity);
            EntityEntry<T> entityEntry = _unitOfWork.Db.Entry(entity);
            if (included == null)
            {
                _unitOfWork.Db.ChangeTracker.AutoDetectChangesEnabled = false;
                _unitOfWork.Db.Set<T>().Attach(entity);
            }
            else
            {
                _unitOfWork.Db.ChangeTracker.AutoDetectChangesEnabled = false;
                _unitOfWork.Db.Set<T>().Attach(entity);
                string[] array = included;
                foreach (string propertyName in array)
                {
                    entityEntry.Property(propertyName).IsModified = true;
                }
            }

            if (exclude != null)
            {
                foreach (string propertyName2 in exclude)
                {
                    _unitOfWork.Db.ChangeTracker.AutoDetectChangesEnabled = false;
                    _unitOfWork.Db.Set<T>().Attach(entity);
                    entityEntry.Property(propertyName2).IsModified = false;
                }
            }

            try
            {
                bool flag;
                do
                {
                    flag = false;
                    try
                    {
                        _unitOfWork.Db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex2)
                    {
                        flag = true;
                        ex2.Entries.Single().Reload();
                    }
                }
                while (flag);
                return true;
            }
            catch (Exception ex3)
            {
                Console.WriteLine("Update Error=> " + ex3.Message);
                return false;
            }
        }

        public bool Delete(int id)
        {
            T val = Get(id);
            PropertyInfo property = val.GetType().GetProperty("IsDeleted", BindingFlags.Instance | BindingFlags.Public);
            if (null != property && property.CanWrite)
            {
                property.SetValue(val, true, null);
            }

            dbSet.Attach(val);
            _unitOfWork.Db.Entry(val).Property("IsDeleted").IsModified = true;
            _unitOfWork.Db.SaveChanges();
            return true;
        }

        public int PhysicalDelete(T entity)
        {
            if (_unitOfWork.Db.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            dynamic val = dbSet.Remove(entity);
            _unitOfWork.Db.SaveChanges();
            return val.Id;
        }

        public void PhysicalDelete(int id)
        {
            T entity = Get(id);
            if (_unitOfWork.Db.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            object obj = dbSet.Remove(entity);
            _unitOfWork.Db.SaveChanges();
        }

        public bool PhysicalDelete(List<T> entities)
        {
            dbSet.RemoveRange(entities);
            _unitOfWork.Db.SaveChanges();
            return true;
        }

        public Dictionary<string, string> GetAuditNames(dynamic dynamicObject)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll(int take = 50, int skip = 0)
        {
            return dbSet.Where((T x) => !x.IsDeleted).Take(50).Skip(skip * take)
                .ToList();
        }

        public IEnumerable<T> GetAllByParentId(string parentfiled, object id)
        {
            object id2 = id;
            PropertyInfo prop = typeof(T).GetProperty(parentfiled);
            return dbSet.Where((T x) => !x.IsDeleted && prop.GetValue(x, null) == id2).ToList();
        }

        public IQueryable<T> GetQuery(bool includDeleted = false)
        {
            return dbSet.Where((T x) => x.IsDeleted == includDeleted);
        }

        public virtual List<T> Search(Func<T, bool> filter)
        {
            return (from x in dbSet.Where(filter)
                    where !x.IsDeleted
                    select x).ToList();
        }

        public decimal Sum(Func<T, bool> filter, Func<T, decimal?> prop)
        {
            return dbSet.Where(filter).Sum(prop).GetValueOrDefault();
        }

        public virtual PagedListResult<T> Search(string orderby = "", string direction = "")
        {
            SearchQuery<T> query = Query;
            IQueryable<T> source = dbSet;
            source = source.Where((T x) => !x.IsDeleted);
            source = ManageFilters(query, source);
            source = ManageIncludeProperties(query, source);
            source = ManageSortCriterias(query, source);
            if (!string.IsNullOrEmpty(orderby))
            {
                source = OrderBy(source, orderby, direction);
            }

            return GetTheResult(query, source);
        }

        public virtual IQueryable<T> SearchQuery()
        {
            SearchQuery<T> query = Query;
            IQueryable<T> source = dbSet;
            source = source.Where((T x) => !x.IsDeleted);
            source = ManageFilters(query, source);
            return ManageIncludeProperties(query, source);
        }

        protected virtual PagedListResult<T> GetTheResult(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            try
            {
                int num = sequence.Count();
                List<T> entities = ((searchQuery.Take > 0) ? sequence.Skip(searchQuery.Skip).Take(searchQuery.Take).ToList() : sequence.ToList());
                bool hasNext = (searchQuery.Skip > 0 || searchQuery.Take > 0) && searchQuery.Skip + searchQuery.Take < num;
                return new PagedListResult<T>
                {
                    Entities = entities,
                    HasNext = hasNext,
                    HasPrevious = (searchQuery.Skip > 0),
                    Count = num
                };
            }
            catch
            {
                return new PagedListResult<T>
                {
                    Entities = new List<T>(),
                    HasNext = false,
                    HasPrevious = (searchQuery.Skip > 0),
                    Count = 0
                };
            }
        }

        protected virtual IQueryable<T> ManageSortCriterias(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            if (searchQuery.SortCriterias != null && searchQuery.SortCriterias.Count > 0)
            {
            ISortCriteria<T> sortCriteria = searchQuery.SortCriterias[0];
                IOrderedQueryable<T> orderedQueryable = sortCriteria.ApplyOrdering(sequence, useThenBy: false);
                if (searchQuery.SortCriterias.Count > 1)
                {
                    for (int i = 1; i < searchQuery.SortCriterias.Count; i++)
                    {
                        ISortCriteria<T> sortCriteria2 = searchQuery.SortCriterias[i];
                        orderedQueryable = sortCriteria2.ApplyOrdering(orderedQueryable, useThenBy: true);
                    }
                }

                sequence = orderedQueryable;
            }
            else
            {
                sequence = ((IOrderedQueryable<T>)sequence).OrderBy((T x) => true);
            }

            return sequence;
        }

        protected virtual IQueryable<T> ManageFilters(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            if (searchQuery.Filters != null && searchQuery.Filters.Count > 0)
            {
                foreach (Expression<Func<T, bool>> filter in searchQuery.Filters)
                {
                    sequence = sequence.Where(filter);
                }
            }

            return sequence;
        }

        protected virtual IQueryable<T> ManageIncludeProperties(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery.IncludeProperties))
            {
                string[] array = searchQuery.IncludeProperties.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] array2 = array;
                foreach (string navigationPropertyPath in array2)
                {
                    sequence = EntityFrameworkQueryableExtensions.Include(sequence, navigationPropertyPath);
                }
            }

            return sequence;
        }

        public IOrderedQueryable<T> OrderBy(IQueryable<T> query, string memberName, string order = "asc")
        {
            ParameterExpression[] array = new ParameterExpression[1] { Expression.Parameter(typeof(T), "") };
            PropertyInfo property = typeof(T).GetProperty(memberName);
            if (order == "asc")
            {
                return (IOrderedQueryable<T>)query.Provider.CreateQuery(Expression.Call(typeof(Queryable), "OrderBy", new Type[2]
                {
                typeof(T),
                property.PropertyType
                }, query.Expression, Expression.Lambda(Expression.Property(array[0], property), array)));
            }

            return (IOrderedQueryable<T>)query.Provider.CreateQuery(Expression.Call(typeof(Queryable), "OrderByDescending", new Type[2]
            {
            typeof(T),
            property.PropertyType
            }, query.Expression, Expression.Lambda(Expression.Property(array[0], property), array)));
        }

        public void Dispose()
        {
            if (_unitOfWork != null)
            {
                _unitOfWork.Dispose();
            }
        }
    }


