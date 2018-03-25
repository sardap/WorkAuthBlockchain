using System.Windows;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		// Initialize application and set-up a none-null dataContext here
		// If the data context is null nothing will be displayed
		DataContext = ..
	}

	protected override void OnClosed(EventArgs e)
	{
		this.HtmlView.Dispose();
	}
}
