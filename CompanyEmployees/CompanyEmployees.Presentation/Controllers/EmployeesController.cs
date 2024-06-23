using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController: ControllerBase
{
    private readonly IServiceManager _service;
    public EmployeesController(IServiceManager service) => _service = service;

    [HttpGet("{id:guid}", Name="GetEmployeeForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = _service.EmployeeService.GetEmployeeAsync(companyId, id, false);

        return Ok(employee);
    }

    [HttpGet]
	public IActionResult GetEmployeesForCompany(Guid companyId)
	{
		var employees = _service.EmployeeService.GetEmployeesAsync(companyId, trackChanges: false);

		return Ok(employees);
	}
    
    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        if (employee == null)
            return BadRequest("EmployeeForCreationDto object is null");
        
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        
        var employeeToReturn = _service.EmployeeService.CreateEmployeeFromCompanyAsync(companyId, employee, false);

        return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id =
            employeeToReturn.Id },
            employeeToReturn);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges:false);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
    {
        if (employee is null)
            return BadRequest("EmployeeForUpdateDto object is null");
        
        _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, compTrackChanges: false, empTrackChanges: true);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null");
        
        var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id,
			compTrackChanges: false, empTrackChanges: true);

		patchDoc.ApplyTo(result.employeeToPatch, ModelState);

		TryValidateModel(result.employeeToPatch);

		if (!ModelState.IsValid)
			return UnprocessableEntity(ModelState);

		await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);

		return NoContent();
    }
}