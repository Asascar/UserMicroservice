using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using UserMicroservice.Models;
using UserMicroservice.Repositories;
using UserMicroservice.Services;
using Xunit;

namespace UserMicroservice.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly IConfiguration _configuration;
        private readonly UserService _service;
        private readonly AppDbContext _dbContext;

        public UserServiceTests()
        {
            _dbContext = DbContextFactory.Create();
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _service = new UserService(_dbContext, _configuration);

            // Limpar o banco de dados antes de cada teste
            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();
        }

        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var users = GetTestUsers();
            AddUsersToDatabase(users);

            // Act
            var result = _service.GetAllUsers();

            // Assert
            Assert.Equal(users, result.OrderBy(x => x.Id));
        }

        [Fact]
        public void GetUserById_ExistingId_ReturnsUser()
        {
            // Arrange
            var users = GetTestUsers();
            AddUsersToDatabase(users);
            var userId = 1;

            // Act
            var result = _service.GetUserById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Find(u => u.Id == userId), result);
        }

        [Fact]
        public void GetUserById_NonExistingId_ReturnsNull()
        {
            // Arrange
            var testId = 1;

            // Act
            var result = _service.GetUserById(testId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserByEmail_ExistingEmail_ReturnsUser()
        {
            // Arrange
            var testEmail = "test@example.com";
            var testUser = new User { Email = testEmail, Password = "teste", Username = "teste" };
            _dbContext.Users.Add(testUser);
            _dbContext.SaveChanges();

            // Act
            var result = _service.GetUserByEmail(testEmail);

            // Assert
            Assert.Equal(testUser, result);
        }

        [Fact]
        public void GetUserByEmail_NonExistingEmail_ReturnsNull()
        {
            // Arrange
            var testEmail = "test@example.com";

            // Act
            var result = _service.GetUserByEmail(testEmail);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CreateUser_ValidObject_ReturnsUser()
        {
            // Arrange
            var testUser = new User { Id = 1, Username = "TestUser", Password = "testPassword", Email = "teste@teste.com.br" };

            // Act
            var result = _service.CreateUser(testUser);

            // Assert
            Assert.Equal(testUser, result);
        }

        [Fact]
        public void UpdateUser_ExistingId_UpdatesUser()
        {
            // Arrange
            var testId = 1;
            var testUser = new User { Id = testId, Username = "TestUser", Password = "testPassword", Email = "teste@teste.com.br" };
            _dbContext.Users.Add(testUser);
            _dbContext.SaveChanges();

            // Act
            _service.UpdateUser(testId, testUser);

            // Assert
            var updatedUser = _dbContext.Users.Find(testId);
            Assert.Equal(testUser.Username, updatedUser.Username);
            Assert.Equal(testUser.Password, updatedUser.Password);
        }

        [Fact]
        public void UpdateUser_NonExistingId_DoesNothing()
        {
            // Arrange
            var testId = 1;
            var testUser = new User { Id = testId, Username = "TestUser", Password = "testPassword", Email = "teste@teste.com.br" };

            // Act
            _service.UpdateUser(testId, testUser);

            // Assert
            var updatedUser = _dbContext.Users.Find(testId);
            Assert.Null(updatedUser);
        }

        [Fact]
        public void DeleteUser_ExistingId_DeletesUser()
        {
            // Arrange
            var testId = 1;
            var testUser = new User { Id = testId, Username = "TestUser", Password = "testPassword", Email = "teste@teste.com.br" };
            _dbContext.Users.Add(testUser);
            _dbContext.SaveChanges();

            // Act
            _service.DeleteUser(testId);

            // Assert
            var deletedUser = _dbContext.Users.Find(testId);
            Assert.Null(deletedUser);
        }

        [Fact]
        public void DeleteUser_NonExistingId_DoesNothing()
        {
            // Arrange
            var testId = 1;
            var testUser = new User { Id = testId, Username = "TestUser", Password = "testPassword", Email = "teste@teste.com.br" };

            // Act
            _service.DeleteUser(testId);

            // Assert
            var deletedUser = _dbContext.Users.Find(testId);
            Assert.Null(deletedUser);
        }

        [Fact]
        public void GenerateJwtToken_ReturnsToken()
        {
            // Arrange
            var testUser = new User { Id = 1, Username = "TestUser" };

            // Act
            var token = _service.GenerateJwtToken(testUser);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "test_user";
            var password = "test_password";
            var testUser = new User { Username = username, Password = password, Email = "teste@teste.com.br" };
            _dbContext.Users.Add(testUser);
            _dbContext.SaveChanges();

            // Act
            var result = _service.Authenticate(username, password);

            // Assert
            Assert.Equal(testUser, result);
        }

        [Fact]
        public void Authenticate_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var username = "test_user";
            var password = "test_password";
            var testUser = new User { Username = username, Password = password, Email = "teste@teste.com.br" };
            _dbContext.Users.Add(testUser);
            _dbContext.SaveChanges();

            // Act
            var result = _service.Authenticate(username, "wrong_password");

            // Assert
            Assert.Null(result);
        }

        private List<User> GetTestUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Username = "User1", Password = "Password1", Email = "teste@teste.com.br" },
                new User { Id = 2, Username = "User2", Password = "Password2", Email = "teste@teste.com.br"  },
                new User { Id = 3, Username = "User3", Password = "Password3", Email = "teste@teste.com.br"  }
            };
        }

        private void AddUsersToDatabase(List<User> users)
        {
            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
        }
    }
}
