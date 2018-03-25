﻿using Microsoft.Win32;
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
			string data = DataEntry.Text;
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
					smartContractAddress = await dataCustodianPublisher.PublishWorkHistoryAsync(data, senderAddress, password);
					ProgressLabel.Content = "Waiting for contract to be mined";

					while (smartContractAddress == null) ;
					ProgressLabel.Content = "Smart Contract mined";

					SmartContractLabel.Text = smartContractAddress;
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

		private async void VerfiyDataClick(object sender, RoutedEventArgs e)
		{
			DataVaildLabel.Content = "";

			try
			{
				WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();

				DataConsumer dataConsumer = new DataConsumer();
				dataConsumer.WorkHistroySmartContract = workHistroySmartContract;

				// @Bad Should probalby not be a list of fucking bools
				List<bool> verfiyResult = await dataConsumer.Verfiy(VaildateDataInput.Text, SmartContractAddressInput.Text);

				string result = "";

				result += "The data hash is " + verfiyResult[0];
				result += "The data issuer is " + verfiyResult[1];

				DataVaildLabel.Content += " " + result;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}