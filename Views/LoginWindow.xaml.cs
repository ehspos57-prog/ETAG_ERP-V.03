using ETAG_ERP.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ETAG_POS
{
    public partial class LoginWindow : Window
    {
        public ObservableCollection<User> Users { get; set; }
        private Button? _selectedButton = null;
        public User SelectedUser { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public LoginWindow()
        {
            InitializeComponent();
            LoadUsers();

            // إضافة حدث التقاط زر Enter في النافذة
            this.PreviewKeyDown += LoginWindow_PreviewKeyDown;
        }

        private void LoginWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;  // منع تعامل آخر مع الزر
                Login_Click(null, null);  // استدعاء حدث الضغط على زر الدخول
            }
        }
        private void LoadUsers()
        {
            Users = new ObservableCollection<User>()
            {
                new User(){ UserName="M.Hussain", Password="102030", ImagePath="Images/admin.png", IsAdmin=true},
                new User(){ UserName="M.Essawy", Password="405060", ImagePath="Images/user1.png", IsAdmin=false},
                new User(){ UserName="Manager", Password="708090", ImagePath="Images/user2.png", IsAdmin=false},
                new User(){ UserName="Dataentry", Password="100100", ImagePath="Images/user3.png", IsAdmin=false},
                new User(){ UserName="Accountant", Password="100200", ImagePath="Images/user4.png", IsAdmin=false},
                new User(){ UserName="Shop", Password="100300", ImagePath="Images/user5.png", IsAdmin=false},
                new User(){ UserName="Workshop", Password="100400", ImagePath="Images/user5.png", IsAdmin=false},
            };

            UsersList.Items.Clear();

            foreach (var user in Users)
            {
                var btn = new Button()
                {
                    Tag = user,
                    Height = 50,
                    Margin = new Thickness(0, 5, 0, 5),
                    Background = Brushes.White,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Cursor = Cursors.Hand,
                    Style = (Style)FindResource("UserButtonStyle")
                };

                var sp = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

                var img = new Image()
                {
                    Source = new BitmapImage(new Uri(uriString: user.ImagePath, UriKind.Relative)),
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(5, 0, 10, 0)
                };

                var tb = new TextBlock()
                {
                    Text = user.UserName,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = user.IsAdmin ? FontWeights.Bold : FontWeights.Normal,
                    FontSize = 16
                };

                sp.Children.Add(img);
                sp.Children.Add(tb);

                btn.Content = sp;
                btn.Click += UserButton_Click;

                UsersList.Items.Add(btn);
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedButton != null)
            {
                _selectedButton.ClearValue(Button.BackgroundProperty);
                _selectedButton.ClearValue(Button.BorderBrushProperty);
                _selectedButton.Foreground = Brushes.Black;
            }

            var btn = sender as Button;
            _selectedButton = btn;

            btn.Background = new SolidColorBrush(Color.FromRgb(74, 144, 226));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(20, 60, 120));
            btn.Foreground = Brushes.White;

            SelectedUser = (User)btn.Tag;
            txtSelectedUser.Text = $"المستخدم المختار: {SelectedUser.UserName}";
        }

        private void Login_Click(object? sender, RoutedEventArgs? e)
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("يرجى اختيار مستخدم أولاً", "خطأ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("يرجى إدخال كلمة المرور", "خطأ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedUser.Password != txtPassword.Password)
            {
                MessageBox.Show("كلمة المرور غير صحيحة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ✅ فتح الشاشة الرئيسية وتعيينها كنافذة التطبيق
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class User
    {
        public string? UserName { get; set; }
        public string? ImagePath { get; set; }
        public bool IsAdmin { get; set; }
        public string Password { get; internal set; }
    }
}
