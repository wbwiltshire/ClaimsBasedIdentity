using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Data.Repository
{
    public class ApplicationUserClaimRepository : RepositoryBase<ApplicationUserClaim>, IRepository<ApplicationUserClaim>
    {
        public ApplicationUserClaimRepository(AppSettingsConfiguration s, ILogger l, IDBConnection d) : 
            base(s, l, d)
        {
            Initialize(l);
        }

        private ILogger logger;

        private void Initialize(ILogger l)
		{
            logger = l;
            OrderBy = "Id,UserId";
		}

        public override ICollection<ApplicationUserClaim> FindAll()
        {
            return base.FindAll();
        }

        public object Add(ApplicationUserClaim entity)
        {
            return base.Add(entity, new PrimaryKey() { Key = -1, IsIdentity = true });
        }

        public int Delete(PrimaryKey pk)
        {
            return base.Delete(pk);
        }

        public IPager<ApplicationUserClaim> FindAll(IPager<ApplicationUserClaim> pager)
        {
            throw new NotImplementedException();
        }

        public override ApplicationUserClaim FindByPK(IPrimaryKey pk)
        {
            throw new NotImplementedException();
        }

        public int Update(ApplicationUserClaim entity)
        {
            return base.Update(entity, entity.PK);
        }
    }
}
