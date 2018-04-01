using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using WorkAuthBlockChain.src;

namespace ProgramGUI
{
    public class VerfiyEntry : ViewBase
	{
		public bool SenderValid { get; set; }
		public bool DataValid { get; set; }
		public string SenderValidString
		{
			get
			{
				return SenderValid ? "yes" : "no";
			}
		}
		public string DataValidString
		{
			get
			{
				return SenderValid ? "yes" : "no";
			}
		}

		public string Address { get; set; }
		public string Domain { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }
		public string Name { get; set; }
		public string ExtraData { get; set; }


		public VerfiyEntry(Entry entry) : base()
		{
			Address = entry.Address;
			Domain = entry.Domain;
			Department = entry.Department;
			Position = entry.Position;
			Name = entry.Name;
			ExtraData = entry.ExtraData;
		}
    }
}
