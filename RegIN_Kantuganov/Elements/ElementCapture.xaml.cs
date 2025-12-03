using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegIN_Kantuganov.Elements
{
    /// <summary>
    /// Логика взаимодействия для ElementCapture.xaml
    /// </summary>
    public partial class ElementCapture : UserControl
    {
        public CorrectCapture HandlerCorrectCapture;
        public delegate void CorrectCapture();

        public string StrCapture;
        public int WidthCapture = 280, HeightCapture = 50;
        public ElementCapture()
        {
            InitializeComponent();
            CreateCapture();
        }
        public void CreateCapture()
        {
            InputCapture.Text = "";
            Capture.Children.Clear();
            StrCapture = "";
            CreateBackground();
            Background();
        }

        void CreateBackground() {
            Random ThisRandom = new Random();
            for (int i = 0; i < 100; i++)
            {
                int Number = ThisRandom.Next(0, 9);
                Label lNumber = new Label()
                {
                    Content = Number,
                    FontSize = ThisRandom.Next(10, 16),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(
                        Color.FromArgb(
                            100,
                            (byte)ThisRandom.Next(0, 255),
                            (byte)ThisRandom.Next(0, 255),
                            (byte)ThisRandom.Next(0, 255))),
                    Margin = new Thickness(
                        ThisRandom.Next(0, WidthCapture - 20),
                        ThisRandom.Next(0, HeightCapture - 20), 0, 0)
                };
                Capture.Children.Add(lNumber);
            }
        }
        void Background()
        {
            Random ThisRandom = new Random();
            for (int i = 0; i < 4; i++)
            {
                int Number = ThisRandom.Next(0, 9);
                Label lNumber = new Label()
                {
                    Content = Number,
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(
                        Color.FromArgb(
                            255,
                            (byte)ThisRandom.Next(0, 255),
                            (byte)ThisRandom.Next(0, 255),
                            (byte)ThisRandom.Next(0, 255))),
                    Margin = new Thickness(
                        WidthCapture/2 - 60 + i*30,
                        ThisRandom.Next(-10, 10), 0, 0)
                };
                Capture.Children.Add(lNumber);
                StrCapture += Number.ToString();
            }
        }

        public bool OnCapture()
        {
            return StrCapture == InputCapture.Text;
        }

        private void EnterCapture(object sender, KeyEventArgs e)
        {
            if (InputCapture.Text.Length == 4)
            {
                if (!OnCapture()) CreateCapture();
                else if (HandlerCorrectCapture != null)
                    HandlerCorrectCapture.Invoke();
            }
        }
    }
}
