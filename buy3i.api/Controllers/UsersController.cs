using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using buy3i.api.Models;
using System.Data;
using Newtonsoft.Json;

namespace buy3i.api.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class UsersController : ControllerBase
   {
      private readonly UsersDbContext _context;

      public UsersController(UsersDbContext context)
      {
         _context = context;
      }

      // GET: api/Users
      [HttpGet]
      public async Task<ActionResult<IEnumerable<User>>> GetUser()
      {
         return await _context.TBUsers.ToListAsync();
      }

      // GET: api/Users/5
      [HttpGet("{id}")]
      public async Task<ActionResult<User>> GetUser(int id)
      {
         var user = await _context.TBUsers.FindAsync(id);
         if (user == null) return NotFound();

         return user;
      }

      // PUT: api/Users/5
      [HttpPut("{id}")]
      public async Task<bool> PutUser(int id, User user)
      {
         bool response = false;

         try
         {
            if (id != user.id_user) return response;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            response = true;
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!UserExists(id)) return false; else throw;
         }

         return response;
      }

      // POST: api/Users
      [HttpPost]
      public async Task<ActionResult<User>> PostUser(User user)
      {
         user.date_add = DateTime.Now;
         _context.TBUsers.Add(user);
         await _context.SaveChangesAsync();

         return CreatedAtAction("GetUser", new { id = user.id_user }, user);
      }

      // DELETE: api/Users/5
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteUser(int id)
      {
         var user = await _context.TBUsers.FindAsync(id);
         if (user == null) return NotFound();
         _context.TBUsers.Remove(user);
         await _context.SaveChangesAsync();

         return NoContent();
      }

		private bool UserExists(int id)
		{
			return _context.TBUsers.Any(e => e.id_user == id);
		}

		// POST: api/Users
		[HttpPost("GetLogin")]
		public async Task<string> GetLogin()
		{
			string email = HttpContext.Request.Form["email"];
			string password = HttpContext.Request.Form["password"];
			string sql = $"select * from TBUsers where email='{email}' and password='{password}'";
			var dt = new DataTable();
			var jsonString = "";

			using (var command = _context.Database.GetDbConnection().CreateCommand())
			{
				command.CommandText = sql;
				_context.Database.OpenConnection();
				using (var result = command.ExecuteReader())
				{
					dt.Load(result);
					jsonString = JsonConvert.SerializeObject(dt);
				}
			}

			return jsonString;
		}

	}
}
