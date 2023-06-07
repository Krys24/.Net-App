using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        public BookController(Database db)
        {
            Db = db;
        }

        // GET all books
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        }

        // GET one book
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        // POST new book
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Book body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            int result=await body.InsertAsync();
            return new OkObjectResult(result);
        }

        // PUT api/Book/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOne(int id, [FromBody]Book body)
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.title = body.title;
            result.author = body.author;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE api/Book/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkObjectResult(result);
        }

        public Database Db { get; }
    }
}