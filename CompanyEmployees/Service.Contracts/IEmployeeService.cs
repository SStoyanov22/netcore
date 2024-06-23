using Entities.Models;
using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges);
    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);
    Task<EmployeeDto> CreateEmployeeFromCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges);
    Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges);
    Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges);
    Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
        Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);
    Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity);
}