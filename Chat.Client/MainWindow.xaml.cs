using System.Windows;

using Chat.Client.ViewModel;

namespace Chat.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
