namespace Models
{
    public class DatabaseLog
	{
	    public int DatabaseLogID { get; set; }
	    public string DatabaseUser { get; set; }
	    public string Event { get; set; }
	    public string Object { get; set; }
	    public string Schema { get; set; }
	    public string TSQL { get; set; }
	}
}
