using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ETAG_ERP.Views
{
    public partial class ReceivingNoteView : Window
    {
        public ReceivingNoteView()
        {
            InitializeComponent();
            this.DataContext = new ReceivingNoteViewModel();
        }
    }
}
