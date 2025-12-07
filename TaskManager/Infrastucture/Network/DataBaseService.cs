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
        private static readonly string _host = "5.129.192.166";
        private static readonly Uri _uri = new Uri($"http://{_host}/rmpPhpApi/api/");
        private static HttpClient client = new HttpClient();
        
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
    }
}
