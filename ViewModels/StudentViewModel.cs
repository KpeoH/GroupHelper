using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFinal.DataBaseFiles;
using WPFinal.Models;
using WPFinal.ViewModels.Commands;

namespace WPFinal.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private readonly appDbContext _context;

        public StudentViewModel(appDbContext context)
        {
            _context = context;
            LoadStudents();
            LoadGroups();
            AddStudentCommand = new ActionCommand(AddStudent);
            DeleteStudentCommand = new ActionCommand(DeleteStudent);
            NewStudent = new Student();
        }

        private ObservableCollection<Student> _students;
        public ObservableCollection<Student> Students
        {
            get { return _students; }
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Students));
            }
        }

        private ObservableCollection<Group> _groups;
        public ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }

        public Student NewStudent { get; set; }

        public ICommand AddStudentCommand { get; }
        public ICommand DeleteStudentCommand { get; }

        public async Task LoadStudents()
        {
            Students = new ObservableCollection<Student>(await _context.Students.Include(s => s.Group).ToListAsync());
        }

        public async Task LoadGroups()
        {
            Groups = new ObservableCollection<Group>(await _context.Groups.ToListAsync());
        }

        private async void AddStudent(object parameter)
        {
            var newStudent = new Student
            {
                FirstName = NewStudent.FirstName,
                LastName = NewStudent.LastName,
                DateOfBirth = NewStudent.DateOfBirth,
                GroupId = NewStudent.GroupId
            };

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();
            Students.Add(newStudent);

            NewStudent.FirstName = string.Empty;
            NewStudent.LastName = string.Empty;
            NewStudent.DateOfBirth = DateTime.Now;
            NewStudent.GroupId = 0;
            OnPropertyChanged(nameof(NewStudent));


        }

        private async void DeleteStudent(object parameter)
        {
            int studentId = (int)parameter;
            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                Students.Remove(student);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
