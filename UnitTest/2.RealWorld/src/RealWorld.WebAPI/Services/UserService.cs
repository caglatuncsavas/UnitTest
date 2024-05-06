using FluentValidation;
using RealWorld.WebAPI.Dtos;
using RealWorld.WebAPI.Logging;
using RealWorld.WebAPI.Models;
using RealWorld.WebAPI.Repositories;
using RealWorld.WebAPI.Validators;
using System.Diagnostics;


namespace RealWorld.WebAPI.Services;

public sealed class UserService(
    IUserRepository userRepository,
    ILoggerAdapter<UserService> logger) : IUserService
{

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Get all users");
        try
        {
            return await userRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting all users");
            throw;
        }
        finally
        {
            logger.LogInformation("Get all users completed");
        }
    }

    public async Task<bool> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        CreateUserDtoValidator validator = new();
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            throw new ValidationException(string.Join(", ", result.Errors.Select(s => s.ErrorMessage)));
        }

        var nameIsExist = await userRepository.NameIsExists(request.Name, cancellationToken);
        if (nameIsExist)
        {
            throw new ArgumentException("Name is already exist");
        }

        var user = CreateUserDtoToUserObject(request);

        logger.LogInformation("User Name: {0} bu olan kullanıcı kaydı yapılmaya başlandı", user.Name);
        var stopWatch = Stopwatch.StartNew();
        try 
        {
            return await userRepository.CreateAsync(user, cancellationToken);
        }
       catch (Exception ex)
        {
           logger.LogError(ex, "An error occurred while creating user");
            throw;
        }
        finally
        {
            stopWatch.Stop();
            logger.LogInformation("User Id: {0} olan kullanıcı {1} ms de oluşturuldu", user.Id,
                stopWatch.ElapsedMilliseconds);
        }
    }

    public User CreateUserDtoToUserObject(CreateUserDto request)
    {
        return new User
        {
            Name = request.Name,
            DateOfBirth = request.DateOfBirth,
            Age = request.Age
        };
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
       User? user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        logger.LogInformation("User Id: {0} olan kullanıcı silinmeye başlandı", id);
        var stopWatch = Stopwatch.StartNew();
        try
        {
            return await userRepository.DeleteAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting user");
            throw;
        }
        finally
        {
            stopWatch.Stop();
            logger.LogInformation("User Id: {0} olan kullanıcı {1} ms de silindi", user.Id,
                 stopWatch.ElapsedMilliseconds);
        }
    }

    public async Task<bool> UpdateAsync(UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        User? user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            throw new ArgumentException("User not found!");
        }

        UpdateUserDtoValidator validator = new();
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            throw new ValidationException(string.Join("\n", result.Errors.Select(s => s.ErrorMessage)));
        }


        if (request.Name != user.Name)
        {
            var nameIsExist = await userRepository.NameIsExists(request.Name, cancellationToken); 
            if (nameIsExist)
            {
                throw new ArgumentException("Name is already exist");
            }
        }

        CreateUpdateUserObject(ref user, request);

        logger.LogInformation(" {0} updating user", request.Name);
        var stopWatch = Stopwatch.StartNew();

        try
        {
            return await userRepository.UpdateAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"An error occured while updating user");
            throw;
        }
        finally
        {
            stopWatch.Stop();
            logger.LogInformation("The update process of the user with ID {0} was completed successfully in {1}ms", user.Id, stopWatch.ElapsedMilliseconds);
        }
    }

    public void CreateUpdateUserObject(ref User user, UpdateUserDto request)
    {
        user.Name = request.Name;
        user.Age = request.Age;
        user.DateOfBirth = request.DateOfBirth;
    }
}
