using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Api.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
	private readonly EmployeeDbContext _employeeDbContext;
	private readonly DbSet<T> _dbSet;

	public Repository(EmployeeDbContext employeeDbContext)
	{
		_employeeDbContext = employeeDbContext;
		_dbSet = _employeeDbContext.Set<T>();
	}

	public IQueryable<T> GetAll()
	{
		return _dbSet.AsQueryable();
	}

	public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
	{
		return _dbSet.Where(predicate);
	}

	public async Task<T> CreateAsync(T entity)
	{
		var result = await _dbSet.AddAsync(entity);
		await _employeeDbContext.SaveChangesAsync();

		return result.Entity;
	}
}
