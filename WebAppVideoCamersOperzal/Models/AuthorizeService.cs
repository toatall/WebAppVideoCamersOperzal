using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebAppVideoCamersOperzal.Models
{
    public class AuthorizeService
    {        
        private readonly IAuthorizationService _authorizationService;
        
        public AuthorizeService(IAuthorizationService authorizationService) 
        {
            _authorizationService = authorizationService;    
        }

        public bool IsAdmin(HttpContext httpContext)
        {
            return Task.Run(() => IsAuthorizeAsAdmin(httpContext)).Result;
        }

        private async Task<bool> IsAuthorizeAsAdmin(HttpContext httpContext)
        {
            var res = await _authorizationService.AuthorizeAsync(httpContext.User, null, "Admin");
            return res.Succeeded;
        }
    }
}
