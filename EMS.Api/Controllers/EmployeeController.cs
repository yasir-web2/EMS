using EMS.DL.Abstractions;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    #region Private Fields

    private readonly IEmployeeRepository _employeeRepository;

    #endregion

    #region Constructor

    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    #endregion

    #region Endpoints

    /// <summary>
    /// Gets all employees against searchKey if any.
    /// </summary>
    /// <param name="searchKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="IEnumerable{T}"/> where T is <see cref="EmployeeResponse"/></returns>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAllAsync([FromQuery] string? searchKey = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<EmployeeResponse> employees = await _employeeRepository.GetAllAsync(searchKey, cancellationToken);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets all employee against departmentId.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="IEnumerable{T}"/> where T is <see cref="EmployeeResponse"/></returns>
    [HttpGet("all/department/{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAllByDepartmentIdAsync([FromRoute] Guid departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<EmployeeResponse> employees = await _employeeRepository.GetAllByDepartmentAsync(departmentId, cancellationToken);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets an employee against id.
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="EmployeeResponse"/></returns>
    [HttpGet("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeResponse?>> GetByIdAsync([FromRoute] Guid employeeId, CancellationToken cancellationToken = default)
    {
        try
        {
            EmployeeResponse? employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
            return Ok(employee);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds new employee.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _employeeRepository.AddAsync(request, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _employeeRepository.UpdateAsync(request, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an existing employee.
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpDelete("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid employeeId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _employeeRepository.DeleteAsync(employeeId, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
}