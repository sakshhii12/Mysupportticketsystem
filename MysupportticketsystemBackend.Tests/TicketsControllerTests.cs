using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MysupportticketsystemBackend.Controllers;
using MysupportticketsystemBackend.Data;
using MysupportticketsystemBackend.Models;
using MysupportticketsystemBackend.Models.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class TicketsControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<TicketsController>> _loggerMock;
    private readonly ApplicationDbContext _dbContext;

    public TicketsControllerTests()
    {
        // Setup in-memory EF Core database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TicketsTestDb")
            .Options;
        _dbContext = new ApplicationDbContext(options);

        // Clear DB to have fresh start for each test
        _dbContext.Tickets.RemoveRange(_dbContext.Tickets);
        _dbContext.SaveChanges();

        // Setup UserManager mock
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object,
            null, null, null, null, null, null, null, null);

        // Setup Mapper mock
        _mapperMock = new Mock<IMapper>();

        // Setup Logger mock
        _loggerMock = new Mock<ILogger<TicketsController>>();
    }

    private ClaimsPrincipal CreateUserPrincipal(string userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
    }

    [Fact]
    public async Task GetMyTickets_Returns_AllTickets_ForAdmin()
    {
        // Arrange
        var userId = "admin-user-id";
        var controller = new TicketsController(_dbContext, _userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUserPrincipal(userId, "Admin")
            }
        };

        _dbContext.Tickets.Add(new Ticket
        {
            Id = 1,
            UserId = "user1",
            Title = "Ticket 1",
            Description = "Description 1",
            Priority = TicketPriority.Medium,
            Category = TicketCategory.General,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        });
        _dbContext.Tickets.Add(new Ticket
        {
            Id = 2,
            UserId = "user2",
            Title = "Ticket 2",
            Description = "Description 2",
            Priority = TicketPriority.High,
            Category = TicketCategory.billing,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        _mapperMock.Setup(m => m.Map<List<TicketDto>>(It.IsAny<List<Ticket>>()))
                   .Returns(new List<TicketDto> { new TicketDto { Id = 1, Title = "Ticket 1" }, new TicketDto { Id = 2, Title = "Ticket 2" } });

        // Act
        var result = await controller.GetMyTickets(new TicketQueryDto());

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<TicketDto>>>(result);
        var returnedEnumerable = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(actionResult.Value);
        Assert.NotNull(returnedEnumerable);
        Assert.Equal(2, returnedEnumerable.Count());
    }
    [Fact]
    public async Task CreateTicket_SetsUserAndReturnsOk()
    {
        // Arrange
        var userId = "user-123";
        var controller = new TicketsController(_dbContext, _userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUserPrincipal(userId, "User")
            }
        };

        var createTicketDto = new CreateTicketDto
        {
            Title = "New Ticket",
            Description = "Description here",
            Priority = "High",
            Category = "Technical"
        };

        var ticket = new Ticket
        {
            Id = 1,
            Title = "New Ticket",
            Description = "Description here",
            Priority = TicketPriority.High,
            Category = TicketCategory.Technical,
            UserId = userId,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<Ticket>(createTicketDto)).Returns(ticket);
        _mapperMock.Setup(m => m.Map<TicketDto>(It.IsAny<Ticket>()))
                   .Returns((Ticket source) => new TicketDto
                   {
                       Id = source.Id,
                       Title = source.Title
                   });

        //// Act
        //var result = await controller.CreateTicket(createTicketDto);

        //// Assert
        //var actionResult = Assert.IsType<ActionResult<TicketDto>>(result);
        //var returnedDto = Assert.NotNull(actionResult.Value);
        //Assert.Equal("New Ticket", returnedDto.Title);



        var actionResult = await controller.CreateTicket(createTicketDto);

       
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedDto = Assert.IsType<TicketDto>(okResult.Value);

        Assert.Equal("New Ticket", returnedDto.Title);
    }


}
