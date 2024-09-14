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
    public class ScheduleViewModel : INotifyPropertyChanged
    {
        private readonly appDbContext _context;


        public ScheduleViewModel(appDbContext context)
        {
            _context = context;
            LoadSchedules();
            LoadGroups();
            LoadTeachers();
            AddScheduleCommand = new ActionCommand(AddSchedule);
            DeleteScheduleCommand = new ActionCommand(DeleteSchedule);
            NewSchedule = new Schedule();
        }

        private ObservableCollection<Schedule> _schedules;
        public ObservableCollection<Schedule> Schedules
        {
            get { return _schedules; }
            set
            {
                _schedules = value;
                OnPropertyChanged(nameof(Schedules));
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

        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
                LoadGroupSubjects();
            }
        }

        public Schedule NewSchedule { get; set; }
        public ICommand AddScheduleCommand { get; }
        public ICommand DeleteScheduleCommand { get; }

        public async Task LoadSchedules()
        {
            Schedules = new ObservableCollection<Schedule>(await _context.Schedules.Include(s => s.Group).Include(s => s.Subject).Include(s => s.Teacher).ToListAsync());
        }

        public async Task LoadGroups()
        {
            Groups = new ObservableCollection<Group>(await _context.Groups.ToListAsync());
        }

        public async Task LoadSubjects()
        {
            Subjects = new ObservableCollection<Subject>(await _context.Subjects.ToListAsync());
        }

        public async Task LoadTeachers()
        {
            Teachers = new ObservableCollection<Teacher>(await _context.Teachers.ToListAsync());
        }

        private async void LoadGroupSubjects()
        {
            if (SelectedGroup != null)
            {
                Subjects = new ObservableCollection<Subject>(
                    await _context.Subjects
                    .Where(s => s.Groups.Any(g => g.Id == SelectedGroup.Id))
                    .ToListAsync());
            }
        }

        private async void AddSchedule(object parameter)
        {

                var newSchedule = new Schedule
                {
                    GroupId = NewSchedule.GroupId,
                    SubjectId = NewSchedule.SubjectId,
                    TeacherId = NewSchedule.TeacherId,
                    Date = NewSchedule.Date,
                    StartTime = NewSchedule.StartTime,
                    EndTime = NewSchedule.EndTime
                };

                _context.Schedules.Add(newSchedule);
                await _context.SaveChangesAsync();
                Schedules.Add(newSchedule);

                NewSchedule.GroupId = 0;
                NewSchedule.SubjectId = 0;
                NewSchedule.TeacherId = 0;
                NewSchedule.Date = DateTime.Now;
                NewSchedule.StartTime = TimeSpan.Zero;
                NewSchedule.EndTime = TimeSpan.Zero;
                OnPropertyChanged(nameof(NewSchedule));

        }

        private async void DeleteSchedule(object parameter)
        {
            int scheduleId = (int)parameter;
            var schedule = _context.Schedules.FirstOrDefault(s => s.Id == scheduleId);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
                Schedules.Remove(schedule);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
