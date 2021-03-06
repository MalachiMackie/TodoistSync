﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TodoistSync.Models;

namespace TodoistSync.Services
{
    public class Project
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public long? ParentId { get; set; }

        public IEnumerable<Section> Sections { get; set; } = new List<Section>();

        public IEnumerable<Item> Items { get; set; } = new List<Item>();
    }

    public interface IProjectService
    {
        public Task<Project> GetProjectAsync(long projectId);

        public Task<long> CreateProject(ProjectSettings projectSettings);

        public Task<IEnumerable<Project>> GetAllProjectsAsync();
    }

    public class ProjectService : IProjectService
    {
        private HttpClient HttpClient { get; }

        public ProjectService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<Project> GetProjectAsync(long projectId)
        {
            var response = await HttpClient.GetAsync($"projects/{projectId}");

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Project>(json);
        }

        public async Task<long> CreateProject(ProjectSettings projectSettings)
        {
            var content = new StringContent(JsonConvert.SerializeObject(projectSettings), Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync("https://api.todoist.com/rest/v1/projects", content);

            string responseContent = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
            {
                throw new InvalidOperationException(responseContent);
            }

            response.EnsureSuccessStatusCode();

            Project created = JsonConvert.DeserializeObject<Project>(responseContent);
            return created.Id;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var response = await HttpClient.GetAsync("projects");

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<Project>>(json);
        }
    }
}
