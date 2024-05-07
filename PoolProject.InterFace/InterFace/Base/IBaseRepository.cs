using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolProject.InterFace.InterFace.Base;S
public interface IBaseRepository<T> where T : class
{

    T Get(int primaryKey, string include = "");

    T Get();

    IEnumerable<T> GetAll(int take = 50, int skip = 0);

    IQueryable<T> GetQuery(bool includDeleted = false);
    T Insert(T entity);

    void Insert(List<T> entity);

    bool Update(T entity, string[] included = null, string[] excluded = null);

    bool Delete(int id);

}
