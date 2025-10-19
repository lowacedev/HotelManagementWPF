using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Services/ISessionService.cs
namespace HotelManagementWPF.Services
{
    public interface ISessionService
    {
        int? CurrentUserId { get; }
        string UserName { get; }
        string UserRole { get; }

        void SetUser(int userId, string name, string role);
        void ClearSession();
        bool IsLoggedIn { get; }
    }
}