#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Milan.VisualModeling.Extensions
{
  public static class ControlExtensions
  {
    public static IObservable<MouseButtonEventArgs> GetClick(this Control graphic)
    {
      var gDown = Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(h => graphic.MouseLeftButtonDown += h,
                                                                                      h => graphic.MouseLeftButtonDown -= h);


      var gUp = Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(h => graphic.MouseLeftButtonUp += h,
                                                                                    h => graphic.MouseLeftButtonUp -= h);


      var gMove = Observable.FromEvent<MouseEventHandler, MouseEventArgs>(h => graphic.MouseMove += h, h => graphic.MouseMove -= h);

      // wait for any mouse left down event
      return gDown.SelectMany(e => // then wait for a single mouse left up event
                              gUp.Take(1)
                                 .TakeUntil(gMove));
    }

    //public static IObservable<MouseButtonEventArgs> GetDoubleClick(this Graphic graphic,
    //                                                               int millisecondsBetweenClick)
    //{
    //    IObservable<MouseEventArgs> gMove = Observable.FromEvent
    //        (
    //            (EventHandler<MouseEventArgs> ev) => new MouseEventHandler(ev),
    //            ev => graphic.MouseMove += ev,
    //            ev => graphic.MouseMove += ev
    //        );

    //    return (from first in graphic.GetClick()
    //            from second in graphic.GetClick()
    //                                  .TakeUntil(gMove)
    //                                  .TakeUntil(Observable.Timer(TimeSpan.FromMilliseconds(millisecondsBetweenClick))
    //                                                       .Take(1))
    //            select first).Repeat();
    //}

    //public static IObservable<MouseButtonEventArgs> GetDoubleClick(this Graphic graphic)
    //{
    //    return graphic.GetDoubleClick(500);
    //}

    //public static IDisposable MakeDraggable(this Graphic graphic,
    //                                        Map map)
    //{
    //    graphic.RequireArgument<Graphic>("graphic")
    //           .NotNull<Graphic>();
    //    map.RequireArgument<Map>("map")
    //       .NotNull<Map>();

    //    //IObservable<MouseEventArgs> graphicMouseMove = Observable.FromEvent
    //    //    (
    //    //        ( EventHandler<MouseEventArgs> ev ) => new MouseEventHandler( ev ),
    //    //        ev => graphic.MouseMove += ev,
    //    //        ev => graphic.MouseMove += ev
    //    //    );

    //    IObservable<MouseEventArgs> mapMouseMove = Observable.FromEvent
    //        (
    //            (EventHandler<MouseEventArgs> ev) => new MouseEventHandler(ev),
    //            ev => map.MouseMove += ev,
    //            ev => map.MouseMove += ev
    //        );

    //    IObservable<MouseButtonEventArgs> graphicLeftMouseButtonDown = Observable.FromEvent
    //        (
    //            (EventHandler<MouseButtonEventArgs> ev) => new MouseButtonEventHandler(ev),
    //            ev => graphic.MouseLeftButtonDown += ev,
    //            ev => graphic.MouseLeftButtonDown -= ev
    //        );

    //    //IObservable<MouseButtonEventArgs> graphicLeftMouseButtonUp = Observable.FromEvent
    //    //    (
    //    //        (EventHandler<MouseButtonEventArgs> ev) => new MouseButtonEventHandler(ev),
    //    //        ev => graphic.MouseLeftButtonUp += ev,
    //    //        ev => graphic.MouseLeftButtonUp -= ev
    //    //    );

    //    IObservable<MouseButtonEventArgs> mapLeftMouseButtonUp = Observable.FromEvent
    //        (
    //            (EventHandler<MouseButtonEventArgs> ev) => new MouseButtonEventHandler(ev),
    //            ev => map.MouseLeftButtonUp += ev,
    //            ev => map.MouseLeftButtonUp -= ev
    //        );

    //    //var stopper = graphicLeftMouseButtonUp.Merge(mapLeftMouseButtonUp);
    //    //var dragger = graphicMouseMove.Merge(mapMouseMove);
    //    var stopper = mapLeftMouseButtonUp;
    //    var dragger = mapMouseMove;

    //    var drag = dragger.SkipUntil(graphicLeftMouseButtonDown)
    //                      .TakeUntil(stopper)
    //                      .Repeat();

    //    Func<MouseEventArgs> , IEvent <
    //    MouseEventArgs >,
    //    Unit > handler = (prev,
    //                      cur) =>
    //        {
    //            var prevMapPoint = map.ScreenToMap(prev.EventArgs.GetPosition(map));
    //            var curMapPoint = map.ScreenToMap(cur.EventArgs.GetPosition(map));
    //            var deltaX = curMapPoint.X - prevMapPoint.X;
    //            var deltaY = curMapPoint.Y - prevMapPoint.Y;
    //            graphic.Geometry.Offset(deltaX, deltaY);
    //            return new Unit();
    //        };

    //    var removeHandlers = new Disposables();
    //    removeHandlers.Subscriptions.Add(graphicLeftMouseButtonDown.Subscribe(e => e.EventArgs.Handled = true));
    //    removeHandlers.Subscriptions.Add
    //        (
    //            drag
    //                .Zip(drag.Skip(1), handler)
    //                .Subscribe()
    //        );

    //    return removeHandlers;
    //}

    //public static void Flash(this Graphic graphic,
    //                         double milliseconds,
    //                         int repeat)
    //{
    //    graphic.RequireArgument<Graphic>("graphic")
    //           .NotNull<Graphic>();

    //    var count = 1;
    //    repeat = repeat*2;
    //    Symbol tempSymbol = graphic.Symbol;
    //    var storyboard = new Storyboard();
    //    storyboard.Duration = TimeSpan.FromMilliseconds(milliseconds);
    //    graphic.Symbol = null;
    //    storyboard.Completed += (sender,
    //                             e) =>
    //        {
    //            if (count%2 == 1)
    //            {
    //                graphic.Symbol = tempSymbol;
    //            }
    //            else
    //            {
    //                graphic.Symbol = null;
    //            }

    //            if (count <= repeat)
    //            {
    //                storyboard.Begin();
    //            }

    //            count++;
    //        };
    //    storyboard.Begin();
    //}

    //public static void Flash(this Graphic graphic)
    //{
    //    graphic.RequireArgument<Graphic>("graphic")
    //           .NotNull<Graphic>();

    //    Flash(graphic, 200, 1);
    //}
  }
}