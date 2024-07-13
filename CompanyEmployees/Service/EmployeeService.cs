using System.Dynamic;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;
internal sealed class EmployeeService : IEmployeeService
{
private readonly IRepositoryManager _repository;
private readonly ILoggerManager _logger;
private readonly IMapper _mapper;
private readonly IDataShaper<EmployeeDto> _dataShaper;
private readonly IEmployeeLinks _employeeLinks;
public EmployeeService(IRepositoryManager repository, 
                        ILoggerManager logger,
                        IMapper mapper,
                        IEmployeeLinks employeeLinks)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _employeeLinks = employeeLinks;
    }

    public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync
        (Guid companyId, LinkParameters linkParameters, bool trackChanges)
    {
        if (!linkParameters.EmployeeParameters.ValidAgeRange)
            throw new MaxAgeRangeBadRequestException();

        await CheckIfCompanyExists(companyId, trackChanges);

        var employeesWithMetaData = await _repository.Employee
            .GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);
        var links = _employeeLinks.TryGenerateLinks(employeesDto, linkParameters.EmployeeParameters.Fields,
            companyId, linkParameters.Context);

        return (linkResponse: links, metaData: employeesWithMetaData.MetaData);
}

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges);
        
        _repository.Employee.DeleteEmployee(employee);
        await _repository.SaveAsync();
    }

    public async Task <EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges);
        var employeeDto = _mapper.Map<EmployeeDto>(employee);

        return employeeDto;
    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges);
        
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
    var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employee);

        return (employeeToPatch, employee);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(
        Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
    {
        
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId,empTrackChanges);
        
        _mapper.Map(employeeForUpdate, employee);
        await _repository.SaveAsync();
    }

    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);
    }
    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists
    (Guid companyId, Guid id, bool trackChanges)
    {
        var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id,
        trackChanges);
        if (employeeDb is null)
            throw new EmployeeNotFoundException(id);

        return employeeDb;
    }
    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId,
        EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }
}