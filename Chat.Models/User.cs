using System.Collections.ObjectModel;

namespace Chat.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLogin { get; set; }

        public User()
        {
            Messages = new ObservableCollection<ChatMessage>();
        }
    }
}
