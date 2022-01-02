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

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double pi1, pi2;
            if (!double.TryParse(pi1Box.Text, out pi1) || !double.TryParse(pi2Box.Text, out pi2))
                return;

            ModelSimulateAsync(pi1, pi2);
        }

        private async Task ModelSimulateAsync(double pi1, double pi2)
        {
            ImitationModel model = new ImitationModel(pi1, pi2);
            await Task.Run(model.Simulate);
            var resultWindow = new ResultWindow();
            resultWindow.Probabilities.Text = "P2000: " + model["2000"] + "\n"
                + "P1000: " + model["1000"] + "\n"
                + "P2010: " + model["2010"] + "\n"
                + "P1010: " + model["1010"] + "\n"
                + "P2110: " + model["2110"] + "\n"
                + "P1110: " + model["1110"] + "\n"
                + "P2210: " + model["2210"] + "\n"
                + "P1210: " + model["1210"] + "\n"
                + "P1001: " + model["1001"] + "\n"
                + "P2011: " + model["2011"] + "\n"
                + "P1011: " + model["1011"] + "\n"
                + "P2111: " + model["2111"] + "\n"
                + "P1111: " + model["1111"] + "\n"
                + "P2211: " + model["2211"] + "\n"
                + "P1211: " + model["1211"];
            resultWindow.Coefficients.Text = "Pотк: " + model.DenialProbability + "\n"
                + "Wс: " + model.AvgTimeInSystem + "\n"
                + "Lоч: " + model.AvgQueueLength;
            resultWindow.Show();
        }
    }
}
