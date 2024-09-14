using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFinal.DataBaseFiles;
using WPFinal.Models;
using WPFinal.ViewModels.Commands;

namespace WPFinal.ViewModels
{
    public class SubjectViewModel : INotifyPropertyChanged
    {
        private readonly appDbContext _context;

        public SubjectViewModel(appDbContext context)
        {
            _context = context;
            LoadSubjects();
            AddSubjectCommand = new ActionCommand(AddSubject);
            DeleteSubjectCommand = new ActionCommand(DeleteSubject);
            NewSubject = new Subject();
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

        public Subject NewSubject { get; set; }
        public ICommand AddSubjectCommand { get; }
        public ICommand DeleteSubjectCommand { get; }

        public event EventHandler SubjectsUpdated;

        public async Task LoadSubjects()
        {
            Subjects = new ObservableCollection<Subject>(await _context.Subjects.Include(s => s.Teachers).Include(s => s.Groups).ToListAsync());
        }

        private async void AddSubject(object parameter)
        {
            var newSubject = new Subject
            {
                Name = NewSubject.Name
            };

            _context.Subjects.Add(newSubject);
            await _context.SaveChangesAsync();
            Subjects.Add(newSubject);

            NewSubject.Name = string.Empty;
            OnPropertyChanged(nameof(NewSubject));

            SubjectsUpdated?.Invoke(this, EventArgs.Empty);
        }

        private async void DeleteSubject(object parameter)
        {
            int subjectId = (int)parameter;
            var subject = _context.Subjects.FirstOrDefault(s => s.Id == subjectId);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                Subjects.Remove(subject);

                SubjectsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
