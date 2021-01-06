using System.ComponentModel.Composition;

namespace Milan.Simulation.Factories
{
  [Export(typeof(IConnectionFactory))]
  internal class ConnectionFactory: IConnectionFactory
  {
    public IConnection Create()
    {
      return new Connection();
    }

    public IConnection Duplicate(IConnection connection)
    {
      var duplicate = new Connection
                      {
                        Priority = connection.Priority,
                        Destination = connection.Destination,
                        IsRoutingPerProductType = connection.IsRoutingPerProductType,
                      };
      foreach (var productType in connection.ProductTypes)
      {
        duplicate.Add(productType);
      }
      return duplicate;
    }
  }
}