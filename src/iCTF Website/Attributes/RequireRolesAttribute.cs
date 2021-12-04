using iCTF_Shared_Resources.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace iCTF_Website.Attributes
{
    public class RequireRolesAttribute : ActionFilterAttribute
    {
        public string[] Roles { get; set; }

        public RequireRolesAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
            var apiKey = context.HttpContext.Request.Query["apikey"];
            if (apiKey == StringValues.Empty)
            {
                context.Result = new  StatusCodeResult(403);
                return;
            }
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.ApiKey == apiKey.ToString());
            if (user == null)
            {
                context.Result = new StatusCodeResult(403);
                return;
            }
            var roles = await userManager.GetRolesAsync(user);
            if (!roles.Intersect(Roles).Any())
            {
                context.Result = new StatusCodeResult(403);
                return;
            }

            await next();
        }
    }
}
