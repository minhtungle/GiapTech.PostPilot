using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Infrastructure.Helpers
{
    public class UserContext : IUserContext
    {
        private ClaimsPrincipal User => HttpContext.Current?.User as ClaimsPrincipal;

        public Guid UserId =>
            Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty;

        public Guid DonViId =>
            Guid.TryParse(User?.FindFirst("MaDonViSuDung")?.Value, out var id) ? id : Guid.Empty;
    }
}