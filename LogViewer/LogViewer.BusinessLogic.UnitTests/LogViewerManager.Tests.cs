namespace LogViewer.BusinessLogic.UnitTests
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
	public class LogViewerManagerTests
	{
	    [SetUp]
		public void SetUp()
		{
			InitializePagedResultMock();
			systemUnderTest = InitializeSystemUnderTest();
		}

	    private void InitializePagedResultMock()
		{
			dbActionsMock = Substitute.For<IPagerDbActions<DatabaseLog>>();
		}

	    private void SetMockResultSet(int numberOfRecords)
	    {
	        var resultSet = new List<DatabaseLog>();
	        for (int i = 1; i <= numberOfRecords; i++)
	        {
	            resultSet.Add(
	                new DatabaseLog
	                {
	                    DatabaseLogID = i,
	                    DatabaseUser = $"user{i}",
	                    Event = $"event{i}",
	                    Object = $"object{i}",
	                    Schema = $"schema{i}",
	                    TSQL = $"tsql {i}"
	                });
	        }

	        dbActionsMock.GetPagedResult(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<SortType>())
	            .Returns(resultSet);
	        dbActionsMock.GetTotalRecordCount().Returns(numberOfRecords);
	    }

	    private LogViewerManager systemUnderTest;
	    private IPagerDbActions<DatabaseLog> dbActionsMock;

	    private LogViewerManager InitializeSystemUnderTest()
		{
			return new LogViewerManager(dbActionsMock);
		}

	    [TestCase(1, 15, 1)]
	    [TestCase(2, 10, 11)]
	    [TestCase(5, 20, 81)]
        public void GetFirstRowIndex_WithParameters_ReturnsExpectedFirstRowIndex(int pageNumber, int pageSize, int expectedFirstRowNumber)
	    {
	        systemUnderTest.PageNumber = pageNumber;
	        systemUnderTest.PageSize = pageSize;
            var actualFirstRowIndex = systemUnderTest.GetFirstRowIndex();
	        Assert.AreEqual(expectedFirstRowNumber, actualFirstRowIndex);
	    }

	    [TestCase(20, 15, 2)]
	    [TestCase(21, 10, 3)]
	    [TestCase(100, 25, 4)]
	    [TestCase(0, 15, 0)]
        public void GetTotalNumberOfPages_WhenInvoked_ReturnExpectedValue(int totalRecords, int pageSize, int expectedNumberOfPages)
	    {
	        systemUnderTest.PageSize = pageSize;
	        SetMockResultSet(totalRecords);
            var actualResult = systemUnderTest.GetTotalNumberOfPages();
            Assert.AreEqual(expectedNumberOfPages, actualResult);
	    }

	    [TestCase(1, 15, "DatabaseLogID", 1)]
	    [TestCase(2, 50, "Schema", 51)]
        public void GetLogs_WhenInvokedWithVariablesSet_CallsDatabaseWithCorrectParameters(int pageNumber, int pageSize, string columnName, int expectedFirstRowIndex)
	    {
	        systemUnderTest.PageNumber = pageNumber;
	        systemUnderTest.PageSize = pageSize;
	        systemUnderTest.SortType = SortType.ASC;
	        systemUnderTest.SortColumnName = columnName;
            var actualResult = systemUnderTest.GetData();
	        dbActionsMock.Received(1).GetPagedResult(expectedFirstRowIndex, pageSize, columnName, SortType.ASC);
	    }

	    [Test]
	    public void Constructor_WhenInvoked_SetsDefaults()
	    {
            Assert.AreEqual(15, systemUnderTest.PageSize);
            Assert.AreEqual(SortType.DESC, systemUnderTest.SortType);
	        Assert.AreEqual(1, systemUnderTest.PageNumber);
	        Assert.AreEqual("DatabaseLogID", systemUnderTest.SortColumnName);
        }

	    [Test]
	    public void GetLogs_WhenInvokedWithDefaults_CallsDatabaseWithCorrectParameters()
	    {
	        var actualResult = systemUnderTest.GetData();
	        dbActionsMock.Received(1).GetPagedResult(1, 15, "DatabaseLogID", SortType.DESC);
	    }

	    [Test]
	    public void GetTotalRecordCount_WhenInvoked_ReturnsCorrectCount()
	    {
	        SetMockResultSet(20);
	        var actualResult = systemUnderTest.GetTotalRecordCount();
	        Assert.AreEqual(20, actualResult);
	    }
	}
}
