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
    public class TeacherViewModel : INotifyPropertyChanged
    {
        private readonly appDbContext _context;

        public ICommand DeleteTeacherCommand { get; }

        public TeacherViewModel(appDbContext context)
        {
            _context = context;
            LoadTeachers();
            LoadSubjects();
            AddTeacherCommand = new ActionCommand(AddTeacher);
            AssignSubjectCommand = new ActionCommand(AssignSubject);
            DeleteTeacherCommand = new ActionCommand(DeleteTeacher);
            NewTeacher = new Teacher();
            SelectedTeacher = null;
        }

        private ObservableCollection<Teacher> _teachers;
        public ObservableCollection<Teacher> Teachers
        {
            get { return _teachers; }
            set
            {
                _teachers = value;
                OnPropertyChanged(nameof(Teachers));
            }
        }

        private ObservableCollection<Subject> _subjects;
        public ObservableCollection<Subject> Subjects
        {
            get { return _subjects; }
            set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        private Teacher _selectedTeacher;
        public Teacher SelectedTeacher
        {
            get { return _selectedTeacher; }
            set
            {
                _selectedTeacher = value;
                OnPropertyChanged(nameof(SelectedTeacher));
                UpdateTeacherSubjects();
            }
        }

        public Teacher NewTeacher { get; set; }
        public Subject SelectedSubject { get; set; }
        public ICommand AddTeacherCommand { get; }
        public ICommand AssignSubjectCommand { get; }

        public event EventHandler TeachersUpdated;

        public async Task LoadTeachers()
        {
            Teachers = new ObservableCollection<Teacher>(
                await _context.Teachers.Include(t => t.Subjects).ToListAsync());

            foreach (var teacher in Teachers)
            {
                teacher.Subjects = new ObservableCollection<Subject>(
                    await _context.Subjects
                        .Where(s => s.Teachers.Any(t => t.Id == teacher.Id))
                        .ToListAsync());
            }
        }

        public async Task LoadSubjects()
        {
            Subjects = new ObservableCollection<Subject>(await _context.Subjects.ToListAsync());
        }

        private async void AddTeacher(object parameter)
        {
            var newTeacher = new Teacher
            {
                FirstName = NewTeacher.FirstName,
                LastName = NewTeacher.LastName,
                Subjects = new ObservableCollection<Subject>()
            };

            _context.Teachers.Add(newTeacher);
            await _context.SaveChangesAsync();
            Teachers.Add(newTeacher);

            NewTeacher.FirstName = string.Empty;
            NewTeacher.LastName = string.Empty;
            OnPropertyChanged(nameof(NewTeacher));

            TeachersUpdated?.Invoke(this, EventArgs.Empty);
        }

        private async void AssignSubject(object parameter)
        {
            if (SelectedTeacher != null && SelectedSubject != null)
            {
                if (SelectedTeacher.Subjects == null)
                {
                    SelectedTeacher.Subjects = new ObservableCollection<Subject>();
                }

                SelectedTeacher.Subjects.Add(SelectedSubject);

                await _context.SaveChangesAsync();

                // Тут очень важно. Последний вызов обновляет препода с предметами. Иначе не получилось.
                await UpdateTeacherSubjects();
                await LoadTeachers();
            }
        }

        private async Task UpdateTeacherSubjects()
        {
            if (SelectedTeacher != null)
            {
                var subjects = await _context.Subjects
                    .Where(s => s.Teachers.Any(t => t.Id == SelectedTeacher.Id))
                    .ToListAsync();

                SelectedTeacher.Subjects.Clear();
                foreach (var subject in subjects)
                {
                    SelectedTeacher.Subjects.Add(subject);
                }

                OnPropertyChanged(nameof(SelectedTeacher));
                OnPropertyChanged(nameof(Teachers));
            }
        }

        private async void DeleteTeacher(object parameter)
        {
            int teacherId = (int)parameter;
            var teacher = _context.Teachers.FirstOrDefault(t => t.Id == teacherId);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                Teachers.Remove(teacher);

                TeachersUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
