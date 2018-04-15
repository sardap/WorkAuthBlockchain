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

namespace ProgramGUI
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

		private void CreateDataClick(object sender, RoutedEventArgs e)
		{
			Main main = (Main)Window.GetWindow(this);
			main.Content = new CreateData();
		}

		private void VerfiyDataClick(object sender, RoutedEventArgs e)
		{
			Main main = (Main)Window.GetWindow(this);
			main.Content = new VerfiyDataMain();
		}
	}
}
