using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using RegIN_Kantuganov.Classes;

namespace RegIN_Kantuganov.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        string OldLogin;
        int CountSetPassword = 2;
        bool IsCapture = false;
        public Login()
        {
            InitializeComponent();
            MainWindow.mainWindow.UserLogIn.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogIn.HandlerInCorrectLogin += InCorrectLogin;
            Capture.HandlerCorrectCapture += CorrectCapture;
        }
        public void CorrectLogin()
        {
            if (OldLogin != tbLogin.Text)
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);

                try
                {
                    BitmapImage biling = new BitmapImage();
                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogIn.Image);
                    biling.BeginInit();
                    biling.StreamSource = ms;
                    biling.EndInit();
                    ImageSource imgSrc = biling;

                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    StartAnimation.Completed += delegate
                    {
                        IUser.Source = imgSrc;
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        EndAnimation.From = 0;
                        EndAnimation.To = 1;
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };

                    IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }

                OldLogin = tbLogin.Text;
            }
        }

        private void InCorrectLogin()
        {
            if (LNameUser.Content != "")
            {
                LNameUser.Content = "";
                DoubleAnimation StartAnimation = new DoubleAnimation();
                StartAnimation.From = 1;
                StartAnimation.To = 0;
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                StartAnimation.Completed += delegate
                {
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,./Images/ic-user.png"));
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    IUser.BeginAnimation(OpacityProperty, EndAnimation);
                };
                IUser.BeginAnimation(OpacityProperty, StartAnimation);
            }

            if (!(tbLogin.Text.Length > 0))
            {
                SetNotification("Login is incorrect", Brushes.Red);
            }
        }

        private void CorrectCapture() {
            Capture.IsEnabled = false;
            IsCapture = true;
            SendNewPassword();
        }

        public void SendNewPassword()
        {
            if (IsCapture)
            {
                if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
                {
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    StartAnimation.Completed += delegate
                    {
                        IUser.Source = new BitmapImage(new Uri("pack://application:,,./Images/ic-mail.png"));
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        EndAnimation.From = 0;
                        EndAnimation.To = 1;
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        IUser.BeginAnimation(OpacityProperty, EndAnimation);
                    };
                    IUser.BeginAnimation(OpacityProperty, StartAnimation);
                    SetNotification("An email has been sent to your email.", Brushes.Black);
                    MainWindow.mainWindow.UserLogIn.CreateNewPassword();
                }
            }
        }
        private void SetLogin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.mainWindow.UserLogIn.GetUserLogin(tbLogin.Text);
            }
            if (tbPassword.Password.Length > 0)
                SetPassword();
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.UserLogIn.GetUserLogin(tbLogin.Text);
            if (tbPassword.Password.Length > 0)
                SetPassword();
        }

        private void SetPassword(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SetPassword();
        }

        public void SetPassword()
        {
            if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
            {
                if (IsCapture)
                {
                    if (MainWindow.mainWindow.UserLogIn.Password == tbPassword.Password)
                    {
                        MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Login));
                    }
                    else
                    {
                        if (CountSetPassword > 0)
                        {
                            SetNotification($"Password is Incorrect, {CountSetPassword} attempts left", Brushes.Red);
                            CountSetPassword--;
                        }
                        else
                        {
                            Thread TBlockAutorization = new Thread(BlockAutorization);
                            TBlockAutorization.Start();
                        }
                        SendMail.SendMessage("An attempt was made to log into your account.", MainWindow.mainWindow.UserLogIn.Login);
                    }
                }
                else
                    SetNotification("Enter capture", Brushes.Red);
            }
        }

        public void BlockAutorization()
        {
            DateTime StartBlock = DateTime.Now.AddMinutes(3);
            Dispatcher.Invoke(() =>
            {
                tbLogin.IsEnabled = false;
                tbPassword.IsEnabled = false;
                Capture.IsEnabled = false;
            });

            for (int i = 0; i < 180; i++)
            {
                TimeSpan TimeId = StartBlock.Subtract(DateTime.Now);
                string s_minutes = TimeId.Minutes.ToString();
                if (TimeId.Minutes < 10)
                    s_minutes = "0" + TimeId.Minutes;
                string s_seconds = TimeId.Seconds.ToString();
                if (TimeId.Seconds < 10)
                    s_seconds = "0" + TimeId.Seconds;

                Dispatcher.Invoke(() =>
                {
                    SetNotification($"Reauthorization available in: {s_minutes}:{s_seconds}", Brushes.Red);
                });

                Thread.Sleep(1000);
            }

            Dispatcher.Invoke(() =>
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                tbLogin.IsEnabled = true;
                tbPassword.IsEnabled = true;
                Capture.IsEnabled = true;
                Capture.CreateCapture();
                IsCapture = false;
                CountSetPassword = 2;
            });
        }

        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            LNameUser.Content = Message;
            LNameUser.Foreground = _Color;
        }

        private void RecoveryPassword(object sender, MouseButtonEventArgs e) =>
    MainWindow.mainWindow.OpenPage(new Recovery());

        private void OpenRegin(object sender, MouseButtonEventArgs e) =>
            MainWindow.mainWindow.OpenPage(new Regin());
    }
}
