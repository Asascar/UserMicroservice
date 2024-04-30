using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserMicroservice.Models;
using UserMicroservice.Services;

namespace UserMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var currentUserId = GetUserIdFromToken();
            if (currentUserId != id)
            {
                return Forbid();
            }

            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post(User user)
        {
            _userService.CreateUser(user);
            var token = _userService.GenerateJwtToken(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, new { user, token });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, User user)
        {
            var currentUserId = GetUserIdFromToken();

            if (currentUserId != id)
            {
                return Forbid();
            }

            var existingUser = _userService.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _userService.UpdateUser(id, user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var currentUserId = GetUserIdFromToken();
            if (currentUserId != id)
            {
                return Forbid();
            }

            var existingUser = _userService.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _userService.DeleteUser(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(string username, string Password)
        {
            var authenticatedUser = _userService.Authenticate(username, Password);

            if (authenticatedUser == null)
            {
                return BadRequest(new { message = "Usuário ou senha inválidos" });
            }

            var token = _userService.GenerateJwtToken(authenticatedUser);

            return Ok(new { token });
        }

        [HttpPost("{email}/change-password")]
        public IActionResult ChangePassword(string email, string novaSenha)
        {
            var currentUserId = GetUserIdFromToken();
            var existingUser = _userService.GetUserByEmail(email);

            if (existingUser == null)
            {
                return NotFound();
            }

            if (currentUserId != existingUser.Id)
            {
                return Forbid();
            }

            existingUser.Password = novaSenha;
            _userService.UpdateUser(existingUser.Id, existingUser);
            return Ok(new { message = "Senha alterada com sucesso" });
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return -1;
            }

            return userId;
        }
    }
}
