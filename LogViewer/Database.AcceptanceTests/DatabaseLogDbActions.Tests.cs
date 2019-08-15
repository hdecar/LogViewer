namespace Database.AcceptanceTests
{
    using System;
    using System.Linq;
    using Models;
    using NUnit.Framework;

    [TestFixture]
	public class DatabaseLogDbActionsTests
	{
	    [SetUp]
		public void SetUp()
		{
			InitializeSystemUnderTest();
		}

	    private DatabaseLogDbActions systemUnderTest;

	    private const string connectionString = @"Data Source=.;Initial Catalog=AdventureWorks2014;Integrated Security=true;";

	    private void InitializeSystemUnderTest()
	    {
	        systemUnderTest = new DatabaseLogDbActions(connectionString);
	    }

	    [TestCase(1, 15, "DatabaseLogId", SortType.DESC, 1597, 1583, 15)]
	    [TestCase(101, 50, "Schema", SortType.ASC, 1566, 931, 50)]
	    [TestCase(1501, 100, "Object", SortType.ASC, 707, 1031, 97)]
        public void GetPagedResult_WithValidParameters_ReturnedExpectedFirstId(int startIndex, int rowCount, string sortColumn, SortType sortType, int expectedFirstDatabaseLogId, int expectedLastDatabaseLogId, int expectedRowCount)
	    {
	        var result = systemUnderTest.GetPagedResult(startIndex, rowCount, sortColumn, sortType).ToList();
	        Assert.AreEqual(expectedFirstDatabaseLogId, result[0].DatabaseLogID);
            Assert.AreEqual(expectedLastDatabaseLogId, result[expectedRowCount - 1].DatabaseLogID);
            Assert.AreEqual(expectedRowCount, result.Count);
        }

	    [TestCase(2000, 15, "DatabaseLogId", SortType.DESC)]
	    public void GetPagedResult_WithInvalidStartIndex_ReturnsNoResult(int startIndex, int rowCount, string sortColumn, SortType sortType)
	    {
	        var result = systemUnderTest.GetPagedResult(startIndex, rowCount, sortColumn, sortType);
            Assert.AreEqual(0, result.Count());
	    }

	    [TestCase(2000, 15, "NotAColumn", SortType.DESC)]
	    public void GetPagedResult_WithInvalidColumnName_ThrowsDbError(int startIndex, int rowCount, string sortColumn, SortType sortType)
	    {
	        try
	        {
	            var result = systemUnderTest.GetPagedResult(startIndex, rowCount, sortColumn, sortType);
	        }
	        catch (Exception ex)
	        {
                Assert.AreEqual("Invalid column name 'NotAColumn'.", ex.Message);
	        }
	    }

	    [Test]
	    public void GetTotalRecordCount_WhenDbError_ThrowsException()
	    {
	        try
	        {
	            systemUnderTest = new DatabaseLogDbActions(string.Empty);
	            systemUnderTest.GetTotalRecordCount();
                Assert.IsTrue(false, "An error was not thrown.");
	        }
	        catch
	        {
	            Assert.IsTrue(true);
	        }
	    }

	    [Test]
	    public void GetTotalRecordCount_WhenInvoked_ReturnsCorrectNumberOfRecords()
	    {
	        var result = systemUnderTest.GetTotalRecordCount();
            Assert.AreEqual(1597, result);
	    }
	}
}
