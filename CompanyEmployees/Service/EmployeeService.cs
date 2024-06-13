using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
namespace Service;
internal sealed class EmployeeService : IEmployeeService
{
private readonly IRepositoryManager _repository;
private readonly ILoggerManager _logger;
private readonly IMapper _mapper;
public EmployeeService(IRepositoryManager repository, 
                        ILoggerManager logger,
                        IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public EmployeeDto CreateEmployeeFromCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        if (company == null)
            throw new CompanyNotFoundException(companyId);
        
        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        _repository.Save();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }

    public void DeleteEmployeeForCompany(Guid companyId, Guid employeeId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId,trackChanges);
        if (company == null)
            throw new CompanyNotFoundException(companyId);
        
        var employee = _repository.Employee.GetEmployee(companyId, employeeId, trackChanges);
        if (employee == null)
           throw new EmployeeNotFoundException(employeeId);
        
        _repository.Employee.DeleteEmployee(employee);
        _repository.Save();
    }

    public EmployeeDto GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId,trackChanges);
        if (company == null)
            throw new CompanyNotFoundException(companyId);
        
        var employee = _repository.Employee.GetEmployee(companyId, employeeId, trackChanges);
        if (employee == null)
           throw new EmployeeNotFoundException(employeeId);
        var employeeDto = _mapper.Map<EmployeeDto>(employee);

        return employeeDto;
    }

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);
        
        var employees = _repository.Employee.GetEmployees(companyId, trackChanges);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        
        return employeesDto;
    }

    public void UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, compTrackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);
        
        var employee = _repository.Employee.GetEmployee(companyId, employeeId, empTrackChanges);
        if (employee is null)
            throw new EmployeeNotFoundException(employeeId);
        
        _mapper.Map(employeeForUpdate, employee);
        _repository.Save();
    }
}