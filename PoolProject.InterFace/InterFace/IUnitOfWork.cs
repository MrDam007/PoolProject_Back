using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolProject.InterFace.InterFace;
public interface IUnitOfWork : IDisposable
{
    DbContext Db { get; }

    void Commit();

    void StartTransaction();
}
