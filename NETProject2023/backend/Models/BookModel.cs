using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace backend
{
    public class Book
    {
        public int id_book { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        internal Database Db { get; set; }

        public Book()
        {
        }

        internal Book(Database db)
        {
            Db = db;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  book ;";
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<Book> FindOneAsync(int id_book)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  book  WHERE  id_book  = @id_book";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_book",
                DbType = DbType.Int32,
                Value = id_book,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }


        public async Task<int> InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO  book  ( title, author ) VALUES ( @title, @author);";
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
            cmd.CommandText = @"UPDATE  book  SET title = @title, author  = @author WHERE  id_book  = @id_book;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  book  WHERE  id_book  = @id_book;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<List<Book>> ReturnAllAsync(DbDataReader reader)
        {
            var posts = new List<Book>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Book(Db)
                    {
                        id_book = reader.GetInt32(0),
                        title = reader.GetString(1),
                        author = reader.GetString(2),
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
                ParameterName = "@id_book",
                DbType = DbType.Int32,
                Value = id_book,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@title",
                DbType = DbType.String,
                Value = title,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@author",
                DbType = DbType.String,
                Value = author,
            });
        }
    }
}