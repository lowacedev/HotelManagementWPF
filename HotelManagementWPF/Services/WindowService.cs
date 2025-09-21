using HotelManagementWPF.Views.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HotelManagementWPF.Services
{
    public class WindowService : IWindowService
    {
        public void ShowAddRoomDialog()
        {
            var window = new AddRoomFormView();
            window.ShowDialog(); 
        }
        public void ShowAddRoomForm()
        {
            var addRoomForm = new AddRoomFormView();
            addRoomForm.Owner = Application.Current.MainWindow;
            addRoomForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addRoomForm.ShowDialog(); 
        }

        public void ShowEditRoomForm(Models.Room room)
        {
            var editForm = new Views.Room.EditRoomFormView(room);
            editForm.Owner = Application.Current.MainWindow;
            var result = editForm.ShowDialog();

            if (result == true)
            {
                // Refresh the room list after successful edit
                // You might want to raise an event or call a refresh method
            }
        }
    }
}