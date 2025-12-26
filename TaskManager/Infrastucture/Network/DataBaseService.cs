using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Model;

namespace TaskManager.Infrastucture.Network
{
    public static class DataBaseService
    {
        //fields
        private static readonly string _host = "5.129.192.166";
        private static readonly Uri _uri = new Uri($"http://{_host}/rmpPhpApi/api/");
        private static HttpClient client = new HttpClient();
        //methods
        public static async Task<UserResponse> AuthorizeUser(UserRequest userRequestObj)
        {
            if (!String.IsNullOrEmpty(userRequestObj.username))
            {
                StringContent content = new StringContent(
                    JsonSerializer.Serialize(userRequestObj),
                    Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await client.PostAsync(_uri + "login/login.php", content);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    JsonNode responseRootJson;
                    try { responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync()); }
                    catch (Exception) { throw new Exception("DataBaseService error \"getting clear JsonNode from httpResponseMessage\""); }

                    if (responseRootJson["success"].GetValue<bool>())
                    {
                        JsonObject responseUserJson = responseRootJson["user"].AsObject();
                        try
                        {
                            return new UserResponse
                            {
                                username = responseUserJson["username"].GetValue<string>(),
                                idRole = (int)responseUserJson["idRole"].GetValue<int>()
                            };
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"DataBaseService error: \"getting UserResponse from responseUserJson\"\nMessage = {ex.Message}");
                        }
                    }
                    else
                    {
                        throw new Exception("Неверный логин или пароль!");
                    }
                }
                else throw new Exception("DataBaseService error\"AuthorizeUser()\"\nhttp code = " + httpResponseMessage.StatusCode);
            }
            throw new Exception("userObj.username is null or empty");
        }
        public static async Task<List<Category>> GetCategories()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(_uri + "getTable/getScopes.php");

                JsonNode responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                JsonArray responseCategoriesJson = responseRootJson["data"].AsArray();

                List<Category> categories = responseCategoriesJson.Deserialize<List<Category>>();
                return categories ?? throw new Exception("response was null");
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public static async Task<List<TaskHumanReadable>> GetTasksHumanReadable()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(_uri + "getTable/getTasks-HumanReadable.php");

                JsonNode responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                // subarray with data
                JsonArray responseDataJson = responseRootJson["data"].AsArray();

                List<TaskHumanReadable> responseTasks = responseDataJson.Deserialize<List<TaskHumanReadable>>();
                return responseTasks ?? throw new Exception("response was null");
            }
            catch (Exception e)
            {
                throw new Exception($"DataBaseService Exception Occured!\nMessage = {e.Message}");
            }
        }
        public static async Task<List<Model.Task>> GetTasks()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(_uri + "getTable/getTasks.php");

                JsonNode responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                // subarray with data
                JsonArray responseDataJson = responseRootJson["data"].AsArray();

                List<Model.Task> responseTasks = responseDataJson.Deserialize<List<Model.Task>>();
                return responseTasks ?? throw new Exception("response was null");
            }
            catch (Exception e)
            {
                throw new Exception($"DataBaseService Exception Occured!\nMessage = {e.Message}");
            }
        }
        public static async Task<bool> DeleteFromTableById(string tableName, int id)
        {
            var data = new Dictionary<string, object>
            {
                ["id"] = id.ToString(),
                ["table_name"] = tableName
            };
            string requestJson = JsonSerializer.Serialize(data);

            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "generics/deleteFromTableById.php", new StringContent(requestJson, Encoding.UTF8, "application/json"));
            JsonNode responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());

            return responseRootJson["success"].GetValue<bool>();
        }
        public static async Task<bool> PutScope(string name)
        {
            var data = new Dictionary<string, string>
            {
                ["name"] = name
            };
            string requestJson = JsonSerializer.Serialize(data);

            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "scope/putScope.php", new StringContent(requestJson, Encoding.UTF8, "application/json"));
            JsonNode responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());

            return responseRootJson["success"].GetValue<bool>();
        }
        public static async Task<bool> UpdateFieldFromTableById(string tableName, string fieldName, string fieldNewValue, int id)
        {
            var data = new Dictionary<string, string>
            {
                ["table_name"] = tableName,
                ["field_name"] = fieldName,
                ["field_new_value"] = fieldNewValue,
                ["id"] = id.ToString()
            };
            string requestJson = JsonSerializer.Serialize(data);

            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "generics/updateFieldFromTableById.php", new StringContent(requestJson, Encoding.UTF8, "application/json"));
            JsonNode responseJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());

            return responseJson["success"].GetValue<bool>();
        }
        public static async Task<List<User>> GetUsers()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(_uri + "getTable/getUsers.php");

                JsonNode responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                JsonArray responseDataJson = responseRootJson["data"].AsArray();
                List<User> responseTasks = responseDataJson.Deserialize<List<User>>();
                return responseTasks ?? throw new Exception("response was null");
            }
            catch (Exception e)
            {
                throw new Exception($"DataBaseService Exception Occured!\nMessage = {e.Message}");
            }
        }
        public static async Task<bool> PutUser(User user, string password)
        {
            var data = new Dictionary<string, string>
            {
                ["username"] = user.Username,
                ["lname"] = user.Lname,
                ["idRole"] = user.IdRole.ToString(),
                ["password"] = password
            };
            string requestJson = JsonSerializer.Serialize(data);
            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "user/putUser.php", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            JsonNode responseJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
            
            return responseJson["success"].GetValue<bool>();
        }
        public static async Task<bool> UpdateUserById(User user, string password) // password transmitted separately for provide security
        {
            var data = new Dictionary<string, string>
            {
                ["id"] = user.Id.ToString(),
                ["login"] = user.Username,
                ["lname"] = user.Lname,
                ["idRole"] = user.IdRole.ToString(),
                ["password"] = password
            };
            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "user/updateUser.php", content);

            JsonNode responseJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
            return responseJson["success"].GetValue<bool>();
        }
        public static async Task<string> GetPasswordById(int id)
        {
            var data = new Dictionary<string, string>
            {
                ["id"] = id.ToString()
            };
            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "login/getPassword.php", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                JsonNode responseJsonRoot = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());

                if (responseJsonRoot["success"].GetValue<bool>())
                {
                    JsonObject responseJsonObject = responseJsonRoot["data"].AsObject();
                    return responseJsonObject["password"].GetValue<string>();
                }
                else throw new Exception("response success is false");
            }
            else throw new Exception("response http code is not 200");
        }
        public static async Task<User> GetUserByLname(string lname)
        {
            var data = new Dictionary<string, string>
            {
                ["lname"] = lname
            };
            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "user/getUserByLname.php", content);

            JsonNode responseJsonRoot = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
            if (responseJsonRoot["success"].GetValue<bool>())
            {
                JsonObject jsonObjectUser = responseJsonRoot["data"].AsObject();
                return new User
                {
                    Id = jsonObjectUser["id"].GetValue<int>(),
                    Username = jsonObjectUser["username"].GetValue<string>(),
                    Lname = jsonObjectUser["lname"].GetValue<string>(),
                    IdRole = jsonObjectUser["idRole"].GetValue<int>()
                };
            }
            else throw new Exception("http response is not 200");
        }
        public static async Task<User> GetUserByUsername(string username)
        {
            var data = new Dictionary<string, string>
            {
                ["username"] = username
            };
            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "user/getUserByUsername.php", content);

            JsonNode responseJsonRoot = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
            if (responseJsonRoot["success"].GetValue<bool>())
            {
                JsonObject jsonObjectUser = responseJsonRoot["data"].AsObject();
                return new User
                {
                    Id = jsonObjectUser["id"].GetValue<int>(),
                    Username = jsonObjectUser["username"].GetValue<string>(),
                    Lname = jsonObjectUser["lname"].GetValue<string>(),
                    IdRole = jsonObjectUser["idRole"].GetValue<int>()
                };
            }
            else throw new Exception("http response is not 200");
        }
        public static async Task<bool> UpdateTaskById(int id, Model.Task newTask)
        {
            var data = new Dictionary<string, string>
            {
                ["id"] = id.ToString(),
                ["id_owner"] = newTask.IdOwner.ToString(),
                ["id_scope"] = newTask.IdScope.ToString(),
                ["title"] = newTask.Title,
                ["since"] = newTask.Since.ToString("yyyy-MM-dd"),
                ["deadline"] = newTask.Deadine.ToString("yyyy-MM-dd")
            };

            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "task/updateTask.php", content);

            JsonNode responseJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());

            return responseJson["success"].GetValue<bool>();
        }
        public static async Task<bool> PutTask(Model.Task task)
        {
            var data = new Dictionary<string, string>
            {
                ["idOwner"] = task.IdOwner.ToString(),
                ["idStatus"] = task.IdStatus.ToString(),
                ["title"] = task.Title,
                ["idScope"] = task.IdScope.ToString(),
                ["since"] = task.Since.ToString("yyyy-MM-dd"),
                ["deadline"] = task.Deadine.ToString("yyyy-MM-dd")
            };

            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PutAsync(_uri + "task/putTask.php", content);

            JsonNode responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
            return responseRootJson["success"].GetValue<bool>();
        }
    }
}
