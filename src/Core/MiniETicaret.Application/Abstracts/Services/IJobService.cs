using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;

namespace MiniETicaret.Application.Abstracts.Services;

public interface IJobService
{
    Task<BaseResponse<string>> FreezeInactiveUsersAsync();
    Task<BaseResponse<string>> FreezeIfInactive(AppUser user);

}
