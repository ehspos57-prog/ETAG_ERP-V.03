using ETAG_ERP.Models;
using ETAG_ERP.Views;
using System.Windows.Input;

namespace ETAG_ERP.ViewModels
{
    public class AddEditExpenseViewModel : BaseViewModel
    {
        public Expense Expense { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action? CloseAction { get; set; }


        private void Save(object? obj)
        {
            // استخدام الدوال مباشرة على الكلاس الثابت
            if (Expense.Id == 0)
            {
                DatabaseHelper.InsertExpense(Expense);
            }
            else
            {
                DatabaseHelper.UpdateExpense(Expense);
            }

            CloseAction?.Invoke();
        }

        private void Cancel(object? obj)
        {
            CloseAction?.Invoke();
        }
    }
}
