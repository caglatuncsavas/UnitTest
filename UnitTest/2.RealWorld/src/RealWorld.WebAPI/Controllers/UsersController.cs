using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWorld.WebAPI.Dtos;
using RealWorld.WebAPI.Services;

namespace RealWorld.WebAPI.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await userService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto request, CancellationToken cancellationToken)
    {
        var result = await userService.CreateAsync(request, cancellationToken);
        if (result)
        {
            return Ok(new {Message = "Kullanıcı kaydı başarılı"});
        }

        return BadRequest(new {Message = "Kullanıcı kaydı sırasından bir hatayla karşılaştık"});
    }

    [HttpGet]
    public async Task<IActionResult> DeleteById(int id, CancellationToken cancellationToken)
    {
        var result = await userService.DeleteByIdAsync(id, cancellationToken);
        if (result)
        {
            return Ok(new {Message = "User has been deleted successfully." });
        }

        return BadRequest(new {Message = "Take an error while the user deletion process." });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateUserDto request, CancellationToken cancellationToken)
    {
        var result = await userService.UpdateAsync(request, cancellationToken);
        if (result)
        {
            return Ok(new {Message = "User has been updated successfully." });
        }

        return BadRequest(new {Message = "Take an error while the user update process." });
    }
}
