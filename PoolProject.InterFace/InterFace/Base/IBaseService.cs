using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolProject.InterFace.InterFace.Base;
public interface IBaseService<TEntity, TGet, TCreate, TUpdate> where TEntity : class
{

    TGet Get(int primaryKey, string include = "");

    TGet Get();

    IEnumerable<TGet> GetAll(int take = 50, int skip = 0);

    IQueryable<TEntity> GetQuery(bool includDeleted = false);

    TEntity Insert(TCreate entity);

    void Insert(List<TCreate> entity);

    bool Update(TUpdate entity, List<string> included = null, List<string> excluded = null);

    bool Delete(int id);
}
