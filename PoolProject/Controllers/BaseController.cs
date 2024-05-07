using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using PoolProject.Domain;
using PoolProject.InterFace.InterFace.Base;

namespace PoolProject.Controllers
{
    public class BaseController<TEntity, TGet, TCreate, TUpdate> : ControllerBase where TEntity : BaseModel
    {
        public readonly IBaseService<TEntity, TGet, TCreate, TUpdate> service;
        public BaseController(IBaseService<TEntity, TGet, TCreate, TUpdate> _service)
        {
            service = _service;
        }

        [HttpPost("Create")]
        public TEntity Create([FromBody] TCreate item) => service.Insert(item);

        [HttpGet("GetAll")]
        public IEnumerable<TGet> GetAll(int take = 50, int skip = 0) => service.GetAll(take, skip);

        [HttpGet("Get")]
        public TGet Get(int id) => service.Get(id);

        [HttpDelete]
        public bool Delete(int id) => service.Delete(id);

        [HttpPut("Update")]
        public bool Update([FromBody] TUpdate item) => service.Update(item);
    }
}
