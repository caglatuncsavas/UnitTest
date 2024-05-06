using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;
using RealWorld.WebAPI.Dtos;
using RealWorld.WebAPI.Logging;
using RealWorld.WebAPI.Models;
using RealWorld.WebAPI.Repositories;
using RealWorld.WebAPI.Services;


namespace Users.API.Tests.Unit;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly ILoggerAdapter<UserService> logger = Substitute.For<ILoggerAdapter<UserService>>();
    private readonly CreateUserDto createUserDto = new("Çaðla Tunç Savaþ", 30, new(1994, 06, 04));
    private readonly UpdateUserDto updateUserDto = new(1, "Tuðçe Canlý", 30, new DateOnly(1994, 04, 05));
    private  User user = new()
    {
        Id = 1,
        Name = "Çaðla Tunç Savaþ",
        Age = 30,
        DateOfBirth = new(1994, 06, 04)
    };

    public UserServiceTests()
    {
        _sut = new(userRepository, logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmtyList_WhenNoUsersExist()
    {
        // Arrange
        userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

        //Act
        var result = await _sut.GetAllAsync();

        //Asset
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenSomeUserExist()
    {
        // Arrange
        var caglaUser = new User
        {
            Id = 1,
            Age = 30,
            Name = "Çaðla Tunç Savaþ",
            DateOfBirth = new(1994, 06, 04)
        };

        var ercanUser = new User
        {
            Id = 1,
            Age = 30,
            Name = "Ercan  Savaþ",
            DateOfBirth = new(1994, 06, 04)
        };

        var users = new List<User>() { caglaUser, ercanUser };
        userRepository.GetAllAsync().Returns(users);

        //Act
        var result = await _sut.GetAllAsync();

        //Asset
        result.Should().BeEquivalentTo(users);
        result.Should().HaveCount(2);
        result.Should().NotHaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

        //Act
        await _sut.GetAllAsync();

        //Assert
        logger.Received(1).LogInformation(Arg.Is("Get all users"));
        logger.Received(1).LogInformation(Arg.Is("Get all users completed"));
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessageAnException_WhenExceptionIsThrown()
    {
        //Arrange
        var exception = new ArgumentException("An error occurred while getting all users");
        userRepository.GetAllAsync().Throws(exception);

        //Act
        var requestAction = async () => await _sut.GetAllAsync();

        await requestAction.Should().ThrowAsync<ArgumentException>();

        logger.Received(1).LogError(Arg.Is(exception), Arg.Is("An error occurred while getting all users"));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrownAnError_WhenUserCreateDetailAreNotValid()
    {
        //Arrange
        CreateUserDto request = new("", 0, new(2007, 01, 01));

        //Act
        var action = async () => await _sut.CreateAsync(request);

        //Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrownAnError_WhenUserNameIsExist()
    {
        //Arrange
        userRepository.NameIsExists(Arg.Any<string>()).Returns(true);

        //Act
        var action = async () => await _sut.CreateAsync(new CreateUserDto("Çaðla Tunç Savaþ", 30, new DateOnly(1994, 06, 04)));

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void CreateAsync_ShouldCreateUserDtoToUserObject()
    {
        //Arrange
        var user = _sut.CreateUserDtoToUserObject(createUserDto);

        //Assert
        user.Name.Should().Be(createUserDto.Name);
        user.Age.Should().Be(createUserDto.Age);
        user.DateOfBirth.Should().Be(createUserDto.DateOfBirth);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenDetailsAreValidAndUnique()
    {
        //Arrange
        userRepository.NameIsExists(createUserDto.Name).Returns(false);
        userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

        //Act
        var result = await _sut.CreateAsync(createUserDto);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        userRepository.NameIsExists(createUserDto.Name).Returns(false);
        userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

        //Act
        await _sut.CreateAsync(createUserDto);

        //Assert
        logger.Received(1).LogInformation(
            Arg.Is("User Name: {0} bu olan kullanýcý kaydý yapýlmaya baþlandý"),
            Arg.Any<string>());

        logger.Received(1).LogInformation(
            Arg.Is("User Id: {0} olan kullanýcý {1} ms de oluþturuldu"),
            Arg.Any<int>(),
            Arg.Any<long>());
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessagesAnException_WhenExceptionIsThrown()
    {
        //Arrange
        var exception = new ArgumentException("An error occurred while creating user");
        userRepository.CreateAsync(Arg.Any<User>()).Throws(exception);

        //Act
        var action = async () => await _sut.CreateAsync(createUserDto);

        //Assert
        await action.Should()
            .ThrowAsync<ArgumentException>();

        logger.Received(1).LogError(Arg.Is(exception), Arg.Is("An error occurred while creating user"));

    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldThrownAnError_WhenUserIsNotFound()
    {
        //Arrange
        int userId = 1;
        userRepository.GetByIdAsync(userId).ReturnsNull();

        //Act
        var action = async () => await _sut.DeleteByIdAsync(userId);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldDeleteUser_WhenUserIsFound()
    {
        //Arrange
        int userId = 1;
        User user = new()
        {
            Id = userId,
            Name = "Çaðla Tunç Savaþ",
            Age = 30,
            DateOfBirth = new(1994, 06, 04)
        };
        userRepository.GetByIdAsync(userId).Returns(user);
        userRepository.DeleteAsync(user).Returns(true);

        //Act
        var result = await _sut.DeleteByIdAsync(userId);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteByIsAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        int userId = 1;
        var user = new User
        {
            Id = userId,
            Name = "Çaðla Tunç Savaþ",
            Age = 30,
            DateOfBirth = new(1994, 06, 04)
        };

        userRepository.GetByIdAsync(userId).Returns(user);
        userRepository.DeleteAsync(user).Returns(true);

        //Act
        await _sut.DeleteByIdAsync(userId);

        //Assert
        logger.Received(1).LogInformation(
              Arg.Is("User Id: {0} olan kullanýcý silinmeye baþlandý"),
              Arg.Is(userId));
        logger.Received(1).LogInformation(
              Arg.Is("User Id: {0} olan kullanýcý {1} ms de silindi"),
              Arg.Is(userId),
              Arg.Any<long>());
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldLogMessagesAndException_WhenExceptionIsThrown()
    {
        //Arrange
        int userId = 1;
        var user = new User()
        {
            Id = userId,
            Name = "Çaðla Tunç Savaþ",
            Age = 30,
            DateOfBirth = new(1994, 06, 04)
        };

        userRepository.GetByIdAsync(userId).Returns(user);
        var exception = new ArgumentException("An error occurred while deleting user");
        userRepository.DeleteAsync(user).Throws(exception);

        //Act
        var action = async () => await _sut.DeleteByIdAsync(userId);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
        logger.Received(1).LogError(
            Arg.Is(exception),
            Arg.Is("An error occurred while deleting user"));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowAnError_WhenUserNotExist()
    {
        //Arrange
        userRepository.GetByIdAsync(updateUserDto.Id).ReturnsNull();

        //Act

        var action = async () => await _sut.UpdateAsync(updateUserDto);

        //Assert

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrownAnError_WhenUserUpdateDetailsAreNotValid()
    {
        //Arrange
        UpdateUserDto invalidUpdateDto = new UpdateUserDto(1, "Tuðce Canlý", 17, new DateOnly(2007, 01, 02));
        userRepository.GetByIdAsync(invalidUpdateDto.Id).Returns(user);

        //Act
        var action = async () => await _sut.UpdateAsync(invalidUpdateDto);

        //Assert

        await action.Should().ThrowAsync<ValidationException>();

    }

    [Fact]
    public async Task UpdateAsync_ShoulThrownAnError_WhenUserNameExist()
    {
        //Arrange
        userRepository.NameIsExists(Arg.Any<string>()).Returns(true);
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);

        //Act
        var action = async () => await _sut.UpdateAsync(updateUserDto);

        //Assert

        await action.Should().ThrowAsync<ArgumentException>();

    }

    [Fact]
    public async Task UpdateAsync_ShouldCreateUpdateUserDtoToUserObject()
    {
        //Act
        _sut.CreateUpdateUserObject(ref user , updateUserDto);

        //Asset

        user.Name.Should().Be(updateUserDto.Name);
        user.Age.Should().Be(updateUserDto.Age);
        user.DateOfBirth.Should().Be(updateUserDto.DateOfBirth);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenDetailsAreValidAndUnique()
    {
        //Arrange
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);
        userRepository.NameIsExists(updateUserDto.Name).Returns(false);
        userRepository.UpdateAsync(user).Returns(true);

        //Act
        var result = await _sut.UpdateAsync(updateUserDto);

        //Assert

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ShouldLogMessages_WhenInvoked()
    {

        //Arrange
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);
        userRepository.NameIsExists(updateUserDto.Name).Returns(false);
        userRepository.UpdateAsync(user).Returns(true);

        //Act
        await _sut.UpdateAsync(updateUserDto);

        //Assert
        logger.Received(1).LogInformation(
            Arg.Is(" {0} updating user"),
            Arg.Any<string>());

        logger.Received(1).LogInformation(
            Arg.Is("The update process of the user with ID {0} was completed successfully in {1}ms"),
            Arg.Any<int>(),
            Arg.Any<long>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldLogMessagesAndException_WhenExceptionIsThrown()
    {
        //Arrange
        var exception = new ArgumentException("An error occurred while updating user");
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);
        userRepository.NameIsExists(updateUserDto.Name).Returns(false);
        userRepository.UpdateAsync(Arg.Any<User>()).Throws(exception);

        //Act
        var action = async () => await _sut.UpdateAsync(updateUserDto);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();

        logger.Received(1).LogError(
            Arg.Is(exception),
            Arg.Is("An error occured while updating user"));
    }

}