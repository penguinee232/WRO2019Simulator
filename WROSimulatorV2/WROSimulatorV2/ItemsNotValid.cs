using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class ItemsNotValid<T>
    {
        public HashSet<T> InvalidItems { get; private set; }
        public ItemsNotValid(HashSet<T> invalidItems)
        {
            InvalidItems = invalidItems;
        }
        public bool IsValid(T item)
        {
           return !InvalidItems.Contains(item);
        }
    }
}
