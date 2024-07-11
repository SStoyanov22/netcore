using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

public interface IEmployeeLinks
{
    LinkResponse TryGenerateLinks(
        IEnumerable<EmployeeDto> employeesDto,
        string fields, 
        Guid companyId, 
        HttpContext httpContext);
}