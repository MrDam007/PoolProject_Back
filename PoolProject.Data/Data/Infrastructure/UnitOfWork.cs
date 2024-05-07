using Microsoft.EntityFrameworkCore;
using PoolProject.InterFace.InterFace.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PoolProject.Data.Data.Infrastructure;
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private TransactionScope _transaction;

    private readonly DbContext _db;

    public DbContext Db => _db;

    public UnitOfWork(DbContext context)
    {
        _db = context;
    }

    public void Dispose()
    {
    }

    public void StartTransaction()
    {
        _transaction = new TransactionScope();
    }

    public void Commit()
    {
        _db.SaveChanges();
        _transaction.Complete();
    }
}

