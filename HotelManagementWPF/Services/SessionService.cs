using HotelManagementWPF.Models;
using System;

namespace HotelManagementWPF.Services
{
    public class SessionService : ISessionService
    {
        public int? CurrentUserId => Session.IsLoggedIn ? Session.CurrentUserId : (int?)null;
        public string UserName => Session.CurrentUserName;
        public string UserRole => Session.CurrentUserRole;

        public bool IsLoggedIn => Session.IsLoggedIn;

        public void SetUser(int userId, string name, string role)
        {
            Session.SetUser(userId, name, role);
        }

        public void ClearSession()
        {
            Session.Clear();
        }
    }
}