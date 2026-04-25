using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Domain.Location.Entities;

public class MunicipalityEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string Divipola { get; set; } = string.Empty;
}
