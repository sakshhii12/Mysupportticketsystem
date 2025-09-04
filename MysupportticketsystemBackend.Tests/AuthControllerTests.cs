using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using MysupportticketsystemBackend.Controllers;
using MysupportticketsystemBackend.Models;
using MysupportticketsystemBackend.Models.DTOs;
using System.Threading.Tasks;
using Xunit;

public class AuthControllerTests
{
    private Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
    {
        var userManagerMock = MockUserManager();
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new ApplicationUser());
        var configurationMock = new Mock<IConfiguration>();
        var controller = new AuthController(userManagerMock.Object, configurationMock.Object);
        var registerDto = new RegisterUserDto
        {
            Email = "test@example.com",
            Password = "Pass123!"
        };

        var result = await controller.Register(registerDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenNewUserIsCreated()
    {
        var userManagerMock = MockUserManager();
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync((ApplicationUser)null); // user not found
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success);
        var configurationMock = new Mock<IConfiguration>();
        var controller = new AuthController(userManagerMock.Object, configurationMock.Object);
        var registerDto = new RegisterUserDto
        {
            Email = "newuser@example.com",
            Password = "Pass123!"
        };

        var result = await controller.Register(registerDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var messageProperty = okResult.Value.GetType().GetProperty("message");
        Assert.NotNull(messageProperty);
        var messageValue = messageProperty.GetValue(okResult.Value) as string;
        Assert.Equal("User created successfully!", messageValue);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenCreateFails()
    {
        var userManagerMock = MockUserManager();
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync((ApplicationUser)null);
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));
        var configurationMock = new Mock<IConfiguration>();
        var controller = new AuthController(userManagerMock.Object, configurationMock.Object);
        var registerDto = new RegisterUserDto
        {
            Email = "failuser@example.com",
            Password = "weakpass"
        };

        var result = await controller.Register(registerDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // ------------ LOGIN TESTS ----------------
   
    [Fact]
    public async Task Login_Succeeds_WithValidCredentials()
    {
        var user = new ApplicationUser { Id = "test-user-id", Email = "validuser@example.com", UserName = "validuser@example.com" };
        var userManagerMock = MockUserManager();
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>()); // Mock roles

        // Update config keys to match your code
        var inMemorySettings = new Dictionary<string, string> {
        {"JWT:Secret", "YourSuperSecretKeyForTesting!12345"},
        {"JWT:ValidIssuer", "YourAppIssuer"},
        {"JWT:ValidAudience", "YourAppAudience"}
    };
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c[It.Is<string>(key => key == "JWT:Secret")])
                         .Returns(inMemorySettings["JWT:Secret"]);
        configurationMock.Setup(c => c[It.Is<string>(key => key == "JWT:ValidIssuer")])
                         .Returns(inMemorySettings["JWT:ValidIssuer"]);
        configurationMock.Setup(c => c[It.Is<string>(key => key == "JWT:ValidAudience")])
                         .Returns(inMemorySettings["JWT:ValidAudience"]);

        var controller = new AuthController(userManagerMock.Object, configurationMock.Object);

        var loginDto = new LoginUserDto
        {
            Email = "validuser@example.com",
            Password = "ValidPassword123"
        };

        var result = await controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }


    [Fact]
    public async Task Login_Fails_WithInvalidPassword()
    {
        var user = new ApplicationUser { Email = "validuser@example.com", UserName = "validuser@example.com" };
        var userManagerMock = MockUserManager();
    }
}