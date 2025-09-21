using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Services
{
    public interface IWindowService
    {
        void ShowAddRoomDialog();
        void ShowAddRoomForm();
        void ShowEditRoomForm(Models.Room room);
    }
