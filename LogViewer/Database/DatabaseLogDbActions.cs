namespace Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Interfaces;
    using Models;

    public class DatabaseLogDbActions : IPagerDbActions<DatabaseLog>
	{
	    private readonly string connectionString;
	    private int totalNumberOfRecords;

	    public DatabaseLogDbActions(string connectionString)
		{
		    this.connectionString = connectionString;
		}

	    public IEnumerable<DatabaseLog> GetPagedResult(int startRowIndex, int returnCount, string sortColumnName, SortType sortOrder)
		{
		    var returnResult = new List<DatabaseLog>();

		    try
		    {
		        string sql = CreateQuery(startRowIndex, returnCount, sortColumnName, sortOrder);
                
                DataSet ds = ExecuteQuery(sql);
		        PopulateResult(returnResult, ds);
		    }
		    catch (Exception ex)
		    {
                //TODO: handle error
		        throw ex;
		    }

		    return returnResult;

		}

	    public int GetTotalRecordCount()
	    {
	        if (totalNumberOfRecords == 0)
	        {

	            try
	            {
	                string sql =
	                    "select count([DatabaseLogID]) as TotalRecords from [AdventureWorks2014].[dbo].[DatabaseLog]";

	                using (var conn = new SqlConnection(connectionString))
	                {
	                    var comm = new SqlCommand(sql, conn);
	                    conn.Open();
	                    totalNumberOfRecords = (int) comm.ExecuteScalar();
	                    conn.Close();
	                }
	            }
	            catch (Exception ex)
	            {
	                //TODO: handle error
	                throw ex;
	            }

	        }

	        return totalNumberOfRecords;

	    }

	    private DatabaseLog CreateDatabaseLogFromRow(DataRow row)
	    {
	        return new DatabaseLog
	        {
                DatabaseLogID = (int) row["DatabaseLogID"],
	            Schema = row["Schema"].ToString(),
	            DatabaseUser = row["DatabaseUser"].ToString(),
	            Event = row["Event"].ToString(),
	            Object = row["Object"].ToString(),
	            TSQL = row["TSQL"].ToString()
	        };

	    }

	    private string CreateQuery(int startRowIndex, int returnCount, string sortColumnName, SortType sortOrder)
	    {
            //TODO: If this is used in a real-world application, refactor in a way that is not prone to SQL Injection by using a stored procedure, or at the very least cleanse sortColumnName
	        return $@"select top {returnCount}
                        [RowTable].[RowNumber], 
                        [Log].[DatabaseLogID],
                        [Log].[DatabaseUser],
                        [Log].[Event],
                        [Log].[Schema],
                        [Log].[Object],
                        [Log].[TSQL]  
                        FROM [AdventureWorks2014].[dbo].[DatabaseLog] as [Log]
                        inner join (SELECT ROW_NUMBER() OVER(ORDER BY [{sortColumnName}] {sortOrder}) AS [RowNumber], [DatabaseLogID] FROM [AdventureWorks2014].[dbo].[DatabaseLog]) as [RowTable] on [RowTable].DatabaseLogID = [Log].DatabaseLogID
                        where [RowTable].[RowNumber] >= {startRowIndex}
                        order by [RowTable].[RowNumber]";
	    }

	    private DataSet ExecuteQuery(string sql)
	    {
	        var result = new DataSet();
	        using (var conn = new SqlConnection(connectionString))
	        {
	            var dataAdapter = new SqlDataAdapter(sql, conn);
	            dataAdapter.SelectCommand.CommandType = CommandType.Text;
                dataAdapter.Fill(result);
	        }
	        return result;
	    }

	    private void PopulateResult(List<DatabaseLog> returnResult, DataSet ds)
	    {
	        if (ds.Tables?.Count > 0)
	        {
	            foreach (DataRow row in ds.Tables[0].Rows)
	            {
	                returnResult.Add(CreateDatabaseLogFromRow(row));
	            }
            }
	    }
	}
}
