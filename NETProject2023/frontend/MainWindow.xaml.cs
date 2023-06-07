using frontend;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Windows.Media;

namespace frontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                Color.FromRgb(234, 191, 188),
                Color.FromRgb(190, 248, 252),
                new Point(0.5, 0),
                new Point(0.5, 1));
            Background = gradientBrush;
        }

        private string token = null;
        static async Task<string> LoginHttp(string u, string p)
        {

            var response = string.Empty;

            User objectUser = new User(u, p);

            var json = JsonConvert.SerializeObject(objectUser);
            var postData = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5192/login/";

            var client = new HttpClient();
            try
            {
                HttpResponseMessage result = await client.PostAsync(url, postData);
                response = await result.Content.ReadAsStringAsync();
                return response;
            }
            catch (Exception)
            {
                return "-1";
            }

        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string u = TextUsername.Text;
            string p = TextPassword.Text;

            var data = Task.Run(() => LoginHttp(u, p));
            data.Wait();

            string response = data.Result;
            if (string.Compare(response, "-1") == 0)
            {
                MessageBox.Show("No connection to server");
            }
            else
            {
                if (string.Compare(response, "0") == 0)
                {
                    MessageBox.Show("No connection to Database");
                }
                else
                {
                    if (string.Compare(response, "false") == 0)
                    {
                        labelResult.Content = "Wrong username/password";
                    }
                    else
                    {
                        labelResult.Content = "Login OK";
                        token = "Bearer " + response;
                        UserInfo userInfo = new UserInfo();
                        userInfo.Token = token;
                        userInfo.Username = u;
                        userInfo.SetUsername(u);
                        userInfo.Show();
                    }
                }
            }
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            // Create a NewUser object from the input values
            NewUser newUser = new NewUser
            {
                fname = NewTextFname.Text,
                lname = NewTextLname.Text,
                username = NewTextUsername.Text,
                password = NewTextPassword.Text,
                address = NewTextAddress.Text
            };

            var client = new HttpClient();

            // Convert the User object to JSON
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(newUser);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the POST request to the API
            HttpResponseMessage response = await client.PostAsync("http://localhost:5192/user", content);

            if (response.IsSuccessStatusCode)
            {
                // User added successfully
                MessageBox.Show("New User Added!");
            }
            else
            {
                // Handle error 
                MessageBox.Show("Failed to add new user. Please try again.");
            }
        }
    }
}