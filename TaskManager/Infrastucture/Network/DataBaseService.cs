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

        public static HttpClient client = new HttpClient();
        
        public static async Task<UserResponse> AuthorizeUser(UserRequest userRequestObj)
        {
            UserResponse userResponseObj = new UserResponse();

            if (!String.IsNullOrEmpty(userRequestObj.username))
            {
                // dev only 
                userRequestObj = new UserRequest
                {
                    username = "sanyament",
                    password = "chachatop"
                };
                
                
                // serialize request object
                StringContent content = new StringContent(JsonSerializer.Serialize(userRequestObj), Encoding.UTF8, "application/json");

                // dev only
                string reqCon = await content.ReadAsStringAsync();
                MessageBox.Show("StringContent (request) = " + reqCon, "info");
                
                // flush request
                HttpResponseMessage httpResponseMessage = await client.PostAsync(_uri + "login/login.php", content);

                // deserialize response
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    JsonNode responseRootJson;
                    // getting root JsonNode from httpResponseMessage
                    try
                    {
                        responseRootJson = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("DataBaseService error \"getting clear JsonNode from httpResponseMessage\"");
                    }
                    // getting JsonNode subarray user from responseRootJson
                    JsonObject responseUserJson = responseRootJson["user"].AsObject();
                    // getting UserResponse from responseUserJson
                    try
                    {
                    userResponseObj.username = responseUserJson["username"].GetValue<string>();
                    userResponseObj.idRole = (int)responseUserJson["idRole"].GetValue<int>();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"DataBaseService error: \"getting UserResponse from responseUserJson\"\nMessage = {ex.Message}");
                    }
                    return userResponseObj;
                }
                else
                {
                    throw new Exception("error on DataBaseService \"AuthorizeUser()\"\nhttp code = " + httpResponseMessage.StatusCode);
                }
            }
            throw new Exception("userObj.username is null or empty");
        }
    }
}
