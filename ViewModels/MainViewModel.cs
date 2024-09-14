using System;
using System.ComponentModel;
using WPFinal.DataBaseFiles;

namespace WPFinal.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly appDbContext _context;

        public MainViewModel()
        {
            _context = new appDbContext();

            GroupViewModel = new GroupViewModel(_context);
            StudentViewModel = new StudentViewModel(_context);
            TeacherViewModel = new TeacherViewModel(_context);
            SubjectViewModel = new SubjectViewModel(_context);
            ScheduleViewModel = new ScheduleViewModel(_context);

            GroupViewModel.GroupsUpdated += OnGroupsUpdated;
            SubjectViewModel.SubjectsUpdated += OnSubjectsUpdated;
            TeacherViewModel.TeachersUpdated += OnTeachersUpdated;
        }

        private void OnGroupsUpdated(object sender, EventArgs e)
        {
            StudentViewModel.LoadGroups();
            ScheduleViewModel.LoadGroups();
        }

        private void OnSubjectsUpdated(object sender, EventArgs e)
        {
            GroupViewModel.LoadSubjects();
            TeacherViewModel.LoadSubjects();
            ScheduleViewModel.LoadSubjects();
        }

        private void OnTeachersUpdated(object sender, EventArgs e)
        {
            ScheduleViewModel.LoadTeachers();
        }

        public async void RefreshData()
        {
            await GroupViewModel.LoadGroups();
            await SubjectViewModel.LoadSubjects();
            await TeacherViewModel.LoadTeachers();
            await StudentViewModel.LoadStudents();
            await ScheduleViewModel.LoadSchedules();
        }

        public StudentViewModel StudentViewModel { get; }
        public GroupViewModel GroupViewModel { get; }
        public TeacherViewModel TeacherViewModel { get; }
        public SubjectViewModel SubjectViewModel { get; }
        public ScheduleViewModel ScheduleViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
