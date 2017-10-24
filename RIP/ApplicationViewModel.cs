using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Data.Entity;
using System.Windows;

namespace RIP
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        AppContext db;

        private Company selectedCompany;
        private User selectedUser;

        public ObservableCollection<Company> CompaniesCollection { get; set; }
        public ObservableCollection<User> UsersCollection { get; set; }

        public ObservableCollection<User> ChangedUserCollection { get; set; }
        public ObservableCollection<Company> ChangedCompanyCollection { get; set; }

        public Company SelectedCompany
        {
            get { return selectedCompany; }
            set
            {
                selectedCompany = value;
                if (selectedCompany != null)
                    sortUsers(selectedCompany.CompanyId);
                try
                {
                    db.SaveChanges();
                }
                catch 
                {
                    MessageBoxResult result = MessageBox.Show("Opps! Something wrong :-(");
                }
                OnPropertyChanged("SelectedCompany");
            }
        }

        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;

                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    MessageBoxResult result = MessageBox.Show("Opps! Something wrong :-(");
                }

                OnPropertyChanged("SelectedUser");
            }
        }

        public ApplicationViewModel()
        {
            db = new AppContext();

            ChangedUserCollection = new ObservableCollection<User>();
            ChangedCompanyCollection = new ObservableCollection<Company>();

            UsersCollection = new ObservableCollection<User>();
            foreach (User item in db.Users)
            {
                UsersCollection.Add(item);
            }

            CompaniesCollection = new ObservableCollection<Company>();
            foreach (Company item in db.Companies)
            {
                CompaniesCollection.Add(item);
            }
        }

        //добавить компанию
        private RelayCommand addCompanyCommand;
        public RelayCommand AddCompanyCommand
        {
            get
            {
                return addCompanyCommand ??
                  (addCompanyCommand = new RelayCommand(obj =>
                  {
                      Company company = new Company();
                      ChangedCompanyCollection.Clear();
                      ChangedCompanyCollection.Add(company);
                      SelectedCompany = company;
                  },
                  (obj) => ChangedCompanyCollection.Count == 0));
            }
        }

        //добавить пользователя
        private RelayCommand addUserCommand;
        public RelayCommand AddUserCommand
        {
            get
            {
                return addUserCommand ??
                  (addUserCommand = new RelayCommand(obj =>
                  {
                      User user = new User();
                      if (selectedCompany != null)
                      {
                          user.CompanyId = selectedCompany.CompanyId;
                          ChangedUserCollection.Clear();
                          ChangedUserCollection.Add(user);
                          SelectedUser = user;
                      }
                      else
                      {
                          MessageBoxResult result = MessageBox.Show("Company is not selected!");
                      } 

                  },
                 (obj) => ChangedUserCollection.Count == 0));
            }
        }

        // удалить компанию
        private RelayCommand removeCompanyCommand;
        public RelayCommand RemoveCompanyCommand
        {
            get
            {
                return removeCompanyCommand ??
                  (removeCompanyCommand = new RelayCommand(obj =>
                  {

                      Company company = obj as Company;
                      if (company != null)
                      {
                          foreach (User user in db.Users)
                          {
                              if (user.CompanyId == company.CompanyId)
                              {
                                  UsersCollection.Remove(user);
                                  db.Users.Remove(user);
                              }

                          }

                          CompaniesCollection.Remove(company);
                          ChangedCompanyCollection.Remove(company);
                          if (CompaniesCollection.Count > 0)
                              SelectedCompany = CompaniesCollection.ElementAt(0);
                          else
                              SelectedCompany = null;
                          db.Companies.Remove(company);
                          db.SaveChanges();
                      }
                  },
                 (obj) => selectedCompany != null));
            }
        }

        // удалить пользователя
        private RelayCommand removeUserCommand;
        public RelayCommand RemoveUserCommand
        {
            get
            {
                return removeUserCommand ??
                  (removeUserCommand = new RelayCommand(obj =>
                  {
                      User user = obj as User;
                      if (user != null)
                      {
                          if (UsersCollection.Count > 0)
                              SelectedUser = UsersCollection.ElementAt(0);
                          else
                              SelectedUser = null;
                          ChangedUserCollection.Remove(user);
                          db.Users.Remove(user);
                          db.SaveChanges();
                          UsersCollection.Remove(user);
                      }
                  },
                 (obj) => selectedUser != null));
            }
        }

        //сохранить
        private RelayCommand saveUserCommand;
        public RelayCommand SaveUserCommand
        {
            get
            {
                return saveUserCommand ??
                  (saveUserCommand = new RelayCommand(obj =>
                  {
                      foreach (User user in ChangedUserCollection)
                      {
                          if (user != null)
                          {

                              if (user.CompanyId > 0)
                              {
                                  db.Entry(user).State = EntityState.Modified;
                              }
                              else
                              {
                                  db.Entry(user).State = EntityState.Added;
                              }
                              UsersCollection.Add(user);
                              db.Users.Add(user);
                          }
                      }
                      ChangedUserCollection.Clear();
                      try
                      {
                          db.SaveChanges();
                      }
                      catch
                      {
                          MessageBoxResult result = MessageBox.Show("Opps! Something wrong :-(");
                      }
                  },
                 (obj) => ChangedUserCollection.Count > 0));
            }
        }

        //сохранить
        private RelayCommand saveCompanyCommand;
        public RelayCommand SaveCompanyCommand
        {
            get
            {
                return saveCompanyCommand ??
                  (saveCompanyCommand = new RelayCommand(obj =>
                  {
                      foreach (Company company in ChangedCompanyCollection)
                      {
                          if (company != null)
                          {
                              CompaniesCollection.Add(company);
                              db.Companies.Add(company);
                          }
                      }
                      ChangedCompanyCollection.Clear();
                      try
                      {
                          db.SaveChanges();
                      }
                      catch
                      {
                          MessageBoxResult result = MessageBox.Show("Opps! Something wrong :-(");
                      }

                  },
                 (obj) => ChangedCompanyCollection.Count > 0));
            }
        }

        //отменить создание пользователя
        private RelayCommand cancelUserCommand;
        public RelayCommand CancelUserCommand
        {
            get
            {
                return cancelUserCommand ??
                  (cancelUserCommand = new RelayCommand(obj =>
                  {
                      User user = obj as User;
                      SelectedUser = null;
                      ChangedUserCollection.Clear();
                  },
                 (obj) => ChangedUserCollection.Count > 0));
            }
        }

        //отменить создание компании
        private RelayCommand cancelCompanyCommand;
        public RelayCommand CancelCompanyCommand
        {
            get
            {
                return cancelCompanyCommand ??
                  (cancelCompanyCommand = new RelayCommand(obj =>
                  {
                      Company company = obj as Company;
                      SelectedCompany = CompaniesCollection.ElementAt(0);
                      ChangedCompanyCollection.Clear();
                  },
                 (obj) => ChangedCompanyCollection.Count > 0));
            }
        }

        //сортировать список пользователей по выбранной компании
        private void sortUsers(int parametr)
        {
            UsersCollection.Clear();
            foreach (User item in db.Users)
            {
                if (item.CompanyId.Equals(parametr))
                {
                    UsersCollection.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
