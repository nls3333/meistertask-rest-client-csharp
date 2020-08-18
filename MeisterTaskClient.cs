using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MeisterTask
{
    class MeisterTaskClient
    {
        private static readonly string CREDS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\cred-meistertask.txt";
        private static readonly string API_ROOT = "https://www.meistertask.com/api/";
        private static readonly int PAGE_SIZE = 100;
        private static readonly int CACHE_TTL_SECONDS = 30;

        private static MeisterTaskClient instance;

        private readonly HttpClient client;
        private readonly Dictionary<string, CacheItem> cache;

        public MeisterTaskClient()
        {
            if (!File.Exists(CREDS))
            {
                throw new Exception("No MeisterTask credentials file found!");
            }
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", File.ReadAllText(CREDS).Trim());
            cache = new Dictionary<string, CacheItem>();
        }

        public List<Project> GetAllProjects()
        {
            List<Project> projects = new List<Project>();
            foreach (var project in GetJsonPaginated("projects"))
            {
                projects.Add(new Project(project));
            }
            return projects;
        }

        public Project GetProject(long id)
        {
            return new Project(GetJsonSingle("projects/" + id));
        }

        public List<Project> SearchProjects(string search)
        {
            List<Project> foundProjects = new List<Project>();
            foreach (Project project in GetAllProjects())
            {
                if (project.Name.Contains(search))
                {
                    foundProjects.Add(project);
                }
            }
            return foundProjects;
        }

        public List<Task> GetAllTasks()
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in GetJsonPaginated("tasks"))
            {
                tasks.Add(new Task(task));
            }
            return tasks;
        }

        public List<Task> GetAllTasksInProject(Project project)
        {
            return GetAllTasksInProject(project.Id);
        }

        public List<Task> GetAllTasksInProject(long projectId)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in GetJsonPaginated("projects/" + projectId + "/tasks"))
            {
                tasks.Add(new Task(task));
            }
            return tasks;
        }

        public List<Task> GetAllTasksInSection(Section section)
        {
            return GetAllTasksInSection(section.Id);
        }

        public List<Task> GetAllTasksInSection(long sectionId)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in GetJsonPaginated("sections/" + sectionId + "/tasks"))
            {
                tasks.Add(new Task(task));
            }
            return tasks;
        }

        public Task GetTask(long id)
        {
            return new Task(GetJsonSingle("tasks/" + id));
        }

        public List<Task> SearchTasks(string search)
        {
            return SearchTasks(search, GetAllTasks());
        }

        public List<Task> SearchTasksInProject(string search, long projectId)
        {
            return SearchTasks(search, GetAllTasksInProject(projectId));
        }

        public List<Task> SearchTasksInSection(string search, long sectionId)
        {
            return SearchTasks(search, GetAllTasksInSection(sectionId));
        }

        private List<Task> SearchTasks(string search, List<Task> all)
        {
            List<Task> foundTasks = new List<Task>();
            foreach (Task task in all)
            {
                if (task.Name.Contains(search))
                {
                    foundTasks.Add(task);
                }
            }
            return foundTasks;
        }

        public List<Comment> GetAllCommentsInTask(Task task)
        {
            return GetAllCommentsInTask(task.Id);
        }

        public List<Comment> GetAllCommentsInTask(long taskId)
        {
            List<Comment> comments = new List<Comment>();
            foreach (var comment in GetJsonPaginated("tasks/" + taskId + "/comments"))
            {
                comments.Add(new Comment(comment));
            }
            return comments;
        }

        public Comment GetComment(long id)
        {
            return new Comment(GetJsonSingle("comments/" + id));
        }

        public List<Person> GetAllPersons()
        {
            List<Person> persons = new List<Person>();
            foreach (var person in GetJsonPaginated("persons"))
            {
                persons.Add(new Person(person));
            }
            return persons;
        }

        public Person GetPerson(long id)
        {
            return new Person(GetJsonSingle("persons/" + id));
        }

        public List<Person> SearchPerson(string search)
        {
            List<Person> foundPersons = new List<Person>();
            foreach (Person person in GetAllPersons())
            {
                if (person.FullName.Contains(search))
                {
                    foundPersons.Add(person);
                }
            }
            return foundPersons;
        }

        public List<Section> GetAllSectionsInProject(Project project)
        {
            return GetAllSectionsInProject(project.Id);
        }

        public List<Section> GetAllSectionsInProject(long projectId)
        {
            List<Section> sections = new List<Section>();
            foreach (var section in GetJsonPaginated("projects/" + projectId + "/sections"))
            {
                sections.Add(new Section(section));
            }
            return sections;
        }

        public Section GetSection(long id)
        {
            return new Section(GetJsonSingle("sections/" + id));
        }

        public List<Section> SearchSectionsInProject(string search, long projectId)
        {
            List<Section> foundSections = new List<Section>();
            foreach (Section section in GetAllSectionsInProject(projectId))
            {
                if (section.Name.Contains(search))
                {
                    foundSections.Add(section);
                }
            }
            return foundSections;
        }

        public Task UpdateTaskAssignee(Task task, Person person)
        {
            return UpdateTaskAssignee(task.Id, person.Id);
        }

        public Task UpdateTaskAssignee(long taskId, long personId)
        {
            return new Task(PutJson("tasks/" + taskId, "{\"assigned_to_id\": " + personId + "}"));
        }

        public Task UpdateTaskMoveToSection(Task task, Section section)
        {
            return UpdateTaskMoveToSection(task.Id, section.Id);
        }

        public Task UpdateTaskMoveToSection(long taskId, long sectionId)
        {
            return new Task(PutJson("tasks/" + taskId, "{\"section_id\": " + sectionId + "}"));
        }

        public Comment UpdateTaskPostComment(Task task, string text)
        {
            return UpdateTaskPostComment(task.Id, text);
        }

        public Comment UpdateTaskPostComment(long taskId, string text)
        {
            string escaped = text.Replace("\"", "\\\"");
            return new Comment(PostJson("tasks/" + taskId + "/comments", "{\"text\": \"" + escaped + "\"}"));
        }

        public Task CreateTask(Section section, string name, string notes = "")
        {
            return CreateTask(section.Id, name, notes);
        }

        public Task CreateTask(long sectionId, string name, string notes = "")
        {
            string json = "{\"name\": \"" + name + "\", \"notes\": \"" + notes + "\"}";
            return new Task(PostJson("sections/" + sectionId + "/tasks", json));
        }

        private HttpResponseMessage Get(string url)
        {
            Task<HttpResponseMessage> request = client.GetAsync(API_ROOT + url);
            request.Wait();
            return request.Result;
        }

        private HttpResponseMessage Post(string url, string body)
        {
            StringContent content = new StringContent(body, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> request = client.PostAsync(API_ROOT + url, content);
            request.Wait();
            return request.Result;
        }

        private HttpResponseMessage Put(string url, string body)
        {
            StringContent content = new StringContent(body, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> request = client.PutAsync(API_ROOT + url, content);
            request.Wait();
            return request.Result;
        }

        private string ReadResponse(HttpResponseMessage response)
        {
            Task<string> readContent = response.Content.ReadAsStringAsync();
            readContent.Wait();
            return readContent.Result;
        }

        private dynamic PutJson(string url, string content)
        {
            try
            {
                HttpResponseMessage response = Put(url, content);
                response.EnsureSuccessStatusCode();
                cache.Clear();
                return Json.Decode(ReadResponse(response));
            }
            catch (Exception e)
            {
                throw new Exception("Request for " + url + " failed! " + e.Message);
            }
        }

        private dynamic PostJson(string url, string content)
        {
            try
            {
                HttpResponseMessage response = Post(url, content);
                response.EnsureSuccessStatusCode();
                cache.Clear();
                return Json.Decode(ReadResponse(response));
            }
            catch (Exception e)
            {
                throw new Exception("Request for " + url + " failed! " + e.Message);
            }
        }

        private dynamic GetCachedSingle(string url)
        {
            if (cache.ContainsKey(url))
            {
                if (cache[url].IsValid())
                {
                    return cache[url].ContentSingle;
                }
                cache.Remove(url);
            }
            return null;
        }

        private List<dynamic> GetCached(string url)
        {
            if (cache.ContainsKey(url))
            {
                if (cache[url].IsValid())
                {
                    return cache[url].Content;
                }
                cache.Remove(url);
            }
            return null;
        }

        private void CacheSingle(string url, dynamic content)
        {
            cache.Add(url, new CacheItem(CACHE_TTL_SECONDS, content));
        }

        private void Cache(string url, List<dynamic> content)
        {
            cache.Add(url, new CacheItem(CACHE_TTL_SECONDS, content));
        }

        private dynamic GetJsonSingle(string url)
        {
            dynamic cached = GetCachedSingle(url);
            if (cached == null)
            {
                try
                {
                    HttpResponseMessage response = Get(url);
                    response.EnsureSuccessStatusCode();
                    dynamic json = Json.Decode(ReadResponse(response));
                    CacheSingle(url, json);
                    return json;
                }
                catch (Exception e)
                {
                    throw new Exception("Request for " + url + " failed! " + e.Message);
                }
            }
            return cached;
        }

        private List<dynamic> GetJsonPaginated(string url)
        {
            List<dynamic> cached = GetCached(url);
            if (cached == null)
            {
                try
                {
                    List<dynamic> items = new List<dynamic>();
                    int page = 1;
                    bool hasNextPage = true;
                    while (hasNextPage)
                    {
                        HttpResponseMessage response = Get(url + "?items=" + PAGE_SIZE + "&page=" + page);
                        response.EnsureSuccessStatusCode();

                        var itemsOnPage = Json.Decode(ReadResponse(response));
                        hasNextPage = itemsOnPage.Length == PAGE_SIZE;
                        page++;

                        foreach (var item in itemsOnPage)
                        {
                            items.Add(item);
                        }
                    }
                    Cache(url, items);
                    return items;
                }
                catch (Exception e)
                {
                    throw new Exception("Request for " + url + " failed! " + e.Message);
                }
            }
            return cached;
        }

        public static MeisterTaskClient GetInstance()
        {
            if (instance == null)
            {
                instance = new MeisterTaskClient();
            }
            return instance;
        }
    }
}
