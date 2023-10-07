using System.Collections.Generic;
using System.Threading.Tasks;

namespace iNet.Service.Interface
{
    public interface ICommonService<TEntity, TKey> where TEntity : class
    {
        Task Create(TEntity param);
        Task Update(TEntity param);
        Task Delete(TKey id);
        //Task<SearchResult<TEntity>> Search(SearchParametersV1<TEntity> param);

        Task<TEntity> GetById(TKey id);
        Task<List<TEntity>> GetAll();

        void BeginTrans();
        void CommitTrans();
        void RollbackTrans();
    }
}
