using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WorkAuthBlockChain;

namespace ProgramGUI
{
    /// <summary>
    /// Interaction logic for VerfiyDataMain.xaml
    /// </summary>
    public partial class VerfiyDataMain : Page
    {

		private string _workHistoryBundlePath;

        public VerfiyDataMain()
        {
            InitializeComponent();
        }

		private void BrowseClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				_workHistoryBundlePath = openFileDialog.FileName;
				ProgressEntry.Text = _workHistoryBundlePath;
			}
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Main main = (Main)Window.GetWindow(this);
			main.Content = new Menu();
		}

		private async void VerfiyClick(object sender, RoutedEventArgs e)
		{
			if(_workHistoryBundlePath != null)
			{
				DataBundle dataBundle = await Utils.ReadFromJson<DataBundle>(_workHistoryBundlePath);

				VerfiyScreen verfiyScreen = new VerfiyScreen(dataBundle);

				Main main = (Main)Window.GetWindow(this);
				main.Content = verfiyScreen;
			}
		}
	}
}
