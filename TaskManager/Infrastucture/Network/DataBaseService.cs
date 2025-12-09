using System;
using System.Collections.Generic;
using System.Net.Http;
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

                // deserialize response
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    JsonNode responseRootJson;
                    // getting root JsonNode from httpResponseMessage
                    try { responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync()); }
                    catch (Exception) { throw new Exception("DataBaseService error \"getting clear JsonNode from httpResponseMessage\""); }
                    
                    // checking and return result
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
                else throw new Exception("error on DataBaseService \"AuthorizeUser()\"\nhttp code = " + httpResponseMessage.StatusCode);
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
        public static async Task<List<TaskHumanReadable>> GetTasks()
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
    }
}
