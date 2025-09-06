using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ETAG_ERP.Views
{
    public partial class AddClientWindow : Window
    {
        public Client Client { get; set; } = new Client();

        public AddClientWindow(Client clone)
        {
            InitializeComponent();
        }

        public AddClientWindow()
        {
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClientNameTextBox.Text))
            {
                MessageBox.Show("يرجى إدخال اسم العميل.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // تعبئة بيانات العميل
            Client.Name = ClientNameTextBox.Text.Trim();
            Client.Address = AddressTextBox.Text.Trim();
            Client.Phone = PhoneTextBox.Text.Trim();
            Client.Fax = FaxTextBox.Text.Trim();
            Client.Email = EmailTextBox.Text.Trim();
            Client.BusinessField = BusinessFieldTextBox.Text.Trim();

            // تقييم الشركة
            Client.EngineerName = EngineerNameTextBox.Text.Trim();
            Client.EvaluationDate = EvaluationDatePicker.SelectedDate ?? DateTime.Now;
            Client.Evaluator = EvaluatorTextBox.Text.Trim();
            Client.CompanyEvaluation = CompanyEvaluationTextBox.Text.Trim();

            // خانات التقييم
            Client.RatingGood = RatingGoodCheckBox.IsChecked ?? false;
            Client.RatingAverage = RatingAverageCheckBox.IsChecked ?? false;
            Client.RatingPoor = RatingPoorCheckBox.IsChecked ?? false;

            // مسؤلين التواصل
            Client.Contacts = ContactsDataGrid.Items.OfType<ContactPerson>().ToList();

            try
            {
                DatabaseHelper.InsertClient(Client);
                MessageBox.Show("تم حفظ العميل بنجاح ✅", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // مهم لإغلاق ShowDialog
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء الحفظ:\n" + ex.Message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== كارتة عميل ===");
                sb.AppendLine($"اسم العميل: {ClientNameTextBox.Text}");
                sb.AppendLine($"العنوان: {AddressTextBox.Text}");
                sb.AppendLine($"الهاتف: {PhoneTextBox.Text}");
                sb.AppendLine($"فاكس: {FaxTextBox.Text}");
                sb.AppendLine($"البريد الإلكتروني: {EmailTextBox.Text}");
                sb.AppendLine($"مجال العمل: {BusinessFieldTextBox.Text}");
                sb.AppendLine("----- تقييم الشركة -----");
                sb.AppendLine($"المهندس: {EngineerNameTextBox.Text}");
                sb.AppendLine($"تاريخ التقييم: {EvaluationDatePicker.SelectedDate?.ToShortDateString()}");
                sb.AppendLine($"المسؤول عن التقييم: {EvaluatorTextBox.Text}");
                sb.AppendLine($"التقييم النصي: {CompanyEvaluationTextBox.Text}");

                string rating = "";
                if (RatingGoodCheckBox.IsChecked == true) rating += "جيد ";
                if (RatingAverageCheckBox.IsChecked == true) rating += "متوسط ";
                if (RatingPoorCheckBox.IsChecked == true) rating += "مقبول ";
                sb.AppendLine($"التقييم المختار: {rating}");


                FlowDocument doc = new FlowDocument(new Paragraph(new Run(sb.ToString())))
                {
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    FontSize = 14,
                    FlowDirection = FlowDirection.RightToLeft
                };

                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "طباعة كارتة عميل");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء الطباعة:\n" + ex.Message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
