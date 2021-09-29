using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Chat.Client.ViewModel;

namespace Chat.Client.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static ObservableCollection<T> GetSortedCollectionByLastMessage<T>(this ObservableCollection<T> items) 
            where T : ChatMemberViewModel
        {
            SortedList<DateTime, T> sortedList = new(items.ToDictionary(
                keySelector: item => item.LastMessage.SendingTime,
                elementSelector: item => item));

            return new ObservableCollection<T>(sortedList.Values.Reverse());
        }
    }
}
