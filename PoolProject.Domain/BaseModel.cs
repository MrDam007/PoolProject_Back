using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolProject.Domain;
public class BaseModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public bool IsDeleted { get; set; }
}

