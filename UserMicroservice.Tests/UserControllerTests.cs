using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UserMicroservice.Controllers;
using UserMicroservice.Models;
using UserMicroservice.Repositories;
using UserMicroservice.Services;
using Xunit;

namespace UserMicroservice.Tests
{
    public class UserControllerTests
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _dbContext = DbContextFactory.Create();
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userService = new UserService(_dbContext, _configuration);
            _controller = new UserController(_userService);
        }

        [Fact]
        public void Get_ReturnsAllUsers()
        {
            // Arrange
            var users = GetTestUsers();
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();

            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnedUsers = result.Value as IEnumerable<User>;
            Assert.NotNull(returnedUsers);
            Assert.Equal(users, returnedUsers);
        }

        [Fact]
        public void GetById_ExistingId_ReturnsUser()
        {
            // Arrange
            var users = GetTestUsers();
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
            var userId = 1;
            _controller.ControllerContext = GetControllerContextWithUserId(userId);

            // Act
            var result = _controller.Get(userId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var user = result.Value as User;
            Assert.NotNull(user);
            Assert.Equal(users.First(u => u.Id == userId), user);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var users = GetTestUsers();
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
            var userId = 999;
            _controller.ControllerContext = GetControllerContextWithUserId(userId);

            // Act
            var result = _controller.Get(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Post_ValidUser_ReturnsCreatedResponse()
        {
            // Arrange
            var user = new User { Username = "TestUser", Password = "TestPassword", Email = "teste@teste.com.br" };

            // Act
            var result = _controller.Post(user) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(nameof(_controller.Get), result.ActionName);
            Assert.Equal(user.Id, result.RouteValues["id"]);
        }

        [Fact]
        public void Put_ExistingId_ValidUser_ReturnsNoContent()
        {
            // Arrange
            var users = GetTestUsers();
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
            var userId = 1;
            _controller.ControllerContext = GetControllerContextWithUserId(userId);
            var updatedUser = new User { Id = userId, Username = "UpdatedUser", Password = "UpdatedPassword", Email = "teste@teste.com.br" };

            // Act
            var result = _controller.Put(userId, updatedUser) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public void Put_NonExistingId_ValidUser_ReturnsNotFound()
        {
            // Arrange
            var users = GetTestUsers();
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
            var userId = 999;
            _controller.ControllerContext = GetControllerContextWithUserId(userId);
            var updatedUser = new User { Id = userId, Username = "UpdatedUser", Password = "UpdatedPassword", Email = "teste@teste.com.br" };

            // Act
            var result = _controller.Put(userId, updatedUser);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var users = GetTestUsers();
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
            var userId = 1;
            _controller.ControllerContext = GetControllerContextWithUserId(userId);


            // Act
            var result = _controller.Delete(userId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public void Delete_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var users = GetTestUsers();
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
            var userId = 999;
            _controller.ControllerContext = GetControllerContextWithUserId(userId);


            // Act
            var result = _controller.Delete(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        private IEnumerable<User> GetTestUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Username = "User1", Password = "Password1", Email = "teste@teste.com.br" },
                new User { Id = 2, Username = "User2", Password = "Password2", Email = "teste@teste.com.br"  },
                new User { Id = 3, Username = "User3", Password = "Password3", Email = "teste@teste.com.br"  }
            };
        }

        private ControllerContext GetControllerContextWithUserId(int userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            return new ControllerContext()
            {
                HttpContext = httpContext,
            };
        }
    }
}
