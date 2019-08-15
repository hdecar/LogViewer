namespace Interfaces
{
	using System.Collections.Generic;
	using Models;

    public interface IPagerDbActions<out TEntity>
	{
	    IEnumerable<TEntity> GetPagedResult(int startRowIndex, int returnCount, string sortColumnName, SortType sortOrder);
	    int GetTotalRecordCount();

	}
}
