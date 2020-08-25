using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Data.Repository
{
    public class ApplicationAuditLogRepository : RepositoryBase<ApplicationAuditLog>, IRepository<ApplicationAuditLog>
    {
        public ApplicationAuditLogRepository(AppSettingsConfiguration s, ILogger l, IDBConnection d) :
            base(s, l, d)
        {
            Initialize(l);
        }

        private ILogger logger;

        private void Initialize(ILogger l)
        {
            logger = l;
            OrderBy = "Id";
        }

        public override ICollection<ApplicationAuditLog> FindAll()
        {
            return base.FindAll();
        }

        public IPager<ApplicationAuditLog> FindAll(IPager<ApplicationAuditLog> pager)
        {
            pager.RowCount = base.FindAllCount();
            pager.Entities = base.FindAllPaged(pager.PageNbr, pager.PageSize);
            return pager;
        }

        public override ApplicationAuditLog FindByPK(IPrimaryKey pk)
        {
            throw new NotImplementedException();
        }

        public object Add(ApplicationAuditLog entity)
        {
            return base.Add(entity, new PrimaryKey() { Key = -1, IsIdentity = true });
        }

        public int Update(ApplicationAuditLog entity)
        {
            return base.Update(entity, entity.PK);
        }

        public int Delete(PrimaryKey pk)
        {
            return base.Delete(pk);
        }

    }
}
