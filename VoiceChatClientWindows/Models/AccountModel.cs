using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using VoiceChatClientWindows.Interfaces;

namespace VoiceChatClientWindows.Models
{
    public static class AccountModel
    {
        public static async Task<string> RegisterAsync(IRegistrationForm registrationForm)
        {
            Client.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var registerModel = new
            {
                UserName = registrationForm.UserName,
                Email = registrationForm.Email,
                Password = registrationForm.Password,
                ConfirmPassword = registrationForm.ConfirmPassword
            };

            var response = await Client.client.PostAsJsonAsync("api/Account/Register", registerModel);
            var value = await response.Content.ReadAsStringAsync();
            var obj = new { message = "", ModelState = new Dictionary<string, string[]>() };

            var responseDecode = JsonConvert.DeserializeAnonymousType(value, obj);

            if (!response.IsSuccessStatusCode)
            {
                string errorString = null;

                try
                {
                    foreach (var error in responseDecode.ModelState[""])
                    {
                        errorString += error;
                    }
                }
                catch (Exception)
                {
                    return "Registration form doesn't match";
                }

                return errorString;
            }

            return "Ok";
        }

        public static async Task<UserDataModel> LoginAsync(ILoginForm loginForm)
        {
            var postdata = new List<KeyValuePair<string, string>>();
            postdata.Add(new KeyValuePair<string, string>("grant_type", "password"));
            postdata.Add(new KeyValuePair<string, string>("username", loginForm.UserName));
            postdata.Add(new KeyValuePair<string, string>("password", loginForm.Password));

            Token token = null;

            using (var content = new FormUrlEncodedContent(postdata))
            {
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage tokenResponse = await Client.client.PostAsync("/Token", content);
                token = await tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() });
            }
            
            Client.client.DefaultRequestHeaders.Clear();
            Client.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);

            var loginModel = new
            {
                UserName = loginForm.UserName,
                Password = loginForm.Password
            };

            var response = await Client.client.PostAsJsonAsync("api/Account/Login", loginModel);

            UserDataModel User = null;

            if (response.IsSuccessStatusCode)
            {
                User = await response.Content.ReadAsAsync<UserDataModel>(new[] { new JsonMediaTypeFormatter() });
                Client.Name = User.UserName;
                Client.AuthorizationToken = token.AccessToken;
            }
            return User;
        }
    }
}
