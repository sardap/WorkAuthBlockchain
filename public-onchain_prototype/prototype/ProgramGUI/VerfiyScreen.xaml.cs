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
using WorkAuthBlockChain.src;

namespace ProgramGUI
{
    /// <summary>
    /// Interaction logic for VerfiyScreen.xaml
    /// </summary>
    public partial class VerfiyScreen : Page
    {
		private VaildDataList _vaildDataList = new VaildDataList();
		private Button _selected;

		public DataBundle DataBundle
		{
			get;
			set;
		}


		public VerfiyScreen()
        {
            InitializeComponent();

			VaildDataConetet.Content = _vaildDataList;
			_selected = RefereeButton;
		}

		public VerfiyScreen(DataBundle dataBundle) : this()
		{
			DataBundle = dataBundle;
			ChangeList(dataBundle.Referees);
		}

		private void ChangeList(List<Entry> entries)
		{
			_vaildDataList.Entries = entries;
			_vaildDataList.BindData();
		}

		// This is really fucking stupid how can i not find just a fucking swap im i fucking idiot
		private void SwitchButtonColors(Button a, Button b)
		{
			Brush tempForground = a.Foreground;
			Brush tempBackground = a.Background;

			a.Foreground = b.Foreground;
			a.Background = b.Background;

			b.Foreground = tempForground;
			b.Background = tempBackground;
		}

		private void ProcessButton(Button button, List<Entry> entries)
		{
			if (_selected != button)
			{
				ChangeList(entries);
				SwitchButtonColors(_selected, button);
				_selected = button;
			}
		}

		private void SwitchToRefeeres(object sender, RoutedEventArgs e)
		{
			ProcessButton(RefereeButton, DataBundle.Referees);
		}

		private void SwitchToWorkHistory(object sender, RoutedEventArgs e)
		{
			ProcessButton(WorkHistoryButton, DataBundle.WorkHistory);
		}
	}
}
