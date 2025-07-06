using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniETicaret.Application.DTOs.RoleDtos;

public class RoleUpdateDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<string> PermissionList { get; set; } = new();

}
