using System.Windows;
using System.Windows.Controls;

using Chat.Client.ViewModel;

namespace Chat.Client.TemplateSelectors
{
    public class ListOfUsersSelector : DataTemplateSelector
    {
        public DataTemplate DefaultUserTemplate { get; set; }
        public DataTemplate BlockedUserTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) => (ChatMemberViewModel)item switch
        {
            { IsBlocked: true } => DefaultUserTemplate,
            _ => DefaultUserTemplate
        };
    }
}
