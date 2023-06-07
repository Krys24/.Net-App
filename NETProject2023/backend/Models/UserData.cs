using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;
using System;

namespace backend
{
    public class UserData
    {
        public int id_review { get; set; }
        public string review_date { get; set; }
        public int user_review {get; set;}
        public int book_review {get; set;}
        public int rating {get;set; }
        internal Database Db { get; set; }

        public UserData() 
        {
        }

        internal UserData(Database db)
        {
            Db = db;
        }

        public async Task<List<UserData>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"select * from review;";
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<UserData> FindOneAsync(int user_review) //limited to one review
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  review  WHERE  user_review  = @user_review";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@user_review",
                DbType = DbType.Int32,
                Value = user_review,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<UserData> FindOneAsyncByID(int id_review) //limited to one review
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  review  WHERE  id_review  = @id_review";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_review",
                DbType = DbType.Int32,
                Value = id_review,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<UserData>> FindAllAsync(int user_review) //all reviews from one user
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM review WHERE user_review = @user_review";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@user_review",
                DbType = DbType.Int32,
                Value = user_review,
            });
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }


        public async Task<int> InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO review(review_date, user_review, book_review, rating)
            VALUES(@review_date, @user_review, @book_review, @rating);";
            BindParams(cmd);
            try
            {
                await cmd.ExecuteNonQueryAsync();
                int id_user = (int) cmd.LastInsertedId;
                return id_user; 
            }
            catch (System.Exception)
            {   
                return 0;
            } 
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE  review  SET review_date = @review_date, user_review  = @user_review,
            book_review = @book_review, rating = @rating WHERE  id_review  = @id_review;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  review  WHERE  id_review  = @id_review;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }


        private async Task<List<UserData>> ReturnAllAsync(DbDataReader reader)
        {
            var posts = new List<UserData>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new UserData(Db)
                    {
                        id_review = reader.GetInt32(0),
                        review_date = reader.GetString(1),
                        user_review = reader.GetInt32(2),
                        book_review = reader.GetInt32(3),
                        rating = reader.GetInt32(4)

                    };
                    posts.Add(post);
                }
            }
            return posts;
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_review",
                DbType = DbType.Int32,
                Value = id_review,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@review_date",
                DbType = DbType.String,
                Value = review_date,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@user_review",
                DbType = DbType.Int32,
                Value = user_review,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@book_review",
                DbType = DbType.Int32,
                Value = book_review,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@rating",
                DbType = DbType.Int32,
                Value = rating,
            });
        }

    }
}