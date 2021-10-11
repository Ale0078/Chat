using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chat.Client.View
{
    public partial class EditMessageUserControl : UserControl
    {
        public static readonly DependencyProperty CancelCommandProperty;
        public static readonly DependencyProperty CancelCommandParametrProperty;

        static EditMessageUserControl() 
        {
            CancelCommandProperty = DependencyProperty.Register(
                name: nameof(CancelCommand),
                propertyType: typeof(ICommand),
                ownerType: typeof(EditMessageUserControl));

            CancelCommandParametrProperty = DependencyProperty.Register(
                name: nameof(CancelCommandParametr),
                propertyType: typeof(object),
                ownerType: typeof(EditMessageUserControl));
        }

        public EditMessageUserControl()
        {
            InitializeComponent();
        }

        public ICommand CancelCommand 
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        public object CancelCommandParametr 
        {
            get => GetValue(CancelCommandParametrProperty);
            set => SetValue(CancelCommandParametrProperty, value);
        }
    }
}
