using handlang.service.Domain;
using iNet.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace iNet.Service
{
    public class CommonService<TEntity, TKey> : ICommonService<TEntity, TKey> where TEntity : class
    {
        protected DmzContainer _db;
        protected DbContextTransaction _transaction;

        public CommonService()
        {
            this._db = new DmzContainer();
        }

        public CommonService(DmzContainer db)
        {
            this._db = db;
        }

        public virtual async Task Create(TEntity param)
        {
            try
            {
                var paramKey = param.GetType().GetProperties().SingleOrDefault(x => x.Name == "Id");
                if (paramKey != null)
                {
                    if (paramKey.PropertyType == typeof(Guid))
                    {
                        //--如果已經有自帶的 Guid，就不管他，但如果沒有的話幫他建

                        Guid id = (Guid)paramKey.GetValue(param);
                        if (id == Guid.Empty)
                        {
                            paramKey.SetValue(param, Guid.NewGuid());
                        }

                    }
                }

                this._db.Set<TEntity>().Attach(param);
                this._db.Set<TEntity>().Add(param);
                await this._db.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task Update(TEntity param)
        {
            try
            {
                var paramKey = param.GetType().GetProperties().SingleOrDefault(x => x.Name == "Id");

                if (paramKey != null)
                {
                    var oriTEntity = await this._db.Set<TEntity>().FindAsync(paramKey.GetValue(param));

                    if (oriTEntity != null)
                    {
                        this._db.Entry(oriTEntity).CurrentValues.SetValues(param);
                        await this._db.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new Exception("泛型物件不包含\"Id\"這個屬性！");
                }

            }
            catch
            {
                throw;
            }
        }

        public virtual async Task Delete(TKey id)
        {
            try
            {
                this._db.Set<TEntity>().Remove(await this._db.Set<TEntity>().FindAsync(id));
                await this._db.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task<TEntity> GetById(TKey id)
        {
            try
            {
                return await this._db.Set<TEntity>().FindAsync(id);
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task<List<TEntity>> GetAll()
        {
            try
            {
                return await this._db.Set<TEntity>().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        //public virtual async Task<SearchResult<TEntity>> Search(SearchParametersV1<TEntity> param)
        //{
        //    return new SearchResult<TEntity>();
        //}

        public void BeginTrans()
        {
            if (_transaction != null) _transaction.Rollback();
            this._transaction = this._db.Database.BeginTransaction();
        }

        public void CommitTrans()
        {
            if (_transaction == null) return;
            this._transaction.Commit();
            this._transaction.Dispose();
            this._transaction = null;
        }

        public void RollbackTrans()
        {
            if (_transaction == null) return;
            this._transaction.Rollback();
            this._transaction.Dispose();
            this._transaction = null;
        }
    }
}
