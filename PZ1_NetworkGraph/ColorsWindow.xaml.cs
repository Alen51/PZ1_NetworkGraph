using PZ1_NetworkGraph.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// Interaction logic for ColorsWindow.xaml
    /// </summary>
    public partial class ColorsWindow : Window
    {

		ModelRenderer renderer1;

        internal ColorsWindow(ModelRenderer renderer)
        {
            InitializeComponent();

			//this.shapeTextManager = shapeTextManager;
			//this.shapeTextUnit = shapeTextUnit;
			renderer1 = renderer;
            InitCmbColors();
        }

		internal void InitCmbColors()
		{
			List<string> e = new List<string>();
			CmbColors.ItemsSource = typeof(Colors).GetProperties();
			e.Add("NodeEntity");
			e.Add("SubstationEntity");
			e.Add("SwitchEntity");
			CmbEntity.ItemsSource = e;

			CmbColors.SelectedItem = GetColorAsPropertyInfo(Colors.Black);
			CmbEntity.SelectedIndex = 0; ;
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

        private void ChangeColorsButton_Click(object sender, RoutedEventArgs e)
        {
			string fillColorString = CmbColors.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fillColorString));

			string entity = CmbEntity.SelectedItem.ToString();
			renderer1.changeEntityGroupColors(entity, fillColor);
			
		}

        private void ExitWindowButton_Click(object sender, RoutedEventArgs e)
        {
			Close();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
			renderer1.RevertEntityColor();
        }
    }
}
