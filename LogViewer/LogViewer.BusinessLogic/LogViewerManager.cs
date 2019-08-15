namespace LogViewer.BusinessLogic
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class LogViewerManager : IPager<DatabaseLog>
    {
        private readonly IPagerDbActions<DatabaseLog> dbActions;
        private IEnumerable<DatabaseLog> resultSet;

        public LogViewerManager(IPagerDbActions<DatabaseLog> dbActions)
		{
			this.dbActions = dbActions;
            SetDefaults();
		}

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortColumnName { get; set; }
        public SortType SortType { get; set; }

        public IEnumerable<DatabaseLog> GetData()
	    {
	        int rowStartIndex = GetFirstRowIndex();
	        resultSet = dbActions.GetPagedResult(rowStartIndex, PageSize, SortColumnName, SortType);
	        return resultSet;
	    }

        public int GetTotalNumberOfPages()
	    {
	        int totalRecords = GetTotalRecordCount();
            int pages = totalRecords / PageSize;
	        if ((pages * PageSize) < totalRecords)
	        {
	            pages++;
	        }

            return pages;
	    }

        public int GetTotalRecordCount()
	    {
            return dbActions.GetTotalRecordCount();
	    }

        public int GetFirstRowIndex()
	    {
	        if (PageNumber == 1)
	            return 1;
	        return ((PageNumber * PageSize) - (PageSize-1));
	    }

        private void SetDefaults()
        {
            PageNumber = 1;
            PageSize = 15;
            SortColumnName = "DatabaseLogID";
            SortType = SortType.DESC;
        }
    }
}
