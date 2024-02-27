using BCrypt.Net;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecurePassword.Context;
using SecurePassword.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SecurePassword.Models.DbReturns;

namespace SecurePassword.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyContext _context;
        public AuthController(MyContext context)
        {
            _context = context;
        }

        [HttpPost("CreateLogin")]
        public async Task<ActionResult> CreateLogin(CreateLoginDTO req)
        {
            try
            {
                req.Password = BCrypt.Net.BCrypt.HashPassword(req.Password);

                var affectedRows = await _context.Database
                    .ExecuteSqlInterpolatedAsync($"EXEC CreateLogin {req.Username}, {req.Password}");

                if (affectedRows == 1)
                {
                    await _context.SaveChangesAsync();
                    return Ok("User Created");
                }
                else
                {
                    return NotFound("Login Not Created: " + affectedRows);
                }
            }
            catch (SqlException sqlEx)
            {
                return BadRequest("Sql Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginDbReturn>> Login(LoginDTO req)
        {
            try
            {
                string wrongLogin = "Incorrect Username or Password";

                if (req.Username != null)
                {
                    var returnList = await _context.LoginDbReturn
                        .FromSqlRaw("EXEC GetLogin @Username", new SqlParameter("@Username", req.Username))
                        .ToListAsync();

                    var loginReturn = returnList.FirstOrDefault();


                    if (loginReturn == null)
                    {
                        return BadRequest(wrongLogin);
                    }


                    if (!BCrypt.Net.BCrypt.Verify(req.Password, loginReturn.HashedPassword))
                    {
                        return BadRequest(wrongLogin);
                    }

                   
                    string hashed = loginReturn.HashedPassword; // Just to Show The Result
                    string username = req.Username; // Just to Show The Result
                    string password = req.Password; // Just to Show The Result
                    return Ok(new { username, password, hashed }); // Would normally just send a "User Successfully Logged In" Message
                }
                else
                {
                    return BadRequest(wrongLogin);
                }
            }
            catch (SqlException sqlEx)
            {
                return BadRequest("Sql Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }
    }
}
