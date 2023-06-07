using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("[controller]")]
    //[Authorize]
    public class UserDataController : ControllerBase
    {
        public UserDataController(Database db)
        {
            Db = db;
        }

        // GET ONE user review
        [HttpGet]
        public async Task<IActionResult> GetOne()
        {
            await Db.Connection.OpenAsync();
            var query = new UserData(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        } 

        // GET ALL user reviews from one user
        [HttpGet("{user_review}")]
        public async Task<IActionResult> GetAll(int user_review)
        {
            await Db.Connection.OpenAsync();
            var query = new UserData(Db);
            var results = await query.FindAllAsync(user_review);
            if (results.Count == 0)
                return new NotFoundResult();
            return new OkObjectResult(results);
        }


        //POST new user review
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserData body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            int result=await body.InsertAsync();
            return new OkObjectResult(result);
        }

        //PUT existing user review (update)
        [HttpPut("{user_review}")]
        public async Task<IActionResult> PutOne(int user_review, [FromBody]UserData body)
        {
            await Db.Connection.OpenAsync();
            var query = new UserData(Db);
            var result = await query.FindOneAsync(user_review);
            if (result is null)
                return new NotFoundResult();
            result.review_date = body.review_date;
            result.user_review = body.user_review;
            result.book_review = body.book_review;
            result.rating = body.rating;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE review
        [HttpDelete("{id_review}")]
        public async Task<IActionResult> DeleteOne(int id_review)
        {
            await Db.Connection.OpenAsync();
            var query = new UserData(Db);
            var result = await query.FindOneAsyncByID(id_review);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkObjectResult(result);
        }

        

        public Database Db { get; }
    }
}