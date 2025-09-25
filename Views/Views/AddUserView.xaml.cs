// Views/AddUserWindow.xaml.cs
using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class AddUserWindow : Window
    {
        private User _editingUser = null;

        public AddUserWindow()
        {
            InitializeComponent();
            cmbRole.SelectedIndex = 0;
        }

        public AddUserWindow(User existingUser) : this()
        {
            _editingUser = existingUser;

            txtUsername.Text = existingUser.Username;
            txtPassword.Password = existingUser.Password;
            txtFullName.Text = existingUser.FullName;

            // ضبط الرتبة
            bool matched = false;
            foreach (ComboBoxItem item in cmbRole.Items)
            {
                if (string.Equals(item.Content?.ToString(), existingUser.Role, StringComparison.OrdinalIgnoreCase))
                {
                    cmbRole.SelectedItem = item;
                    matched = true;
                    break;
                }
            }
            if (!matched) cmbRole.SelectedIndex = 0;

            // تحميل الصلاحيات
            if (!string.IsNullOrWhiteSpace(existingUser.Permissions))
            {
                var perms = existingUser.Permissions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                SetPermissions(perms);
            }
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();
            string fullName = txtFullName.Text.Trim();
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("يرجى ملء جميع الحقول المطلوبة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string permissions = GetSelectedPermissions();

            try
            {
                if (_editingUser == null)
                {
                    var newUser = new User
                    {
                        Username = username,
                        Password = password,
                        FullName = fullName,
                        Role = role,
                        Permissions = permissions
                    };

                    DatabaseHelper.InsertUser(newUser);
                }
                else
                {
                    _editingUser.Username = username;
                    _editingUser.Password = password;
                    _editingUser.FullName = fullName;
                    _editingUser.Role = role;
                    _editingUser.Permissions = permissions;

                    DatabaseHelper.UpdateUser(_editingUser);
                }

                MessageBox.Show("تم حفظ المستخدم بنجاح ✅", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الحفظ:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSelectedPermissions()
        {
            var selected = PermissionsPanel.Children
                                           .OfType<CheckBox>()
                                           .Where(c => c.IsChecked == true)
                                           .Select(c => c.Content?.ToString() ?? string.Empty)
                                           .Where(s => !string.IsNullOrWhiteSpace(s))
                                           .ToArray();

            return string.Join(",", selected);
        }

        private void SetPermissions(IEnumerable<string> permissions)
        {
            var set = new HashSet<string>(permissions.Where(p => !string.IsNullOrWhiteSpace(p)),
                                          StringComparer.OrdinalIgnoreCase);

            foreach (var chk in PermissionsPanel.Children.OfType<CheckBox>())
            {
                var label = chk.Content?.ToString() ?? string.Empty;
                chk.IsChecked = set.Contains(label);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
