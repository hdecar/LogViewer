namespace LogViewer.UI
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.UI;
    using BusinessLogic;
    using Database;
    using Models;

    public partial class _default : Page
    {
        private int currentPage;

        /*
 Need to display column headers and/or field labels that tell me what the data represents.

-A mechanism that allows me to control the number of rows to display.

-Paging mechanism that allow me to page forward and backward.

-A sorting mechanism that allows me to control the order of the data.
         */
        private SortType sortType;

        private void BindGrid(IEnumerable<DatabaseLog> logData)
        {
            gvLogs.DataSource = logData;
            gvLogs.DataBind();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int nextPage = GetNextPage();
            SetCurrentPage(nextPage);
            LoadLogs();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            int prevPage = GetPreviousPage();
            SetCurrentPage(prevPage);
            LoadLogs();
        }

        private LogViewerManager CreateLogViewer()
        {
            var pagerDb = new DatabaseLogDbActions(GetConnectionString());
            var logViewer = new LogViewerManager(pagerDb);
            sortType = GetPersistedSortType();
            logViewer.PageSize = PageSize();
            logViewer.PageNumber = CurrentPage();
            logViewer.SortColumnName = SortColumnName();
            logViewer.SortType = sortType;
            return logViewer;
        }

        private int CurrentPage()
        {
            return Convert.ToInt32(lblCurrentPage.Text);
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetCurrentPage();
            LoadLogs();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["AdvertureWorks2014"].ConnectionString;
        }

        private int GetNextPage()
        {
            int maxPage = NumberOfPages();
            int nextPage = CurrentPage() + 1;
            if (nextPage > maxPage)
            {
                nextPage = maxPage;
            }

            return nextPage;
        }

        private SortType GetPersistedSortType()
        {
            string persistedSortType = hfSortDirection.Value;
            if (string.IsNullOrEmpty(hfSortDirection.Value) 
                || persistedSortType == SortType.ASC.ToString())
            {
                return SortType.ASC;
            }

            return SortType.DESC;
        }

        private int GetPreviousPage()
        {
            int minPage = 1;
            int prevPage = CurrentPage() - 1;
            if (prevPage == 0)
            {
                prevPage = 1;
            }

            return prevPage;
        }

        protected void gvLogs_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
        {
            UpdateSorting(e.SortExpression);
            LoadLogs();
        }

        private void LoadLogs()
        {
            var logViewer = CreateLogViewer();
            IEnumerable<DatabaseLog> logData = logViewer.GetData();
            BindGrid(logData);
            PersistPagingOptions(logViewer);
        }

        private int NumberOfPages()
        {
            return Convert.ToInt32(lblTotalPages.Text);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddlPageSize.SelectedValue = "15";
                ResetCurrentPage();
                LoadLogs();
            }
            
        }

        private int PageSize()
        {
            return Convert.ToInt32(ddlPageSize.SelectedValue);
        }

        private void PersistPagingOptions(LogViewerManager logViewer)
        {
            lblTotalPages.Text = logViewer.GetTotalNumberOfPages().ToString();
            hfSortByColumn.Value = logViewer.SortColumnName;
            hfSortDirection.Value = logViewer.SortType.ToString();
        }

        private void ResetCurrentPage()
        {
            lblCurrentPage.Text = "1";
        }

        private void SetCurrentPage(int nextPage)
        {
            lblCurrentPage.Text = Convert.ToString(nextPage);
        }

        private string SortColumnName()
        {
            if (string.IsNullOrEmpty(hfSortByColumn.Value))
            {
                return "DatabaseLogID";
            }
            return hfSortByColumn.Value;
        }

        private void ToggleSortType()
        {
            sortType = GetPersistedSortType();
            if (sortType == SortType.ASC)
            {
                sortType = SortType.DESC;
            }
            else
            {
                sortType = SortType.ASC;
            }
        }

        private void UpdateSorting(string columnName)
        {
            if (SortColumnName() != columnName)
            {
                sortType = SortType.ASC;
                hfSortByColumn.Value = columnName;
            }
            else
            {
                ToggleSortType();
            }

            hfSortDirection.Value = sortType.ToString();
        }
    }
}