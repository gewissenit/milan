using System.ComponentModel.Composition;
using Milan.JsonStore;

namespace Emporer.Unit.Factories
{
  [Export(typeof(IUnitFactory))]
  internal class UnitFactory: IUnitFactory
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public UnitFactory([Import] IJsonStore store)
    {
      _store = store;
    }

    public IUnit Create()
    {
      var newInstance = new Unit();
      _store.Add(newInstance);
      return newInstance;
    }

    public IUnit Duplicate(IUnit master)
    {
      var clone = new Unit
                  {
                    ReferencedUnit = master.ReferencedUnit,
                    Dimension = master.Dimension,
                    Coefficient = master.Coefficient,
                    Name = master.Name,
                    Symbol = master.Symbol,
                  };
      return clone;
    }
  }
}