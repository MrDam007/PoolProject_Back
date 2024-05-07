using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PoolProject.Domain;
using PoolProject.InterFace.InterFace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoolProject.Data.Data;
public class BaseService<TEntity, TGet, TCreate, TUpdate> : IBaseService<TEntity, TGet, TCreate, TUpdate> where TEntity : BaseModel
{
    private readonly IBaseRepository<TEntity> repo;

    public BaseService(IBaseRepository<TEntity> _repo)
    {
        repo = _repo;
    }

    public bool Delete(int id)
    {
        return repo.Delete(id);
    }

    public TGet Get(int primaryKey, string include = "")
    {
        return ParseEntityToGet(repo.Get(primaryKey, include));
    }

    public TGet Get()
    {
        return ParseEntityToGet(repo.Get());
    }

    public IEnumerable<TGet> GetAll(int take = 50, int skip = 0)
    {
        List<TGet> list = new List<TGet>();
        foreach (TEntity item in repo.GetAll(take, skip))
        {
            try
            {
                list.Add(ParseEntityToGet(item));
            }
            catch (Exception)
            {
            }
        }

        return list;
    }

    public IQueryable<TEntity> GetQuery(bool includDeleted = false)
    {
        return repo.GetQuery(includDeleted);
    }

    public TEntity Insert(TCreate entity)
    {
        return repo.Insert(ParseDtoToEntity(entity));
    }

    public void Insert(List<TCreate> entity)
    {
        List<TEntity> list = new List<TEntity>();
        foreach (TCreate item in entity)
        {
            try
            {
                list.Add(ParseDtoToEntity(item));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    public bool Update(TUpdate entity, List<string> included = null, List<string> excluded = null)
    {
        try
        {
            TEntity val = ParseDtoToEntity(entity);
            Type type = val.GetType();
            Type type2 = entity.GetType();
            IList<PropertyInfo> list = new List<PropertyInfo>(type.GetProperties());
            IList<PropertyInfo> source = new List<PropertyInfo>(type2.GetProperties());
            if (included == null)
            {
                included = new List<string>();
            }

            if (excluded == null)
            {
                excluded = new List<string>();
            }

            foreach (PropertyInfo prop in list)
            {
                Type propertyType = prop.PropertyType;
                if (!propertyType.IsGenericType && prop.Name.ToLower() != "id" && source.Any((PropertyInfo x) => x.Name.ToLower() == prop.Name.ToLower()))
                {
                    object value = prop.GetValue(val, null);
                    included.Add(prop.Name);
                }
            }

            return repo.Update(val, included.ToArray(), excluded.ToArray());
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private TEntity ParseDtoToEntity(object item)
    {
        try
        {
            string value = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<TEntity>(value);
        }
        catch (Exception)
        {
            throw new Exception("DTO is not currect");
        }
    }

    private TGet ParseEntityToGet(TEntity item)
    {
        try
        {
            string value = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<TGet>(value);
        }
        catch (Exception)
        {
            throw new Exception("DTO is not currect");
        }
    }
}
