using EMS.DL.Abstractions;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController : ControllerBase
{
    #region Private Fields

    private readonly IDepartmentRepository _departmentRepository;

    #endregion

    #region Constructor

    public DepartmentController(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    #endregion

    #region Endpoints

    /// <summary>
    /// Gets all departments against searchKey if any.
    /// </summary>
    /// <param name="searchKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="IEnumerable{T}"/> where T is <see cref="DepartmentResponse"/></returns>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAllAsync([FromQuery] string? searchKey = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<DepartmentResponse> departments = await _departmentRepository.GetAllAsync(searchKey, cancellationToken);
            return Ok(departments);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets departments id and name.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="IEnumerable{T}"/> where T is <see cref="DropdownResponse"/></returns>
    [HttpGet("dropdown")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DropdownResponse>>> GetDropdownAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<DropdownResponse> departments = await _departmentRepository.GetDropdownAsync(cancellationToken);
            return Ok(departments);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets a department against id.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="DepartmentResponse"/></returns>
    [HttpGet("{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentResponse?>> GetByIdAsync([FromRoute] Guid departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            DepartmentResponse? department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);
            return Ok(department);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds new department.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _departmentRepository.AddAsync(request, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing department.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromBody] DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _departmentRepository.UpdateAsync(request, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an existing department.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpDelete("{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _departmentRepository.DeleteAsync(departmentId, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
}