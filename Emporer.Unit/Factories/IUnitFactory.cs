namespace Emporer.Unit.Factories
{
  public interface IUnitFactory
  {
    IUnit Create();
    IUnit Duplicate(IUnit master);
  }
}