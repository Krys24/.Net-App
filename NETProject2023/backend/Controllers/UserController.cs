using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;

namespace backend.Controllers
{
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        public UserController(Database db)
        {
            Db = db;
        }

        // GET ALL users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await Db.Connection.OpenAsync();
            var query = new User(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        }

        // GET ONE user
        [HttpGet("{username}")]
        public async Task<IActionResult> GetOne(string username)
        {
            await Db.Connection.OpenAsync();
            var query = new User(Db);
            var result = await query.FindByUsername(username);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        // POST new user
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User body)
        {
            body.password = BCrypt.Net.BCrypt.HashPassword(body.password);
            await Db.Connection.OpenAsync();
            body.Db = Db;
            int result=await body.InsertAsync();
            return new OkObjectResult(result);
        }

        // PUT existing user (update)
        [HttpPut("{username}")]
        public async Task<IActionResult> PutOne(string username, [FromBody]User body)
        {
            await Db.Connection.OpenAsync();
            body.password = BCrypt.Net.BCrypt.HashPassword(body.password);
            var query = new User(Db);
            var result = await query.FindByUsername(username);
            if (result is null)
                return new NotFoundResult();
            result.username = body.username;
            result.fname = body.fname;
            result.lname = body.lname;
            result.password = body.password;
            result.address = body.address;
            //result.password = body.password;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE user
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteOne(string username)
        {
            await Db.Connection.OpenAsync();
            var query = new User(Db);
            var result = await query.FindByUsername(username);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkObjectResult(result);
        }


        public Database Db { get; }
    }
}