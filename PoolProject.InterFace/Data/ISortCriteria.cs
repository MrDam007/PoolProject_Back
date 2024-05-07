using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolProject.InterFace.Data;
public interface ISortCriteria<T>
{
    IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, bool useThenBy);
}
