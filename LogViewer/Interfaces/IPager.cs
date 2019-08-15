namespace Interfaces
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IPager<out TEntity>
    {
        int GetTotalRecordCount();

        int GetTotalNumberOfPages();

        IEnumerable<TEntity> GetData();
    }
}
