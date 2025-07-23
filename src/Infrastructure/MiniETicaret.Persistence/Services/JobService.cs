using System.Net;
using Microsoft.AspNetCore.Identity;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Infrastructure.Services;

namespace MiniETicaret.Persistence.Services;

public class JobService : IJobService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;


    public JobService(UserManager<AppUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;


    }
    public async Task<BaseResponse<string>> FreezeIfInactive(AppUser user)
    {
        if (user.LastLoginDate != null && user.LastLoginDate < DateTime.UtcNow.AddYears(-1) && !user.IsFrozen)
        {
            user.IsFrozen = true;
            await _userManager.UpdateAsync(user);
            return new BaseResponse<string>("User has been frozen due to inactivity.", null, HttpStatusCode.OK);
        }
        else if (user.IsFrozen)
        {
            return new BaseResponse<string>("User is already frozen.", null, HttpStatusCode.BadRequest);
        }
        return new BaseResponse<string>("User is active.", null, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> FreezeInactiveUsersAsync()
    {
        var oneYearAgo = DateTime.UtcNow.AddYears(-1);
        var users = _userManager.Users
            .Where(u => (u.LastLoginDate == null || u.LastLoginDate < oneYearAgo) && !u.IsFrozen)
            .ToList();

        int frozenCount = 0;
        foreach (var user in users)
        {
            user.IsFrozen = true;
            await _userManager.UpdateAsync(user);
            frozenCount++;
        }

        if (frozenCount == 0)
            return new BaseResponse<string>("No inactive users found.", null,HttpStatusCode.NotFound);

        return new BaseResponse<string>($"{frozenCount} inactive users frozen.", null, HttpStatusCode.OK);
    }


    public async Task<BaseResponse<string>> SendEmailAsync(List<string> emails, string subject, string body)
    {
        await _emailService.SendEmailAsync(emails, subject, body);
        return new BaseResponse<string>("Email sent as background job.", null, HttpStatusCode.OK);
    }

}
