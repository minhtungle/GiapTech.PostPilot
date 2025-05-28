using Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.AppServices
{
    public abstract class BaseAppService
    {
        protected readonly IUserContext _userContext;

        protected BaseAppService(IUserContext userContext)
        {
            _userContext = userContext;
        }

        protected Guid CurrentUserId => _userContext.UserId;
        protected Guid CurrentDonViId => _userContext.DonViId;
    }
}