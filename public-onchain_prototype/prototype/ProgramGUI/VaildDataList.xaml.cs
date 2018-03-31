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
using WorkAuthBlockChain;
using WorkAuthBlockChain.src;

namespace ProgramGUI
{
    /// <summary>
    /// Interaction logic for VaildDataList.xaml
    /// </summary>
    public partial class VaildDataList : Page
    {
		public List<Entry> Entries
		{
			get;
			set;
		}

        public VaildDataList()
        {
            InitializeComponent();
		}

		public async void BindData()
		{
			try
			{
				DataLabel.Content = "";

				WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();

				DataConsumer dataConsumer = new DataConsumer
				{
					WorkHistroySmartContract = workHistroySmartContract
				};

				foreach (Entry entry in Entries)
				{
					DataLabel.Content += await VerfiyEntry(dataConsumer, entry);
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

	}
}
