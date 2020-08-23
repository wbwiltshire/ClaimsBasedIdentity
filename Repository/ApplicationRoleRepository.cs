using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Data.Repository
{
    public class ApplicationRoleRepository : RepositoryBase<ApplicationRole>, IRepository<ApplicationRole>
    {
        public ApplicationRoleRepository(AppSettingsConfiguration s, ILogger l, IDBConnection d) :
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

        public override ICollection<ApplicationRole> FindAll()
        {
            return base.FindAll();
        }

		public IPager<ApplicationRole> FindAll(IPager<ApplicationRole> pager)
		{
			throw new NotImplementedException();
		}

		public override ApplicationRole FindByPK(IPrimaryKey pk)
		{
			throw new NotImplementedException();
		}

		public object Add(ApplicationRole entity)
		{
			throw new NotImplementedException();
		}

		public int Update(ApplicationRole entity)
		{
			throw new NotImplementedException();
		}

		public int Delete(PrimaryKey pk)
		{
			throw new NotImplementedException();
		}
	}
}
