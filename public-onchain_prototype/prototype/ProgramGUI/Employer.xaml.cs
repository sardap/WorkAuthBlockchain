using Microsoft.Win32;
using Newtonsoft.Json;
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
		private string _xmlStringPath;


		public MainWindow()
		{
			InitializeComponent();
		}

		private async void PushDataClick(object sender, RoutedEventArgs e)
		{

			submisionProgress.Value = 0;
			Entry entry = new Entry
			{
				Domain = DomainEntry.Text,
				Department = DepartmentEntry.Text,
				Position = PositionEntry.Text,
				Name = NameEntry.Text,
				ExtraData = ExtraDataEntry.Text
			};

			string senderAddress = AddressEntry.Text; // 0x25922333d41f0f3f40be629f81af6983634d0fb6
			string password = passwordEntry.Text;
			string smartContractAddress;

			WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();
			RSACryptoServiceProvider rsa;

			ProgressLabel.Content = "Loading RSA";
			using (rsa = new RSACryptoServiceProvider(Consts.RSA_KEY_LENGTH))
			{
				try
				{
					workHistroySmartContract.RSA = rsa;

					rsa.FromXmlString(File.ReadAllText(_xmlStringPath));

					DataCustodianPublisher dataCustodianPublisher = new DataCustodianPublisher();
					dataCustodianPublisher.WorkHistroySmartContract = workHistroySmartContract;
					dataCustodianPublisher.RSA = rsa;

					ProgressLabel.Content = "Pushing data to blockchain";
					smartContractAddress = await dataCustodianPublisher.PublishWorkHistoryAsync(entry.OnChainString(), senderAddress, password);
					ProgressLabel.Content = "Waiting for contract to be mined";

					while (smartContractAddress == null) ;
					ProgressLabel.Content = "Smart Contract mined";
					entry.Address = smartContractAddress;

					SaveFileDialog saveFileDialog = new SaveFileDialog();
					if (saveFileDialog.ShowDialog() == true)
					{
						using (StreamWriter file = File.CreateText(saveFileDialog.FileName))
						{
							JsonSerializer serializer = new JsonSerializer();
							//serialize object directly into file stream
							serializer.Serialize(file, entry);
						}
					}

					submisionProgress.Value = 100;
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
				_xmlStringPath = openFileDialog.FileName;
			}
		}

		private async Task<string> VerfiyEntry(DataConsumer dataConsumer, Entry entry)
		{
			List<bool> verfiyResult = await dataConsumer.Verfiy(entry.OnChainString(), entry.Address);

			string result = "\n";

			result += "Data is " + (verfiyResult[0] ? "Vaild" : "Invaild") + " Sender is " + (verfiyResult[1] ? "Vaild" : "Invaild") + "\n";
			result += "Address:\t" + entry.Address + "\n";
			result += "Domain:\t\t" + entry.Domain + "\n";
			result += "Department:\t" + entry.Department + "\n";
			result += "Position:\t\t" + entry.Position + "\n";
			result += "Name:\t\t" + entry.Name + "\n";
			result += "Extra Data:\t" + entry.ExtraData + "\n";

			return result;
		}

		private async void VerfiyDataClick(object sender, RoutedEventArgs e)
		{
			DataVaildLabel.Content = "";

			try
			{
				WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();

				DataConsumer dataConsumer = new DataConsumer();
				dataConsumer.WorkHistroySmartContract = workHistroySmartContract;

				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (openFileDialog.ShowDialog() == true)
				{
					using (StreamReader r = new StreamReader(openFileDialog.FileName))
					{
						string json = r.ReadToEnd();
						DataBundle dataBundle = JsonConvert.DeserializeObject<DataBundle>(json);

						DataVaildLabel.Content = "Referees:\n";

						foreach (Entry entry in dataBundle.Referees)
						{
							DataVaildLabel.Content +=  await VerfiyEntry(dataConsumer, entry);
						}

						DataVaildLabel.Content += "\nWork History:\n";

						foreach (Entry entry in dataBundle.WorkHistory)
						{
							DataVaildLabel.Content += await VerfiyEntry(dataConsumer, entry);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ExportDataClick(object sender, RoutedEventArgs e)
		{

		}
	}
}
