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
    public class GroupViewModel : INotifyPropertyChanged
    {
        private readonly appDbContext _context;


        public GroupViewModel(appDbContext context)
        {
            _context = context;
            LoadGroups();
            LoadSubjects();
            LoadStudents();
            RefreshCommand = new ActionCommand(Refresh);
            AddGroupCommand = new ActionCommand(AddGroup);
            AssignSubjectCommand = new ActionCommand(AssignSubject);
            DeleteGroupCommand = new ActionCommand(DeleteGroup);
            NewGroup = new Group();
            SelectedGroup = null;
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

        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }

        public Group NewGroup { get; set; }
        public Subject SelectedSubject { get; set; }
        public Student SelectedStudent { get; set; }
        public ICommand RefreshCommand { get; }
        public ICommand AddGroupCommand { get; }
        public ICommand AssignSubjectCommand { get; }
        public ICommand DeleteGroupCommand { get; }

        public event EventHandler GroupsUpdated;

        public async Task LoadGroups()
        {
            Groups = new ObservableCollection<Group>(
                await _context.Groups.Include(g => g.Subjects).Include(g => g.Students).ToListAsync());

            foreach (var group in Groups)
            {
                group.Subjects = new ObservableCollection<Subject>(
                    await _context.Subjects
                        .Where(s => s.Groups.Any(g => g.Id == group.Id))
                        .ToListAsync());

                group.Students = await _context.Students
                        .Where(s => s.GroupId == group.Id)
                        .ToListAsync();
            }
        }

        private async void Refresh(object parameter)
        {
            await LoadGroups();
        }

        public async Task LoadSubjects()
        {
            Subjects = new ObservableCollection<Subject>(await _context.Subjects.ToListAsync());
        }

        public async Task LoadStudents()
        {
            Students = new ObservableCollection<Student>(await _context.Students.ToListAsync());
        }

        private async void AddGroup(object parameter)
        {
            var newGroup = new Group
            {
                Name = NewGroup.Name,
                Subjects = new ObservableCollection<Subject>()
            };

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();
            Groups.Add(newGroup);

            NewGroup.Name = string.Empty;
            OnPropertyChanged(nameof(NewGroup));

            GroupsUpdated?.Invoke(this, EventArgs.Empty);
        }

        private async void AssignSubject(object parameter)
        {
            if (SelectedGroup != null && SelectedSubject != null)
            {
                if (SelectedGroup.Subjects == null)
                {
                    SelectedGroup.Subjects = new ObservableCollection<Subject>();
                }

                SelectedGroup.Subjects.Add(SelectedSubject);

                await _context.SaveChangesAsync();

                await LoadGroups();
            }
        }

        //private async Task UpdateGroupSubjects()
        //{
        //    if (SelectedGroup != null)
        //    {
        //        var subjects = await _context.Subjects
        //            .Where(s => s.Groups.Any(g => g.Id == SelectedGroup.Id))
        //            .ToListAsync();

        //        SelectedGroup.Subjects.Clear();
        //        foreach (var subject in subjects)
        //        {
        //            SelectedGroup.Subjects.Add(subject);
        //        }

        //        OnPropertyChanged(nameof(SelectedGroup));
        //        OnPropertyChanged(nameof(Groups));
        //    }
        //}

        private async void DeleteGroup(object parameter)
        {
            int groupId = (int)parameter;
            var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
                Groups.Remove(group);

                GroupsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
