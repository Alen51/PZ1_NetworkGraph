using PZ1_NetworkGraph.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PZ1_NetworkGraph
{
    /// <summary>
    /// Interaction logic for AnimationWindow.xaml
    /// </summary>
    public partial class AnimationWindow : Window
    {
        ModelRenderer renderer1;

        internal AnimationWindow(ModelRenderer renderer)
        {
            InitializeComponent();
            renderer1 = renderer;

            CmbColors.ItemsSource = typeof(Colors).GetProperties();
            CmbColors.SelectedItem = GetColorAsPropertyInfo(Colors.Black);

        }

        private void ChangeAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputsValidity())
            {
                double animationDuarion = double.Parse(AnimationDurationTextBox.Text.Trim());
                double animationSize = double.Parse(AnimationSizeTextBox.Text.Trim());

                string animationColorString = CmbColors.SelectedItem.ToString().Split(' ')[1];
                Color animationColor = (Color)ColorConverter.ConvertFromString(animationColorString);

                renderer1.changeAnimation(animationSize, animationDuarion, animationColor);
            }
            else
            {
                MessageBox.Show("\"Animation duration\" and \"size\" must be numbers.\nCheck the input parameters and try again...",
                    "Invalid input!",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                return;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            renderer1.resetAnimation();
        }

        private void ExitWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal PropertyInfo GetColorAsPropertyInfo(Color color)
        {
            foreach (var variable in typeof(Colors).GetProperties())
            {
                if ((Color)(variable as PropertyInfo).GetValue(1, null) == color)
                {
                    return variable;
                }
            }

            return null;
        }

        internal bool CheckInputsValidity()
        {
            string animationDuration = AnimationDurationTextBox.Text.Trim();
            bool animationDurationValidity = Regex.IsMatch(animationDuration, "[0-9.]+");

            string animationSize = AnimationSizeTextBox.Text.Trim();
            bool animationSizeValidity = Regex.IsMatch(animationSize, "[0-9.]+");

            

            return animationDurationValidity && animationSizeValidity;
        }
    }
}
