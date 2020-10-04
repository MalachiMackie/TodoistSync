namespace TodoistSync.Models
{
    public class TodoistResponse
    {
        public bool FullSync { get; set; }

        public string SyncToken { get; set; } = string.Empty;
        
        public TempIdMapping TempIdMapping { get; set; }
    }

    public struct TempIdMapping
    {
        
    }
}