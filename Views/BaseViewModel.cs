using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETAG_ERP.Views // *** هام: هذا الـ Namespace يجب أن يتطابق مع الـ Namespace الرئيسي لمشروعك + اسم مجلد الـ ViewModels الخاص بك (Views) ***
{
    public class BaseViewModel : INotifyPropertyChanged
    {
#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}