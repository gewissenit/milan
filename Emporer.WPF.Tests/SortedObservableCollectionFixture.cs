using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Emporer.WPF.Tests
{
  [TestFixture]
  public class SortedObservableCollectionFixture
  {
    private static readonly IComparer<FakeItem> _StringComparer = new FakeItemComparer();

    private class FakeItemComparer : IComparer<FakeItem>
    {
      public int Compare(FakeItem x, FakeItem y)
      {
        return string.Compare(x.Name, y.Name, false, CultureInfo.InvariantCulture);
      }
    }

    private sealed class FakeItem : INotifyPropertyChanged
    {
      private string _Name;

      public FakeItem(string name)
      {
        _Name = name;
      }

      public string Name
      {
        get { return _Name; }
        set
        {
          if (_Name == value)
          {
            return;
          }
          _Name = value;
          RaisePropertyChanged();
        }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
      {
        var handler = PropertyChanged;
        if (handler != null)
        {
          handler(this, new PropertyChangedEventArgs(propertyName));
        }
      }

      public override string ToString()
      {
        return Name;
      }
    }

    private IEnumerable<FakeItem> GenerateItems(IEnumerable<string> strings)
    {
      return strings.Select(x => new FakeItem(x));
    }

    [Test]
    public void Adding_A_Duplicate_Value_Inserts_It_After_The_First_One()
    {
      var initialItems = GenerateItems(new[]
                                       {
                                         "c", "b", "a"
                                       });

      var sut = new SortedObservableCollection<FakeItem>(initialItems, _StringComparer, "Name");
      var duplicateValue = new FakeItem("b");
      sut.Add(duplicateValue);

      Assert.AreEqual(duplicateValue, sut[2]);
    }

    [Test]
    public void Initial_Ctor_Values_Are_Sorted()
    {
      var initialItems = GenerateItems(new[]
                                       {
                                         "c", "b", "a"
                                       })
        .ToArray();

      var sut = new SortedObservableCollection<FakeItem>(initialItems, _StringComparer, "Name");

      Assert.AreEqual(initialItems[2], sut[0]);
      Assert.AreEqual(initialItems[1], sut[1]);
      Assert.AreEqual(initialItems[0], sut[2]);
    }

    [Test]
    public void On_PropertyChange_Reorder_Ite_After_The_Last_On_PropertyChange_If_Enabled()
    {
      var initialItems = GenerateItems(new[]
                                       {
                                         "a", "b", "c", "d", "e", "f"
                                       })
        .ToArray();

      var sut = new SortedObservableCollection<FakeItem>(initialItems, _StringComparer, "Name");

      initialItems[2].Name = "g";

      Assert.AreEqual(new[]
                      {
                        "a", "b", "d", "e", "f", "g"
                      },
                      sut.Select(x => x.Name));
    }

    [Test]
    public void On_PropertyChange_Reorder_Ite_Before_The_First_On_PropertyChange_If_Enabled()
    {
      var initialItems = GenerateItems(new[]
                                       {
                                         "a", "b", "c", "d", "e", "f"
                                       })
        .ToArray();

      var sut = new SortedObservableCollection<FakeItem>(initialItems, _StringComparer, "Name");

      initialItems[2].Name = "0";

      Assert.AreEqual(new[]
                      {
                        "0", "a", "b", "d", "e", "f"
                      },
                      sut.Select(x => x.Name));
    }


    [Test]
    public void On_PropertyChange_Reorder_Ite_Down_The_List_On_PropertyChange_If_Enabled()
    {
      var initialItems = GenerateItems(new[]
                                       {
                                         "a", "b", "c", "d", "e", "f"
                                       })
        .ToArray();

      var sut = new SortedObservableCollection<FakeItem>(initialItems, _StringComparer, "Name");

      initialItems[1].Name = "d1";

      Assert.AreEqual(new[]
                      {
                        "a", "c", "d", "d1", "e", "f"
                      },
                      sut.Select(x => x.Name));
    }

    [Test]
    public void On_PropertyChange_Reorder_Ite_Up_The_List_On_PropertyChange_If_Enabled()
    {
      var initialItems = GenerateItems(new[]
                                       {
                                         "a", "b", "c", "d", "e", "f"
                                       })
        .ToArray();

      var sut = new SortedObservableCollection<FakeItem>(initialItems, _StringComparer, "Name");

      initialItems[2].Name = "a1";

      Assert.AreEqual(new[]
                      {
                        "a", "a1", "b", "d", "e", "f"
                      },
                      sut.Select(x => x.Name));
    }

    [Test]
    public void On_PropertyChange_Do_Not_Reorder_If_Changed_Ite_Is_The_Only_One_In_The_List()
    {
      var initialItems = GenerateItems(new[]
                                       {
                                         "a"
                                       })
        .ToArray();

      var sut = new SortedObservableCollection<FakeItem>(initialItems, _StringComparer, "Name");

      initialItems[0].Name = "a1";

      Assert.AreEqual(new[]
                      {
                        "a1"
                      },
                      sut.Select(x => x.Name));
    }
  }
}