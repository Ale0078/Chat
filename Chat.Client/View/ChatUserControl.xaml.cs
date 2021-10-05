using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chat.Client.View
{
    public partial class ChatUserControl : UserControl
    {
        private bool _isFirstApplyTemplate;

        public ChatUserControl()
        {
            InitializeComponent();

            _isFirstApplyTemplate = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_isFirstApplyTemplate)
            {
                ChatFill.MouseUp += SetKeyboardFocus;

                _isFirstApplyTemplate = false;
            }
        }

        private void SetKeyboardFocus(object sender, MouseButtonEventArgs e) 
        {
            Keyboard.Focus(sender as IInputElement);
        }
    }
}
