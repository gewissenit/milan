#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using EcoFactory.Components.Events;
using Milan.Simulation;
using Milan.Simulation.CostAccounting;
using Milan.Simulation.Events;
using Milan.Simulation.Resources.Events;

namespace EcoFactory.Components
{
  public static class CostObserverExtensions
  {
    public static class AssemblyStation
    {
      public class Blocked : ProcessCostObserver<IAssemblyStation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessCostObserver<IAssemblyStation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessCostObserver<IAssemblyStation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessCostObserver<IAssemblyStation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessCostObserver<IAssemblyStation, TransformationStartEvent, TransformationEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessCostObserver<IAssemblyStation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class EntryPoint
    {
      public class Entered : ProductTypeEventCostObserver<IEntryPoint, ThroughputStartEvent>
      {
        public Entered()
          : base("Entered")
        {
        }
      }
    }

    public static class ExitPoint
    {
      public class Exited : ProductTypeEventCostObserver<IExitPoint, ThroughputEndEvent>
      {
        public Exited()
          : base("Exited")
        {
        }
      }
    }

    public static class InhomogeneousParallelWorkstation
    {
      public class Blocked : ProcessCostObserver<IInhomogeneousParallelWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessCostObserver<IInhomogeneousParallelWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessCostObserver<IInhomogeneousParallelWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessCostObserver<IInhomogeneousParallelWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing :
        ProductTypeProcessCostObserver<IInhomogeneousParallelWorkstation, ProcessingStartEvent, ParallelProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessCostObserver<IInhomogeneousParallelWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class InhomogeneousWorkstation
    {
      public class Blocked : ProcessCostObserver<IInhomogeneousWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessCostObserver<IInhomogeneousWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessCostObserver<IInhomogeneousWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessCostObserver<IInhomogeneousWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessCostObserver<IInhomogeneousWorkstation, ProcessingStartEvent, ProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessCostObserver<IInhomogeneousWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class ParallelWorkstation
    {
      public class Blocked : ProcessCostObserver<IParallelWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessCostObserver<IParallelWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessCostObserver<IParallelWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessCostObserver<IParallelWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessCostObserver<IParallelWorkstation, ProcessingStartEvent, ParallelProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessCostObserver<IParallelWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class ProbabilityAssemblyStation
    {
      public class Blocked : ProcessCostObserver<IProbabilityAssemblyStation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessCostObserver<IProbabilityAssemblyStation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessCostObserver<IProbabilityAssemblyStation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessCostObserver<IProbabilityAssemblyStation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessCostObserver<IProbabilityAssemblyStation, TransformationStartEvent, TransformationEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessCostObserver<IProbabilityAssemblyStation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class Workstation
    {
      public class AwaitingResources : ProcessCostObserver<IWorkstation, ResourceRequestedEvent, ResourceReceivedEvent>
      {
        public AwaitingResources()
          : base("Awaiting Resources")
        {
        }
      }

      public class Blocked : ProcessCostObserver<IWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessCostObserver<IWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessCostObserver<IWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessCostObserver<IWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessCostObserver<IWorkstation, ProcessingStartEvent, ProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessCostObserver<IWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }
  }
}