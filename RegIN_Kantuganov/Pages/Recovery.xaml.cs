using Mysqlx.Crud;
using RegIN_Kantuganov.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static RegIN_Kantuganov.Classes.User;
using static RegIN_Kantuganov.Elements.ElementCapture;

namespace RegIN_Kantuganov.Pages
{

    public partial class Recovery : Page
    {
        private string _oldLogin;
        private bool _isCapture = false;

        public Recovery()
        {
            InitializeComponent();

            var mainWindow = MainWindow.mainWindow;
            var userLogIn = mainWindow.UserLogIn;

            userLogIn.HandlerCorrectLogin += CorrectLogin;
            userLogIn.HandlerInCorrectLogin += IncorrectLogin;
            Capture.HandlerCorrectCapture += CorrectCapture;
        }

        private void CorrectLogin()
        {
            if (_oldLogin != TbLogin.Text)
            {
                SetNotification($"Hi, {MainWindow.mainWindow.UserLogIn.Name}", Brushes.Black);
                UpdateUserImage();
                _oldLogin = TbLogin.Text;

                SendNewPassword();
            }
        }


        private void IncorrectLogin()
        {
            if (!string.IsNullOrEmpty(lNameUser.Content?.ToString()))
            {
                lNameUser.Content = "";
                AnimateImageTransition(new Uri("pack://application:,,,/Images/ic-user.png"));
            }

            if (TbLogin.Text.Length > 0)
                SetNotification("Login is incorrect", Brushes.Red);
        }

        private void CorrectCapture()
        {
            Capture.IsEnabled = false;
            _isCapture = true;
            SendNewPassword();
        }

        private void SetLogin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
        }

        private void SetLogin() => MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);

        public void SendNewPassword()
        {
            SetNotification("An email has been sent to your email.", Brushes.Black);
            MainWindow.mainWindow.UserLogIn.CrateNewPassword();
        }


        public void SetNotification(string message, SolidColorBrush color)
        {
            lNameUser.Content = message;
            lNameUser.Foreground = color;
        }

        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }


        private void UpdateUserImage()
        {
            try
            {
                var userImage = MainWindow.mainWindow.UserLogIn.Image;
                if (userImage == null || userImage.Length == 0) return;

                var bitmapImage = new BitmapImage();
                using (var memoryStream = new MemoryStream(userImage))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }

                AnimateImageTransition(bitmapImage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AnimateImageTransition(ImageSource newImageSource)
        {
            AnimateImageTransition(image => IUser.Source = newImageSource);
        }

        private void AnimateImageTransition(Uri imageUri)
        {
            AnimateImageTransition(image => IUser.Source = new BitmapImage(imageUri));
        }

        private void AnimateImageTransition(Action<Image> setImageAction)
        {
            var startAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.6)
            };

            startAnimation.Completed += (sender, e) =>
            {
                setImageAction(IUser);

                var endAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(1.2)
                };

                IUser.BeginAnimation(OpacityProperty, endAnimation);
            };

            IUser.BeginAnimation(OpacityProperty, startAnimation);
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {

        }
    }
}
