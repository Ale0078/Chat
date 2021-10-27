using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Chat.Client.ViewModel.Base;

namespace Chat.Client.Extensions
{
    public static class ObservableCollectionExtension
    {
        private static int TimeId { get; set; }

        public static ObservableCollection<T> GetSortedCollectionByLastMessage<T>(this ObservableCollection<T> items) 
            where T : MemberViewModelBase
        {
            SortedList<DateTimeToSort, T> sortedList = new(items.ToDictionary(
                keySelector: item => 
                {
                    try
                    {
                        return new DateTimeToSort()
                        {
                            Id = TimeId,
                            Time = item.LastMessage.SendingTime
                        };
                    }
                    finally 
                    {
                        TimeId++;
                    }
                },
                elementSelector: item => item));

            TimeId = 0;

            return new ObservableCollection<T>(sortedList.Values.Reverse());
        }

        public static void AddViewModel<T>(this ObservableCollection<T> items, T item, PropertyChangedEventHandler handler)
            where T : ViewModelBase
        {
            item.PropertyChanged -= handler;
            item.PropertyChanged += handler;

            items.Add(item);
        }

        public static void SetPropertyChangedEventHandler<T>(this ObservableCollection<T> items, PropertyChangedEventHandler handler)
            where T : ViewModelBase
        {
            foreach (T item in items)
            {
                item.PropertyChanged -= handler;
                item.PropertyChanged += handler;
            }
        }

        private struct DateTimeToSort : IComparable
        {
            public int Id { get; set; }
            public DateTime Time { get; set; }

            public int CompareTo(object other)
            {
                if (Time.CompareTo(((DateTimeToSort)other).Time) == 0)
                {
                    return Id.CompareTo(((DateTimeToSort)other).Id);
                }

                return Time.CompareTo(((DateTimeToSort)other).Time);
            }
        }
    }
}
