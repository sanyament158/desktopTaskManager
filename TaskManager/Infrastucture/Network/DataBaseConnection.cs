using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Model;

namespace TaskManager.Infrastucture.Network
{
    public static class DataBaseConnection
    {
        private static readonly string _host = "5.129.192.166";
        private static readonly Uri _uri = new Uri($"http://{_host}/rmpPhpApi/api/");

        public static HttpClient client = new HttpClient();
        
        public static async Task<UserResponse> AuthorizeUser(UserRequest userRequestObj)
        {
            UserResponse userResponseObj = new UserResponse();

            if (!String.IsNullOrEmpty(userRequestObj.username))
            {
                // serialize request object
                string jsonRequest = JsonSerializer.Serialize(userRequestObj);
                StringContent content = new StringContent(jsonRequest);
                Console.WriteLine(content);
                
                // flush request
                HttpResponseMessage httpResponseMessage = await client.PostAsync(_uri + "login/login.php", content);
                
                // fetch response
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
                    try
                    {
                        userResponseObj.idRole = 
                        userResponseObj = JsonSerializer.Deserialize<UserResponse>(responseJson);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show(ex.Message, "error");
                    }
                    if (userResponseObj != null) return userResponseObj;
                    else throw new Exception("userResponseObj == null");
                }
            }
            throw new Exception("userObj.username is null or empty");
        }
    }
}
