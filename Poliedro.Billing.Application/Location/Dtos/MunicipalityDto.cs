using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Application.Location.Dtos;

public class MunicipalityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string Divipola { get; set; } = string.Empty;
}
