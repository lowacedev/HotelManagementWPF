using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManagementWPF.Models
{
    public class EmployeeModel : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private int _age;
        private string _gender;
        private string _jobTitle;
        private string _department;
        private string _phoneNumber;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public string JobTitle
        {
            get => _jobTitle;
            set
            {
                _jobTitle = value;
                OnPropertyChanged();
            }
        }

        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}