﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

		private async Task<DataBundle> ReadDataBundle()
		{
			using (StreamReader r = new StreamReader(_workHistoryBundlePath))
			{
				string json = await r.ReadToEndAsync();
				return await Task.Run(() => JsonConvert.DeserializeObject<DataBundle>(json));
			}
		}

		private async void VerfiyClick(object sender, RoutedEventArgs e)
		{
			if(_workHistoryBundlePath != null)
			{
				DataBundle dataBundle = await ReadDataBundle();

				VerfiyScreen verfiyScreen = new VerfiyScreen(dataBundle);

				Main main = (Main)Window.GetWindow(this);
				main.Content = verfiyScreen;
			}
		}
	}
}
