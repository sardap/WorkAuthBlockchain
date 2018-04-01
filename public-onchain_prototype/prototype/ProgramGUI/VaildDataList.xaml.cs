using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Controls;
using WorkAuthBlockChain;
using WorkAuthBlockChain.src;
using System.Threading.Tasks;

namespace ProgramGUI
{
    /// <summary>
    /// Interaction logic for ValidDataList.xaml
    /// </summary>
    public partial class ValidDataList : Page
    {
		private bool _dataValid;
		private bool _sendersValid;

		public List<Entry> Entries
		{
			get;
			set;
		}

		public bool NamesMatch()
		{
			return Entries.All(i => i.Name == Entries.First().Name);
		}

		public bool DataValid()
		{
			return _dataValid;
		}

		public bool SendersValid()
		{
			return _sendersValid;
		}

		public ValidDataList()
        {
            InitializeComponent();
		}

		public async Task BindData()
		{
			try
			{
				_dataValid = true;
				_sendersValid = true;

				listView.Items.Clear();

				WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();

				DataConsumer dataConsumer = new DataConsumer
				{
					WorkHistroySmartContract = workHistroySmartContract
				};

				foreach (Entry entry in Entries)
				{
					VerfiyResult verfiyResult = await dataConsumer.Verfiy(entry.OnChainString(), entry.Address);

					VerfiyEntry verfiyEntry = new VerfiyEntry(entry)
					{
						SenderValid = verfiyResult.Sender,
						DataValid = verfiyResult.Data
					};

					if (!verfiyEntry.DataValid)
					{
						_dataValid = false;
					}

					if (!verfiyEntry.SenderValid)
					{
						_sendersValid = false;
					}

					listView.Items.Add(verfiyEntry);
				}

			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/*
		private async Task<bool> VerfiyEntry(DataConsumer dataConsumer, Entry entry)
		{
			List<bool> verfiyResult = await dataConsumer.Verfiy(entry.OnChainString(), entry.Address);

			string result = "\n";

			result += "Data is " + (verfiyResult[0] ? "Valid" : "InValid") + " Sender is " + (verfiyResult[1] ? "Valid" : "InValid") + "\n";
			result += "Address:\t" + entry.Address + "\n";
			result += "Domain:\t\t" + entry.Domain + "\n";
			result += "Department:\t" + entry.Department + "\n";
			result += "Position:\t\t" + entry.Position + "\n";
			result += "Name:\t\t" + entry.Name + "\n";
			result += "Extra Data:\t" + entry.ExtraData + "\n";

			return result;
		}
		*/
	}
}
