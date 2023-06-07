using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace frontend
{
    /// <summary>
    /// Interaction logic for UserInfo.xaml
    /// </summary>
    public partial class UserInfo : Window
    {
        //variables
        public string Token { get; set; }
        public string Username { get; set; }
        

        public void SetUsername(string username)
        {
            labelUsername.Content = "Welcome back " + username+"!";
        }
        public UserInfo()
        {
            //initialise and add background
            InitializeComponent();
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                Color.FromRgb(234, 191, 188),
                Color.FromRgb(190, 248, 252),
                new Point(0.5, 0),
                new Point(0.5, 1));
            Background = gradientBrush;
        }

        //Button click handlers
        private void btnMyData_Click(object sender, RoutedEventArgs e)
        {
            UserData userData = new UserData();
            userData.Token = Token;
            userData.Username = Username;
            userData.ShowMyData();
            userData.Show();
        }

        private void btnMyReviews_Click(object sender, RoutedEventArgs e)
        {
            UserReview userReview = new UserReview();
            userReview.Username = Username;
            userReview.Token = Token;
            userReview.ShowMyReviews();
            userReview.Show();
        }

        static async Task<string> GetBooksFromApi()
        {
            var response = string.Empty;
            var url = "http://localhost:5192/Book/";
            var client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            response = await result.Content.ReadAsStringAsync();
            return response;
        }

        private async void btnShowBooks_Click(object sender, RoutedEventArgs e)
        {
            var data = await GetBooksFromApi();
            Console.WriteLine(data);

            if (data.Length > 3)
            {
                dynamic books = JsonConvert.DeserializeObject(data);
                gridBooks.ItemsSource = books;
            }
            else
            {
                MessageBox.Show("There are no books.");
            }
        }
    }
}