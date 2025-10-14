using System.Windows;

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
