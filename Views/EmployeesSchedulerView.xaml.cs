using ETAG_ERP.Models;
using ETAG_ERP.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ETAG_ERP.ViewModels
{
    public class EmployeesSchedulerView : BaseViewModel
    {
        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<TaskSchedule> Tasks { get; set; }
        public ObservableCollection<Route> Routes { get; set; }

        private Employee? _selectedEmployee;
        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); LoadEmployeeData(); }
        }

        private TaskSchedule? _selectedTask;
        public TaskSchedule? SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        // أوامر
        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand MarkCompleteCommand { get; }
        public ICommand RefreshCommand { get; }



        private void LoadEmployeeData()
        {
            if (SelectedEmployee == null) return;
            Tasks.Clear();
            Routes.Clear();

            foreach (var t in SelectedEmployee.Tasks) Tasks.Add(t);
            foreach (var r in SelectedEmployee.Routes) Routes.Add(r);
        }

        private void AddTask()
        {
            if (SelectedEmployee == null) return;

            var task = new TaskSchedule
            {
                Id = Tasks.Count + 1,
                EmployeeId = SelectedEmployee.Id,
                TaskName = "زيارة عميل جديد",
                ClientName = "شركة المستقبل",
                Location = "القاهرة - مدينة نصر",
                ScheduledDate = DateTime.Now.Date,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                Status = "مجدول"
            };

            Tasks.Add(task);
            SelectedEmployee.Tasks.Add(task);
        }

        private void EditTask()
        {
            if (SelectedTask == null) return;
            SelectedTask.TaskName += " (معدل)";
            OnPropertyChanged(nameof(Tasks));
        }

        private void DeleteTask()
        {
            if (SelectedTask == null || SelectedEmployee == null) return;
            Tasks.Remove(SelectedTask);
            SelectedEmployee.Tasks.Remove(SelectedTask);
        }

        private void MarkTaskComplete()
        {
            if (SelectedTask == null) return;
            SelectedTask.Status = "مكتمل";
            OnPropertyChanged(nameof(Tasks));
        }

        private void RefreshData()
        {
            LoadEmployeeData();
        }

        private void SeedData()
        {
            var emp1 = new Employee { Id = 1, FullName = "أحمد علي", JobTitle = "مندوب مبيعات", Phone = "01012345678" };
            var emp2 = new Employee { Id = 2, FullName = "منى محمد", JobTitle = "موظفة تسويق", Phone = "01098765432" };

            var task1 = new TaskSchedule { Id = 1, EmployeeId = 1, TaskName = "زيارة عميل", ClientName = "شركة X", Location = "المعادي", ScheduledDate = DateTime.Now.Date, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Status = "مجدول" };
            var task2 = new TaskSchedule { Id = 2, EmployeeId = 2, TaskName = "متابعة عقد", ClientName = "شركة Y", Location = "مصر الجديدة", ScheduledDate = DateTime.Now.Date, StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(13, 0, 0), Status = "جاري" };

            emp1.Tasks.Add(task1);
            emp2.Tasks.Add(task2);

            Employees.Add(emp1);
            Employees.Add(emp2);
        }

        internal void ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}
