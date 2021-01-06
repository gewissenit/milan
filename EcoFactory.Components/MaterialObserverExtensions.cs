#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using EcoFactory.Components.Events;
using Milan.Simulation.Events;
using Milan.Simulation.MaterialAccounting;
using Milan.Simulation.Resources.Events;

namespace EcoFactory.Components
{
  public static class MaterialObserverExtensions
  {
    public static class AssemblyStation
    {
      public class Blocked : ProcessMaterialObserver<IAssemblyStation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessMaterialObserver<IAssemblyStation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessMaterialObserver<IAssemblyStation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessMaterialObserver<IAssemblyStation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessMaterialObserver<IAssemblyStation, TransformationStartEvent, TransformationEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessMaterialObserver<IAssemblyStation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class EntryPoint
    {
      public class Entered : ProductTypeEventMaterialObserver<IEntryPoint, ThroughputStartEvent>
      {
        public Entered()
          : base("Entered")
        {
        }
      }
    }

    public static class ExitPoint
    {
      public class Exited : ProductTypeEventMaterialObserver<IExitPoint, ThroughputStartEvent>
      {
        public Exited()
          : base("Exited")
        {
        }
      }
    }

    public static class InhomogeneousParallelWorkstation
    {
      public class Blocked : ProcessMaterialObserver<IInhomogeneousParallelWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessMaterialObserver<IInhomogeneousParallelWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessMaterialObserver<IInhomogeneousParallelWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessMaterialObserver<IInhomogeneousParallelWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing :
        ProductTypeProcessMaterialObserver<IInhomogeneousParallelWorkstation, ProcessingStartEvent, ProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessMaterialObserver<IInhomogeneousParallelWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class InhomogeneousWorkstation
    {
      public class Blocked : ProcessMaterialObserver<IInhomogeneousWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessMaterialObserver<IInhomogeneousWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessMaterialObserver<IInhomogeneousWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessMaterialObserver<IInhomogeneousWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessMaterialObserver<IInhomogeneousWorkstation, ProcessingStartEvent, ProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessMaterialObserver<IInhomogeneousWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class ParallelWorkstation
    {
      public class Blocked : ProcessMaterialObserver<IParallelWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessMaterialObserver<IParallelWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessMaterialObserver<IParallelWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessMaterialObserver<IParallelWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessMaterialObserver<IParallelWorkstation, ProcessingStartEvent, ParallelProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessMaterialObserver<IParallelWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class ProbabilityAssemblyStation
    {
      public class Blocked : ProcessMaterialObserver<IProbabilityAssemblyStation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessMaterialObserver<IProbabilityAssemblyStation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessMaterialObserver<IProbabilityAssemblyStation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessMaterialObserver<IProbabilityAssemblyStation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing :
        ProductTypeProcessMaterialObserver<IProbabilityAssemblyStation, TransformationStartEvent, TransformationEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessMaterialObserver<IProbabilityAssemblyStation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }

    public static class Workstation
    {
      public class AwaitingResources : ProcessMaterialObserver<IWorkstation, ResourceRequestedEvent, ResourceReceivedEvent>
      {
        public AwaitingResources()
          : base("Awaiting Resources")
        {
        }
      }


      public class Blocked : ProcessMaterialObserver<IWorkstation, BlockedStartEvent, BlockedEndEvent>
      {
        public Blocked()
          : base("Blocked")
        {
        }
      }

      public class Failure : ProcessMaterialObserver<IWorkstation, FailureStartEvent, FailureEndEvent>
      {
        public Failure()
          : base("Failure")
        {
        }
      }

      public class Idle : ProcessMaterialObserver<IWorkstation, IdleStartEvent, IdleEndEvent>
      {
        public Idle()
          : base("Idle")
        {
        }
      }

      public class Off : ProcessMaterialObserver<IWorkstation, OffStartEvent, OffEndEvent>
      {
        public Off()
          : base("Off")
        {
        }
      }

      public class Processing : ProductTypeProcessMaterialObserver<IWorkstation, ProcessingStartEvent, ProcessingEndEvent>
      {
        public Processing()
          : base("Processing")
        {
        }
      }

      public class Setup : ProcessMaterialObserver<IWorkstation, SetupStartEvent, SetupEndEvent>
      {
        public Setup()
          : base("Setup")
        {
        }
      }
    }
  }
}