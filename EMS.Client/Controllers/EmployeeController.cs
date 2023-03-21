using EMS.Client.Extensions;
using EMS.Client.Models;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Client.Controllers
{
    public class EmployeeController : Controller
    {
        #region Private Fields

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EmployeeController> _logger;

        #endregion

        #region Constructor

        public EmployeeController(IHttpClientFactory httpClientFactory, ILogger<EmployeeController> logger)
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
                IEnumerable<EmployeeResponse>? employees = await httpClient.GetFromJsonAsync<IEnumerable<EmployeeResponse>>("api/employee/all", cancellationToken);

                return View(PaginatedList<EmployeeResponse>.Create(employees, pageNo, pageSize));
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
                IEnumerable<EmployeeResponse>? employees = await httpClient.GetFromJsonAsync<IEnumerable<EmployeeResponse>>($"api/employee/all?searchKey={searchKey}", cancellationToken);

                return View("List", PaginatedList<EmployeeResponse>.Create(employees, 1, 5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEditAsync(Guid? employeeId, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
                IEnumerable<DropdownResponse>? departments = await httpClient.GetFromJsonAsync<IEnumerable<DropdownResponse>>("api/department/dropdown", cancellationToken);

                if (employeeId == null)
                {
                    ViewBag.Departments = departments.ToSelectListItems();
                    return View("AddOrEdit", new EmployeeRequest { DateOfBirth = DateTime.Now.AddYears(-20).Date});
                }

                EmployeeRequest model = await GetEmployeeAsync(employeeId, cancellationToken);
                ViewBag.Departments = departments.ToSelectListItems(model.DepartmentId);
                return View("AddOrEdit", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(EmployeeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("AddOrEdit", request);
                }

                HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
                if (request.Id == null)
                {
                    await httpClient.PostAsJsonAsync("api/employee/add", request, cancellationToken);
                }
                else
                {
                    await httpClient.PutAsJsonAsync("api/employee/update", request, cancellationToken);
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
        public async Task<IActionResult> DeleteConfirmationAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            try
            {
                EmployeeRequest model = await GetEmployeeAsync(employeeId, cancellationToken);
                return View("DeleteConfirmation", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(EmployeeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
                HttpResponseMessage httpResponse = await httpClient.DeleteAsync($"api/employee/{request.Id}", cancellationToken);
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

        private async Task<EmployeeRequest> GetEmployeeAsync(Guid? employeeId, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("EMSApi");
                EmployeeResponse? response = await httpClient.GetFromJsonAsync<EmployeeResponse>($"api/employee/{employeeId}", cancellationToken) ?? throw new InvalidOperationException($"Employee with Id: {employeeId} not found.");
                return ToViewModel(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static EmployeeRequest ToViewModel(EmployeeResponse response)
        {
            return new EmployeeRequest
            {
                DateOfBirth = response.DateOfBirth,
                DepartmentId = response.DepartmentId,
                Email = response.Email,
                Id = response.Id,
                Name = response.Name,
                DepartmentName = response.DepartmentName
            };
        }

        #endregion
    }
}
