#region License

// Copyright (c) 2013 HTW Berlin All rights reserved.

#endregion License

using GeWISSEN.Utils;
using Milan.Simulation.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public abstract class StationaryElement : Entity, IStationaryElement
  {
    protected Transmitter<Product> _productSender;

    public IEnumerable<Product> ResidingProducts
    {
      get { return GetResidingProducts(); }
    }

    public event EventHandler<EventArgs> GotAvailable;

    public virtual bool IsAvailable(Product product)
    {
      return product != null;
    }

    public virtual void Receive(Product product)
    {
      _lastReceivedProduct = product;
      if (!IsAvailable(product))
      {
        throw new InvalidOperationException("The station is not available. Wrong usage of API. Ask IsAvailable first!");
      }
      var productReceived = new ProductReceiveEvent(this,
                                                    new[]
                                                    {
                                                      product
                                                    });
      productReceived.Schedule(0);
      product.CurrentLocation = this;
    }

    public override void Initialize()
    {
      base.Initialize();
      foreach (var connection in Connections)
      {
        connection.Destination.GotAvailable += OnGotAvailable;
      }
      _productSender = new ProductTransmitter(Connections);
    }

    public override void Reset()
    {
      _productSender = null;
      GotAvailable = null;
      _lastReceivedProduct = null;
      base.Reset();
    }

    [JsonProperty]
    private readonly List<IConnection> _Connections = new List<IConnection>();

    protected Product _lastReceivedProduct;

    public virtual IEnumerable<IConnection> Connections
    {
      get { return _Connections; }
    }

    public virtual void Add(IConnection connection)
    {
      if (connection.Destination == this)
      {
        return; //HACK: we don't ask for CanConnect!
      }

      if (_Connections.Contains(connection))
      {
        throw new InvalidOperationException(string.Format("The entity already contains the given successor: {0}", connection));
      }
      Debug.Assert(CanConnectToDestination(connection.Destination));
      _Connections.Add(connection);
      Added.Raise(this, connection);
    }

    public virtual void Remove(IConnection connection)
    {
      if (!_Connections.Contains(connection))
      {
        throw new InvalidOperationException(string.Format("The entity does not contain the given successor: {0}", connection));
      }
      _Connections.Remove(connection);
      Removed.Raise(this,connection);
    }

    public event Action<INode<IConnection>,IConnection> Added;

    public event Action<INode<IConnection>,IConnection> Removed;

    public virtual bool CanConnectToSource(INode<IConnection> node)
    {
      return !node.Connections.Select(n => n.Destination)
                  .Contains(this) && node != this;
    }

    public virtual bool CanConnectToDestination(INode<IConnection> node)
    {
      return !Connections.Select(n => n.Destination)
                         .Contains(node) && node != this;
    }

    protected abstract IEnumerable<Product> GetResidingProducts();

    protected void RaiseGotAvailable()
    {
      GotAvailable?.Invoke(this, new EventArgs());
    }

    protected virtual void Send(Product product)
    {
      var productTransmitted = new ProductTransmitEvent(this,
                                                        new[]
                                                        {
                                                          product
                                                        });
      productTransmitted.Schedule(0);
      _productSender.Transmit(product);
    }

    protected virtual void OnGotAvailable(object sender, EventArgs e)
    {
    }
  }
}