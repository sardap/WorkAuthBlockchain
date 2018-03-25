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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkAuthBlockChain;
using WorkAuthBlockChain.src;

namespace ProgramGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _XMLString;


		public MainWindow()
		{
			InitializeComponent();
		}

		private async void PushDataClick(object sender, RoutedEventArgs e)
		{

			submisionProgress.Value = 0;
			string data = DataEntry.Text;
			string senderAddress = AddressEntry.Text; // 0x25922333d41f0f3f40be629f81af6983634d0fb6
			string password = passwordEntry.Text;
			string smartContractAddress;

			WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();
			RSACryptoServiceProvider rsa;

			ProgressLabel.Content = "Creating RSA";
			using (rsa = new RSACryptoServiceProvider(Consts.RSA_KEY_LENGTH))
			{
				try
				{
					workHistroySmartContract.RSA = rsa;

					rsa.FromXmlString(_XMLString);

					DataCustodianPublisher dataCustodianPublisher = new DataCustodianPublisher();
					dataCustodianPublisher.WorkHistroySmartContract = workHistroySmartContract;
					dataCustodianPublisher.RSA = rsa;

					ProgressLabel.Content = "Pushing data to blockchain";
					smartContractAddress = await dataCustodianPublisher.PublishWorkHistoryAsync(data, senderAddress, password);
					ProgressLabel.Content = "Waiting for contract to be mined";

					while (smartContractAddress == null) ;
					ProgressLabel.Content = "Smart Contract mined";

					SmartContractLabel.Text = smartContractAddress;
					submisionProgress.Value = 100;
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}
		}

		private async void LoadRSAClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				string path = openFileDialog.FileName;
				_XMLString = File.ReadAllText(path);
			}

		}

		private async void VerfiyDataClick(object sender, RoutedEventArgs e)
		{
			WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();

			DataConsumer dataConsumer = new DataConsumer();
			dataConsumer.WorkHistroySmartContract = workHistroySmartContract;

			// @Bad Should probalby not be a list of fucking bools
			List<bool> verfiyResult = await dataConsumer.Verfiy(VaildateDataInput.Text, SmartContractAddressInput.Text);

			string result = "";

			result += "The data hash is " + verfiyResult[0];
			result +=  "The data issuer is " + verfiyResult[1];

			DataVaildLabel.Content += " " + result;
		}
	}
}
