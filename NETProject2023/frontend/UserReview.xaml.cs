using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// Interaction logic for UserReview.xaml
    /// </summary>
    public partial class UserReview : Window
    {
        //variables
        public string Username { get; set; }
        public string Token { get; set; }
        public string token { get; private set; }

        //initialise and add background
        public UserReview()
        {
            InitializeComponent();
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                Color.FromRgb(234, 191, 188),
                Color.FromRgb(190, 248, 252),
                new Point(0.5, 0),
                new Point(0.5, 1));
            Background = gradientBrush;
        }

        //get user reviews from API using the username
        static async Task<string> GetUserReviewsFromApi(string Token, string Username)
        {
            var response = string.Empty;
            var url = "http://localhost:5192/user/" + Username;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Token);
            HttpResponseMessage result = await client.GetAsync(url);
            response = await result.Content.ReadAsStringAsync();
            return response;
        }

        //get user reviews from API using the ID
        static async Task<string> GetUserReviewByIdFromApi(string Token, string UserId)
        {
            var response = string.Empty;
            var userDataUrl = "http://localhost:5192/UserData/" + UserId;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Token);
            HttpResponseMessage result = await client.GetAsync(userDataUrl);
            response = await result.Content.ReadAsStringAsync();
            return response;
        }

        //handler for adding a review
        private async void btnAddReview_Click(object sender, RoutedEventArgs e)
        {
            string date = newReviewDate.Text;
            string bookId = newBookID.Text;
            string rating = newRating.Text;

            // Validate input values
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(bookId) || string.IsNullOrEmpty(rating))
            {
                MessageBox.Show("Please enter all the required fields.");
                return;
            }

            try
            {
                var url = "http://localhost:5192/UserData";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", Token);

                //get user data from API
                var data = Task.Run(() => GetUserReviewsFromApi(Token, Username));
                data.Wait();
                Console.WriteLine(data.Result);

                //get id_user
                JObject user = JObject.Parse(data.Result);
                var user_review_id = user["id_user"].ToString();

                // Create the review object
                JObject reviewObject = new JObject();
                reviewObject["id_review"] = 0;
                reviewObject["review_date"] = date;
                reviewObject["user_review"] = user_review_id; // Use the current user's ID
                reviewObject["book_review"] = Convert.ToInt32(bookId);
                reviewObject["rating"] = Convert.ToInt32(rating);

                // Convert the review object to JSON string
                string reviewJson = reviewObject.ToString();

                // Send POST request to the API
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(reviewJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Review added successfully!");

                // Clear the text boxes
                newReviewDate.Text = string.Empty;
                newBookID.Text = string.Empty;
                newRating.Text = string.Empty;
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void btnDeleteReview(object sender, RoutedEventArgs e)
        {
            string reviewId = delReviewID.Text;
            int convertedReviewId;

            if (int.TryParse(reviewId, out convertedReviewId))
            {
                // Conversion succeeded
            }

            // Validate input value
            if (convertedReviewId == 0)
            {
                MessageBox.Show("Please enter the review ID.");
                return;
            }

            try
            {
                var url = "http://localhost:5192/UserData/" + convertedReviewId;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", Token);

                // Send DELETE request to the API
                HttpResponseMessage response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Review deleted successfully!");

                    // Clear the textbox
                    delReviewID.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Failed to delete the review. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }





        public void ShowMyReviews()
        {
            //get user data from API
            var data = Task.Run(() => GetUserReviewsFromApi(Token, Username)); 
            data.Wait();
            Console.WriteLine(data.Result);

            if (data.Result.Length > 3) 
            {
                //get id_user
                JObject user = JObject.Parse(data.Result);
                var userReviewID = user["id_user"].ToString();

                //use as user_review
                var userDataById = Task.Run(() => GetUserReviewByIdFromApi(Token, userReviewID));
                userDataById.Wait();
                Console.WriteLine(userDataById.Result);

                if (userDataById.Result.Length > 3)
                { 
                    //parse JSON response to Array
                    JArray jsonArray = JArray.Parse(userDataById.Result);

                    // Create a list to hold the data
                    List<MyReviewData> reviewDataList = new List<MyReviewData>();

                    foreach (JObject j in jsonArray)
                    {
                        //create an instance of MyReviewData to hold the data for each row
                        MyReviewData reviewData = new MyReviewData();
                        reviewData.id_review = j["id_review"].ToString();
                        reviewData.review_date = j["review_date"].ToString();
                        reviewData.user_review = j["user_review"].ToString();
                        reviewData.book_review = j["book_review"].ToString();
                        reviewData.rating = j["rating"].ToString();

                        //add data to list
                        reviewDataList.Add(reviewData);
                    }

                    //display data in DataGrid
                    gridMyReviews.ItemsSource = reviewDataList;
                }

                else
                {
                    //if user_review is not found
                    MessageBox.Show("No user data found!");
                }
            }
            else
            {
                //if id_user is not found
                MessageBox.Show("No user under this username!");
            }
        }
    }
}
