using System;

namespace HotelManagementWPF.Models
{
    public static class Session
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUserName { get; set; } = string.Empty;
        public static string CurrentUserRole { get; set; } = string.Empty;

        public static void SetUser(int userId, string userName, string userRole)
        {
            CurrentUserId = userId;
            CurrentUserName = userName ?? string.Empty;
            CurrentUserRole = userRole ?? string.Empty;
        }

        public static void Clear()
        {
            CurrentUserId = 0;
            CurrentUserName = string.Empty;
            CurrentUserRole = string.Empty;
        }

        public static bool IsLoggedIn => CurrentUserId > 0;
    }
}