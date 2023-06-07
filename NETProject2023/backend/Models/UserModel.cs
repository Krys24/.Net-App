using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace backend
{
    public class User
    {
        public int id_user { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string ? address { get; set; }
        internal Database Db { get; set; }

        public User()
        {
        }

        internal User(Database db)
        {
            Db = db;
        }

        public async Task<List<User>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  User ;";
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<User> FindOneAsync(int id_User)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  User  WHERE  id_User  = @id_User";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_User",
                DbType = DbType.Int32,
                Value = id_User,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }


        public async Task<User> FindByUsername(string username)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  User  WHERE  username  = @username";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@username",
                DbType = DbType.String,
                Value = username,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }



        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  User ";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }
    

        public async Task<int> InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO  User  ( fname, lname, username, password, address ) VALUES (@fname, @lname, @username, @password, @address);";
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
            cmd.CommandText = @"UPDATE  User  SET  fname = @fname, lname = @lname, username  = @username,
            password  = @password, address = @address WHERE  id_user  = @id_User;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  User  WHERE  username  = @username;";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<List<User>> ReturnAllAsync(DbDataReader reader)
        {
            var posts = new List<User>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new User(Db)
                    {
                        id_user = reader.GetInt32(0),
                        fname = reader.GetString(1),
                        lname = reader.GetString(2),
                        username = reader.GetString(3),
                        password = reader.GetString(4),
                        address = reader.GetString(5),
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
                ParameterName = "@id_user",
                DbType = DbType.Int32,
                Value = id_user,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@fname",
                DbType = DbType.String,
                Value = fname,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@lname",
                DbType = DbType.String,
                Value = lname,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@username",
                DbType = DbType.String,
                Value = username,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@password",
                DbType = DbType.String,
                Value = password,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@address",
                DbType = DbType.String,
                Value = address,
            });

        }
    }
}