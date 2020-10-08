namespace TodoistSync.Models
{
    public class TodoistResponse<T>
        where T : class
    {
        public bool FullSync { get; set; }

        public string SyncToken { get; set; } = string.Empty;
        
        public TempIdMapping TempIdMapping { get; set; }

        public T? Item { get; set; }
    }

    public struct TempIdMapping
    {
        
    }
}