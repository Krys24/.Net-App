using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net;

namespace frontend
{
    /// <summary>
    /// Interaction logic for UserData.xaml
    /// </summary>
    public partial class UserData : Window
    {
        //declare variables 
        public string Token { get; set; }
        public string Username { get; set; }

        //initialise UserData and add background
        public UserData()
        {
            InitializeComponent();
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                Color.FromRgb(234, 191, 188),
                Color.FromRgb(190, 248, 252),
                new Point(0.5, 0),
                new Point(0.5, 1));
            Background = gradientBrush;
        }

        //GET user information from API, to display in the grid
        static async Task<string> GetUserDataFromApi(string Token, string Username)
        {
            var response = string.Empty;
            var url = "http://localhost:5192/user/" + Username;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Token);
            HttpResponseMessage result = await client.GetAsync(url);
            response = await result.Content.ReadAsStringAsync();
            return response;
        }
        public void ShowMyData()
        {
            //error check for login
            if (Token == null)
            {
                MessageBox.Show("You have to login first");
            }

            //populate the grid
            else
            {
                var data = Task.Run(() => GetUserDataFromApi(Token, Username));
                data.Wait();
                Console.WriteLine(data.Result);
                if (data.Result.Length > 3)
                {
                    JObject j = JObject.Parse(data.Result);
                    tbUsername.Text = "Username: " + j["username"].ToString();
                    tbFname.Text = "First name: " + j["fname"].ToString();
                    tbLname.Text = "Last name: " + j["lname"].ToString();
                    tbAddress.Text = "Address: " + j["address"].ToString();
                }
                else
                {
                    MessageBox.Show("There is no data");
                }
            }
        }

        //define update button event handler
        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            //Create a User object with the updated values
            //password here causes error, may need to be decyphered or encrypted first
            UpdatedUser updatedUser = new UpdatedUser
            {
                username = updatedUsername.Text,
                fname = updatedFname.Text,
                lname = updatedLname.Text,
                password = updatedPassword.Text,
                address = updatedAddress.Text
            };

            var client = new HttpClient();

            // Convert the User object to JSON
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(updatedUser);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Construct the API URL with the username
                string apiUrl = "http://localhost:5192/user/" + updatedUser.username;

                // Send the PUT request to the API
                HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // User details updated successfully
                    MessageBox.Show("User details updated!");
                }
                else
                {
                    // Handle error case
                    MessageBox.Show("Failed to update user details. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        //DELETE button handler
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();

            try
            {
                // Construct the API URL with the username
                string apiUrl = "http://localhost:5192/User/" + Username;

                // Send the DELETE request to the API
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Account deletion successful
                    MessageBox.Show("Account deleted successfully.");
                }
                else
                {
                    // Handle error case
                    MessageBox.Show("Failed to delete the account. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

    }
}

