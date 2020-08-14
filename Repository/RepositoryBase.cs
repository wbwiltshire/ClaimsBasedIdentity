using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

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

        #region FindAllPaged
        public virtual ICollection<TEntity> FindAllPaged(int offset, int pageSize)
        {

            try
            {
                if (!dbc.IsOpen)
                    dbc.Open();
                return dbc.Select<TEntity>().Skip(offset).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }
        #endregion

        #region FindByPK
        public virtual TEntity FindByPK(IPrimaryKey pk)
        {
            TEntity entity = null;

            try
            {
                if (!dbc.IsOpen)
                    dbc.Open();

                entity = (TEntity)dbc.Select<TEntity>(pk);
                logger.LogInformation($"FindByPK complete for {typeof(TEntity)} entity.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }

            return entity;
        }
        #endregion

        #region Add
        protected object Add(TEntity entity, PrimaryKey pk)
        {
            object result = null;

            try
            {
                if (!dbc.IsOpen)
                    dbc.Open();

                logger.LogInformation($"Add complete for {typeof(TEntity)} entity.");
                result = dbc.Add<TEntity>(entity, pk);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }

            return result;
        }
		#endregion

	}
}
