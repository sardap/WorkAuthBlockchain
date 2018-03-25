using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using WorkAuthBlockChain;
using WorkAuthBlockChain.src;

namespace ProgramGUI
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class DataSubjectWindow : Window
	{
		private string _xmlStringPath;

		public DataSubjectWindow()
		{
			InitializeComponent();
		}

		private async void LoadRSAClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				_xmlStringPath = openFileDialog.FileName;
			}
		}

		private async void LoadDataClick(object sender, RoutedEventArgs e)
		{
			try
			{
				RSACryptoServiceProvider rsa;
				WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();

				using (rsa = new RSACryptoServiceProvider(Consts.RSA_KEY_LENGTH))
				{
					try
					{
						RSACryptoServiceProviderExtensions.FromXmlString(rsa, File.ReadAllText(_xmlStringPath));

						DataSubjectSharer dataSubjectSharer = new DataSubjectSharer();
						dataSubjectSharer.WorkHistroySmartContract = workHistroySmartContract;
						dataSubjectSharer.RSA = rsa;

						workHistroySmartContract.LoadContract(AddressEntry.Text);

						string decryptedData = await dataSubjectSharer.DecryptDataFromContract();

						DataOutput.Text = decryptedData;
					}
					finally
					{
						rsa.PersistKeyInCsp = false;
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
