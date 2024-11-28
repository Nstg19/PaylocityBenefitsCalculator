using System.Linq.Expressions;

namespace Api.Data.Repositories;

public interface IRepository<T>
{
	IQueryable<T> GetAll();
	IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
	Task<T> CreateAsync(T entity);
}
