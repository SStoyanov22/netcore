using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);
    EmployeeDto GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    EmployeeDto CreateEmployeeFromCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges);
    void DeleteEmployeeForCompany(Guid companyId, Guid employeeId, bool trackChanges);
    void UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges);
}