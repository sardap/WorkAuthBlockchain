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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkAuthBlockChain;
using WorkAuthBlockChain.src;

namespace ProgramGUI
{
    /// <summary>
    /// Interaction logic for CreateData.xaml
    /// </summary>
    public partial class CreateData : Page
    {
		private string _xmlStringPath;
		private Effect _blurEffect; 

        public CreateData()
        {
            InitializeComponent();
			_blurEffect = DataEntryPannel.Effect;
			DataEntryPannel.Effect = null;

		}

		private Entry CreateAndPopluateEntry()
		{
			return new Entry
			{
				Domain = DomainEntry.Text,
				Date = DateEntry.Text,
				Department = DeparmentEntry.Text,
				Position = PostionEntry.Text,
				Name = NameEntry.Text,
				ExtraData = ExtraInfoEntry.Text
			};
		}

		private async void CreateClick(object sender, RoutedEventArgs e)
		{
			DataEntryPannel.Effect = _blurEffect;
			ProgressBox.Visibility = Visibility.Visible;

			ProgressEntry.Text = "Create Entry...";

			Entry entry = CreateAndPopluateEntry();

			ProgressEntry.Text = "Creating RSA...";

			string senderAddress = EthereumEntry.Text; // 0x25922333d41f0f3f40be629f81af6983634d0fb6
			string password = GethEntry.Password;
			string smartContractAddress;

			WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();
			RSACryptoServiceProvider rsa;

			using (rsa = new RSACryptoServiceProvider(Consts.RSA_KEY_LENGTH))
			{
				try
				{
					workHistroySmartContract.RSA = rsa;

					rsa.FromXmlString(File.ReadAllText(_xmlStringPath));

					DataProdcuer dataPrdocuer = new DataProdcuer
					{
						WorkHistroySmartContract = workHistroySmartContract,
						RSA = rsa
					};

					ProgressEntry.Text = "Waiting for address...";
					smartContractAddress = await dataPrdocuer.PublishWorkHistoryAsync(entry.OnChainString(), senderAddress, password);

					entry.Address = smartContractAddress;

					ProgressEntry.Text = "Saving file...";
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					if (saveFileDialog.ShowDialog() == true)
					{
						using (StreamWriter file = File.CreateText(saveFileDialog.FileName))
						{
							JsonSerializer serializer = new JsonSerializer();
							//serialize object directly into file stream
							serializer.Serialize(file, entry);

							DomainEntry.Text = "";
							DeparmentEntry.Text = "";
							PostionEntry.Text = "";
							NameEntry.Text = "";
							ExtraInfoEntry.Text = "";
							EthereumEntry.Text = "";
							GethEntry.Password = "";
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}

				ProgressBox.Visibility = Visibility.Hidden;
				DataEntryPannel.Effect = null;
			}
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Main main = (Main)Window.GetWindow(this);
			main.Content = new Menu();
		}

		private void UploadClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				_xmlStringPath = openFileDialog.FileName;
			}
		}
	}
}
