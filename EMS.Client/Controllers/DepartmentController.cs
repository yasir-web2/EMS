using EMS.Client.Models;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Client.Controllers;

public class DepartmentController : Controller
{
    #region Private Fields

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DepartmentController> _logger;

    #endregion

    #region Constructor

    public DepartmentController(IHttpClientFactory httpClientFactory, ILogger<DepartmentController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    #endregion

    #region Public Methods

    [HttpGet]
    public async Task<IActionResult> ListAsync(int pageNo = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
            IEnumerable<DepartmentResponse>? departments = await httpClient.GetFromJsonAsync<IEnumerable<DepartmentResponse>>("api/department/all", cancellationToken);

            return View(PaginatedList<DepartmentResponse>.Create(departments, pageNo, pageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync(string searchKey, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
            IEnumerable<DepartmentResponse>? departments = await httpClient.GetFromJsonAsync<IEnumerable<DepartmentResponse>>($"api/department/all?searchKey={searchKey}", cancellationToken);

            return View("List", PaginatedList<DepartmentResponse>.Create(departments, 1, 5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddOrEditAsync(Guid? departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (departmentId == null)
            {
                return View("AddOrEdit", new DepartmentRequest());
            }

            DepartmentRequest model = await GetDepartmentAsync(departmentId, cancellationToken);
            return View("AddOrEdit", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAsync(DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
            if (request.Id == null)
            {
                await httpClient.PostAsJsonAsync("api/department/add", request, cancellationToken);
            }
            else
            {
                await httpClient.PutAsJsonAsync("api/department/update", request, cancellationToken);
            }

            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteConfirmationAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            DepartmentRequest model = await GetDepartmentAsync(departmentId, cancellationToken);
            return View("DeleteConfirmation", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
            HttpResponseMessage httpResponse = await httpClient.DeleteAsync($"api/department/{request.Id}", cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(httpResponse.ReasonPhrase);
            }
            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return View("Error");
        }
    }

    #endregion

    #region Private Helpers

    private async Task<DepartmentRequest> GetDepartmentAsync(Guid? departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
            DepartmentResponse? response = await httpClient.GetFromJsonAsync<DepartmentResponse>($"api/department/{departmentId}", cancellationToken) ?? throw new InvalidOperationException($"Department with Id: {departmentId} not found.");
            return ToViewModel(response);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static DepartmentRequest ToViewModel(DepartmentResponse response)
    {
        return new DepartmentRequest
        {
            Id = response.Id,
            Name = response.Name
        };
    }

    #endregion
}