using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added
using Module07DataAccess.Model;
using Module07DataAccess.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Module07DataAccess.ViewModel
{
    //Inotifypropertychange for dynamic stuff
    public class PersonalViewModel:INotifyPropertyChanged
    {
        private readonly PersonalService _personalService;
        public ObservableCollection<Personal> PersonalList { get; set; }
        public ObservableCollection<Personal> FilteredPersonalList { get; set; }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterPersonalList();
            }
        }

        private Personal _selectedPersonal;
        public Personal SelectedPersonal
        {
            get => _selectedPersonal;
            set
            {
                _selectedPersonal = value;
                if (_selectedPersonal != null)
                {
                    NewPersonalName = _selectedPersonal.Name;
                    NewPersonalAddress = _selectedPersonal.Address;
                    NewPersonalemail = _selectedPersonal.email;
                    NewPersonalContactNo = _selectedPersonal.ContactNo;
                    IsPersonSelected = true;
                    IsPersonSelectedAdd = false;
                }
                else
                {
                    IsPersonSelected = false;
                    IsPersonSelectedAdd = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _isPersonSelected;
        public bool IsPersonSelected
        {
            get => _isPersonSelected;
            set
            {
                _isPersonSelected = value;
                OnPropertyChanged();
            }
        }
        private bool _isPersonSelectedAdd;
        public bool IsPersonSelectedAdd
        {
            get => _isPersonSelectedAdd;
            set
            {
                _isPersonSelectedAdd = value;
                OnPropertyChanged();
            }
        }
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set 
            { 
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalName;
        public string NewPersonalName
        {
            get => _newPersonalName;
            set
            {
                _newPersonalName = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalemail;
        public string NewPersonalemail
        {
            get => _newPersonalemail;
            set
            {
                _newPersonalemail = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalAddress;
        public string NewPersonalAddress
        {
            get => _newPersonalAddress;
            set
            {
                _newPersonalAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalContactNo;
        public string NewPersonalContactNo
        {
            get => _newPersonalContactNo;
            set
            {
                _newPersonalContactNo = value;
                OnPropertyChanged();
            }
        }
        private void ClearFields()
        {
            NewPersonalName = string.Empty;
            NewPersonalAddress = string.Empty;
            NewPersonalemail = string.Empty;
            NewPersonalContactNo = string.Empty;
        }
        public ICommand LoadDataCommand { get; }
        public ICommand AddPersonalCommand { get; }
        public ICommand UpdatePersonCommand { get; }
        public ICommand SelectedPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }

        //PersonlViewModel Constructor

        public PersonalViewModel()
        {
            _personalService = new PersonalService();
            PersonalList = new ObservableCollection<Personal>();
            //For Filtering
            FilteredPersonalList = new ObservableCollection<Personal>();

            //Initialize the command
            LoadDataCommand = new Command(async () => await LoadData());

            //Add Data
            AddPersonalCommand = new Command(async () => await AddEmployee());

            UpdatePersonCommand = new Command(async () => await UpdatePerson());
            SelectedPersonCommand = new Command<Personal>(person => SelectedPersonal = person);
            DeletePersonCommand = new Command(async () => await DeletePersonal(), () => SelectedPersonal != null);

            LoadData();

        }
        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading Employee Data...";
            try
            {
                var personals = await _personalService.GetAllPersonalsAsync();
                PersonalList.Clear();
                foreach (var personal in personals)
                {

                    var displayPersonal = new Personal
                    {
                        EmployeeId = personal.EmployeeId,
                        Name = personal.Name,
                        Address = personal.Address,
                        email = personal.email,
                        ContactNo = personal.ContactNo,
                        FullInfo = $"Address: {personal.Address}, Email: {personal.email}, Contact No: {personal.ContactNo}"
                    };
                    PersonalList.Add(displayPersonal);
                }
                FilterPersonalList();

                StatusMessage = "Data Loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;

            }
        }

        private async Task AddEmployee()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewPersonalName) || string.IsNullOrWhiteSpace(NewPersonalAddress) || string.IsNullOrWhiteSpace(NewPersonalemail) || string.IsNullOrWhiteSpace(NewPersonalContactNo))
            {
                StatusMessage = "Please Fill in all fields before adding";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new Employee...";

            try
            {
                var newPerson = new Personal
                {
                    Name = NewPersonalName,
                    Address = NewPersonalAddress,
                    email = NewPersonalemail,
                    ContactNo = NewPersonalContactNo,
                };
                var isSuccess = await _personalService.AddPersonalAsync(newPerson);
                if (isSuccess)
                {
                    NewPersonalName = string.Empty;
                    NewPersonalAddress = string.Empty;
                    NewPersonalemail = string.Empty;
                    NewPersonalContactNo = string.Empty;
                    StatusMessage = "New Employee added Successfully";
                }
                else
                {
                    StatusMessage = "Failed to add the new Employee";
                };

            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding Employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadData();
            }
        }

        private async Task DeletePersonal()
        {
            if (SelectedPersonal == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete", $"Are you sure you want to delete {SelectedPersonal.Name}?", "Yes", "No");
            if (!answer) return;
            IsBusy = true;
            StatusMessage = "Deleting Employee";
            try
            {
                var success = await _personalService.DeletePersonalAsync(SelectedPersonal.EmployeeId);
                StatusMessage = success ? "Employee deleted successfully" : "Failed to delete Employee";
                if (success)
                {
                    PersonalList.Remove(SelectedPersonal);
                    SelectedPersonal = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting Employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                //await LoadData();
            }

        }

        private async Task UpdatePerson()
        {
            if (IsBusy || SelectedPersonal == null)
            {
                StatusMessage = "Select a person to update.";
                return;
            }
            IsBusy = true;
            try
            {
                SelectedPersonal.Name = NewPersonalName;
                SelectedPersonal.Address = NewPersonalAddress;
                SelectedPersonal.email = NewPersonalemail;
                SelectedPersonal.ContactNo = NewPersonalContactNo;

                var success = await _personalService.UpdatePersonalAsync(SelectedPersonal);
                StatusMessage = success ? "Person updated successfully!" : "Failed to update person.";

                await LoadData();
                FilterPersonalList();
                ClearFields();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally 
            {
                IsBusy = false;
            }
        }
        private void FilterPersonalList()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                FilteredPersonalList.Clear();
                foreach (var person in PersonalList)
                {
                    FilteredPersonalList.Add(person);
                }
            }
            else
            {
                var filtered = PersonalList.Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                        || p.Address.Contains(SearchText,StringComparison.OrdinalIgnoreCase)
                                                        || p.email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                        || p.ContactNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                FilteredPersonalList.Clear();
                foreach (var person in filtered)
                {
                    FilteredPersonalList.Add(person);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
