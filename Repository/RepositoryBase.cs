using ClaimsBasedIdentity.Data.POCO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace ClaimsBasedIdentity.Data.Repository
{
	public abstract class RepositoryBase<TEntity>
		where TEntity : class
	{
		private readonly ILogger logger;
		private DBConnection dbc;

		protected RepositoryBase(AppSettingsConfiguration s, ILogger l, DBConnection d)
		{
			Settings = s;
			logger = l;
			dbc = d;
		}

		public string OrderBy { get; set; }
		protected string CMDText { get; set; }
		protected AppSettingsConfiguration Settings { get; }

		public DBConnection Connection { get { return dbc; } }

        #region FindAllCount
        protected int FindAllCount()
        {
            try
            {
                if (!dbc.IsOpen)
                    dbc.Open();

                    //Returns an object, not an int
                    logger.LogInformation("FindAllCount complete.");
                    return (int)dbc.Select<TEntity>().Count;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return 0;
            }
        }
        #endregion

        #region FindAll
        public virtual ICollection<TEntity> FindAll()
        {

            try
            {
                if (!dbc.IsOpen)
                    dbc.Open();

                    ICollection<TEntity> entities = new List<TEntity>();
                    logger.LogInformation($"FindAll complete for {typeof(TEntity)} entity.");
                    return dbc.Select<TEntity>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }
        #endregion
    }
}
