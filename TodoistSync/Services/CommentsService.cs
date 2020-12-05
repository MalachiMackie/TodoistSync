using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoistSync.Services
{
    public class Comment
    {
        public string Content { get; set; } = string.Empty;

        public long? ProjectId { get; set; }

        public long? TaskId { get; set; }

        public long? Id { get; set; }
    }

    public interface ICommentsService
    {
        Task<IReadOnlyCollection<Comment>> GetCommentsAsync(long projectId);

        Task CreateComment(Comment comment);
    }

    public class CommentsService : ICommentsService
    {
        private HttpClient HttpClient { get; }

        public CommentsService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<Comment>> GetCommentsAsync(long projectId)
        {
            var response = await HttpClient.GetAsync($"comments?project_id={projectId}");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());
        }

        public async Task CreateComment(Comment comment)
        {
            var content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("comments", content);
            if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(responseContent);
            }
            response.EnsureSuccessStatusCode();
        }
    }
}
