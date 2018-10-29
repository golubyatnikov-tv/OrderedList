[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![NuGet version](https://badge.fury.io/nu/gt.Collections.OrderedList.svg)](https://www.nuget.org/packages/gt.Collections.OrderedList)
# gt.Collections.Ordered

## Installation

Available on [NuGet](https://www.nuget.org/packages/gt.Collections.OrderedList/)

Visual Studio:

```powershell
PM> Install-Package gt.Collections.OrderedList
```

.NET Core CLI:

```bash
dotnet add package gt.Collections.OrderedList
```

## Usage

### usage with primitives
```csharp
var list = OrderedCollection<int>.Create(n => n); // or new OrderedCollection<int, int>(n=>n)
list.Add(2);
list.Add(-1);
list.Add(3);
list.Add(0);

Console.WriteLine(string.Join(", ", list)); // -1, 0, 2, 3
```

### usage with complex objects that throw notifications
```csharp

var list = OrderedCollection<TestNotifyObj>.Create(n => n.Order); // collection uses bisect-right logic by default
var o1 = new TestNotifyObj(2);
var o2 = new TestNotifyObj(3);
var objectToChange = new TestNotifyObj(1);
list.Add(o1);
list.Add(o2);            
list.Add(objectToChange);

Console.WriteLine(string.Join(", ", list)); // Id: 3, Order: 1; Id: 1, Order: 2; Id: 2, Order: 3

objectToChange.Order = 2;
Console.WriteLine(string.Join(", ", list)); // Id: 1, Order: 2, Id: 3, Order: 2, Id: 2, Order: 3

...

public class TestNotifyObj : INotifyPropertyChanged
{
    private int _order;
    private static int _lastId;        

    public TestNotifyObj()
    {
        Id = ++_lastId;
    }

    public TestNotifyObj(int order) : this()
    {
        Order = order;
    }

    public int Id { get; }

    public int Order 
    {
        get => _order;
        set
        {
            if (value == _order) return;
            _order = value;
            OnPropertyChanged();
        }
    }

    public override string ToString()
    {
        return $"Id: {Id}, Order: {Order}";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```
You also can use OrderedList instead of OrderedCollection.

## Mode
You can change mode by passing sortMethod parameter to the constructor or static method `Create` of collection.

### BisectRight mode (default value of sortMethod parameter)
If entries with same order are already present in collection, the insertion point will be after (to the right of) any existing entries.
```csharp
new OrderedCollection<TestNotifyObj, int>(n=>n.Order, sortMethod: SortMethod.BisectRight);
```

### BisectLeft mode
If entries with same order are already present in collection, the insertion point will be before (to the left of) any existing entries.
```csharp
new OrderedCollection<TestNotifyObj, int>(n=>n.Order, sortMethod: SortMethod.BisectLeft);
```

## Helpers
BinarySearchHelper - Binary search algorithm implementation logic for generic collection.

SortingHelper - Binary search algorithm with right and left insertion modes logic.

## Contributing

Contributions are highly welcome. If you have found a bug or if you have a feature request, please report them at this repository issues section.

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.
