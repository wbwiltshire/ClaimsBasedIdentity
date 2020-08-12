using System;
using System.Collections.Generic;

namespace ClaimsBasedIdentity.Data.Interfaces
{
    public interface IDBConnection
	{
        bool IsOpen { get; set; }
        bool Open();
        void Close();
    }

    public interface IPrimaryKey
    {
        object Key { get; set; }
        bool IsIdentity { get; set; }
        bool IsComposite { get; set; }
    }

    public interface IPager<TEntity>
    where TEntity : class
    {
        int PageNbr { get; set; }
        int PageSize { get; set; }
        int RowCount { get; set; }
        ICollection<TEntity> Entities { get; set; }
    }
    public class Pager<TEntity> : IPager<TEntity>
        where TEntity : class
    {
        public int PageNbr { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public ICollection<TEntity> Entities { get; set; }
    }

    public interface IRepository<TEntity>
        where TEntity : class
    {
        IPager<TEntity> FindAll(IPager<TEntity> pager);
        ICollection<TEntity> FindAll();
        TEntity FindByPK(IPrimaryKey pk);
        object Add(TEntity entity);
        int Update(TEntity entity);
        int Delete(PrimaryKey pk);
    }
}
