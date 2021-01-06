#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace Ork.Framework
{
  public class ApplicationCloseStrategy : ICloseStrategy<IWorkspace>
  {
    private Action<bool, IEnumerable<IWorkspace>> _callback;
    private IEnumerator<IWorkspace> _enumerator;
    private bool _finalResult;

    public void Execute(IEnumerable<IWorkspace> toClose, Action<bool, IEnumerable<IWorkspace>> cb)
    {
      _enumerator = toClose.GetEnumerator();
      _callback = cb;
      _finalResult = true;

      Evaluate(_finalResult);
    }

    private void Evaluate(bool result)
    {
      _finalResult = _finalResult && result;

      if (!_enumerator.MoveNext() ||
          !result)
      {
        _callback(_finalResult, new List<IWorkspace>());
      }
      else
      {
        var current = _enumerator.Current;

        var conductor = current as IConductor;
        if (conductor != null)
        {
          var tasks = conductor.GetChildren()
                               .OfType<IHaveShutdownTask>()
                               .Select(x => x.GetShutdownTask())
                               .Where(x => x != null);

          var sequential = new SequentialResult(tasks.GetEnumerator());
          sequential.Completed += (s, e) =>
                                  {
                                    if (!e.WasCancelled)
                                    {
                                      Evaluate(!e.WasCancelled);
                                    }
                                  };
          sequential.Execute(new CoroutineExecutionContext());
        }
        else
        {
          var haveShutdownTask = current as IHaveShutdownTask;
          if (haveShutdownTask != null)
          {
            var shutdownTask = haveShutdownTask.GetShutdownTask();
            if (shutdownTask != null)
            {
              shutdownTask.Completed += (s, e) =>
                                        {
                                          if (!e.WasCancelled)
                                          {
                                            Evaluate(!e.WasCancelled);
                                          }
                                        };
              IoC.BuildUp(shutdownTask);
              shutdownTask.Execute(new CoroutineExecutionContext());
            }
            else
            {
              Evaluate(true);
            }
          }
          else
          {
            Evaluate(true);
          }
        }
      }
    }
  }
}