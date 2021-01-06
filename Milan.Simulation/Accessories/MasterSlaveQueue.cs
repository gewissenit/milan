#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Accessories
{
  /// <summary>
  ///   This class is to be supposed to aid in accomplishing cooperation between master- and slave-entities. Typical
  ///   scenario: master=workstation, slave=product. Dequeuing is only possible if at least one master and one slave are
  ///   available. If AutomaticCooperation is set to true, this queue automatically initiates cooperation as soon as
  ///   dequeuing is possible.
  /// </summary>
  /// <typeparam name="TMaster">The master, processes slaves. Must implement <see cref="ISlave" /></typeparam>
  /// <typeparam name="TSlave">
  ///   The slave, waits to be processed by a master. Must implement <see cref="IMaster" />
  /// </typeparam>
  public class MasterSlaveQueue<TMaster, TSlave>
    where TMaster : class, IMaster
    where TSlave : class, ISlave
  {
    private readonly Queue<TMaster> _masterQueue;
    private readonly Queue<TSlave> _slaveQueue;

    /// <summary>
    /// </summary>
    /// <param name="automaticCooperation">Automatically initiates cooperation if set to true. Otherwise stays inactive.</param>
    public MasterSlaveQueue(bool automaticCooperation)
    {
      AutomaticCooperation = automaticCooperation;
      _masterQueue = new Queue<TMaster>();
      _slaveQueue = new Queue<TSlave>();
    }

    /// <summary>
    ///   Determines if automatic initiation of cooperation is turned on.
    /// </summary>
    public bool AutomaticCooperation { get; set; }

    /// <summary>
    ///   A queue containing master-entities waiting for slave-entities.
    /// </summary>
    protected Queue<TMaster> MasterQueue
    {
      get { return _masterQueue; }
    }

    /// <summary>
    ///   A queue containing slave-entities waiting for master-entities.
    /// </summary>
    protected Queue<TSlave> SlaveQueue
    {
      get { return _slaveQueue; }
    }

    /// <summary>
    ///   Determines if master- and slave-entity are available for cooperation.
    /// </summary>
    public bool IsCooperationPossible
    {
      get { return !MasterQueue.IsEmpty && !SlaveQueue.IsEmpty; }
    }

    /// <summary>
    ///   Amount of master-entities waiting for slave-entities.
    /// </summary>
    public int MasterCount
    {
      get { return MasterQueue.Count; }
    }

    /// <summary>
    ///   Amount of slave entities waiting to be processed by a master-entity.
    /// </summary>
    public int SlaveCount
    {
      get { return SlaveQueue.Count; }
    }

    /// <summary>
    ///   Enqueues a master-entity.
    /// </summary>
    /// <param name="master">Master-entity requesting cooperation with slave-entity.</param>
    /// <exception cref="ArgumentNullException">If master is null.</exception>
    public void EnqueueMaster(TMaster master)
    {
      if (master == null)
      {
        throw new ArgumentNullException("master");
      }
      MasterQueue.Enqueue(master);
      if (AutomaticCooperation && IsCooperationPossible)
      {
        Dequeue();
      }
    }

    /// <summary>
    ///   Enqueues a slave-entity.
    /// </summary>
    /// <param name="slave">Slave-entity requesting cooperation with master-entity.</param>
    /// <exception cref="ArgumentNullException">If slave is null.</exception>
    public void EnqueueSlave(TSlave slave)
    {
      if (slave == null)
      {
        throw new ArgumentNullException("slave");
      }
      SlaveQueue.Enqueue(slave);
      if (AutomaticCooperation && IsCooperationPossible)
      {
        Dequeue();
      }
    }

    /// <summary>
    ///   Initiates cooperation between master- and slave-entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">If either no master or no slave are available.</exception>
    public void Dequeue()
    {
      if (!IsCooperationPossible)
      {
        throw new InvalidOperationException("No cooperation possible!");
      }
      var master = MasterQueue.Dequeue();
      var slave = SlaveQueue.Dequeue();
      master.Cooperate(slave);
    }

    /// <summary>
    ///   Checks if given master-entity is waiting for cooperation.
    /// </summary>
    /// <param name="master">The master-entity.</param>
    /// <returns>True, if master is waiting. Otherwise false.</returns>
    public bool ContainsMaster(TMaster master)
    {
      return MasterQueue.Contains(master);
    }

    /// <summary>
    ///   Checks if given slave-entity is waiting for cooperation.
    /// </summary>
    /// <param name="slave">The slave-entity.</param>
    /// <returns>True, if slave is waiting. Otherwise false.</returns>
    public bool ContainsSlave(TSlave slave)
    {
      return SlaveQueue.Contains(slave);
    }
  }
}