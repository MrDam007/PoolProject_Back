using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolProject.InterFace.InterFace;
public class PagedListResult<TEntity>
{
    public bool HasNext { get; set; }

    public bool HasPrevious { get; set; }

    public int Count { get; set; }

    public IEnumerable<TEntity> Entities { get; set; }
}
