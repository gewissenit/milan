using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Emporer.WPF.Behaviors
{
  public class VisibilityAnimation
  {
    public enum AnimationType
    {
      None,
      Fade
    }

    private const int AnimationDuration = 300;

    private static readonly Dictionary<FrameworkElement, bool> _hookedElements =
      new Dictionary<FrameworkElement, bool>();

    public static readonly DependencyProperty AnimationTypeProperty =
      DependencyProperty.RegisterAttached("AnimationType",
                                          typeof (AnimationType),
                                          typeof (VisibilityAnimation),
                                          new FrameworkPropertyMetadata(AnimationType.None,
                                                                        OnAnimationTypePropertyChanged));

    static VisibilityAnimation()
    {
      // Here we "register" on Visibility property "before change" event
      UIElement.VisibilityProperty.AddOwner(typeof (FrameworkElement),
                                            new FrameworkPropertyMetadata(Visibility.Visible,
                                                                          VisibilityChanged,
                                                                          CoerceVisibility));
    }

    public static AnimationType GetAnimationType(DependencyObject obj)
    {
      return (AnimationType) obj.GetValue(AnimationTypeProperty);
    }

    public static void SetAnimationType(DependencyObject obj, AnimationType value)
    {
      obj.SetValue(AnimationTypeProperty, value);
    }

    private static void OnAnimationTypePropertyChanged(DependencyObject dependencyObject,
                                                       DependencyPropertyChangedEventArgs e)
    {
      var frameworkElement = dependencyObject as FrameworkElement;

      if (frameworkElement == null)
      {
        return;
      }

      // If AnimationType is set to True on this framework element, 
      if (GetAnimationType(frameworkElement) != AnimationType.None)
      {
        // Add this framework element to hooked list
        HookVisibilityChanges(frameworkElement);
      }
      else
      {
        // Otherwise, remove it from the hooked list
        UnHookVisibilityChanges(frameworkElement);
      }
    }

    private static void HookVisibilityChanges(FrameworkElement frameworkElement)
    {
      _hookedElements.Add(frameworkElement, false);
    }

    private static void UnHookVisibilityChanges(FrameworkElement frameworkElement)
    {
      if (_hookedElements.ContainsKey(frameworkElement))
      {
        _hookedElements.Remove(frameworkElement);
      }
    }

    private static void VisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      // Ignore
    }

    private static object CoerceVisibility(DependencyObject dependencyObject, object baseValue)
    {
      // Make sure object is a framework element
      var frameworkElement = dependencyObject as FrameworkElement;
      if (frameworkElement == null)
      {
        return baseValue;
      }

      // Cast to type safe value
      var visibility = (Visibility) baseValue;

      // If Visibility value hasn't change, do nothing.
      // This can happen if the Visibility property is set using data binding 
      // and the binding source has changed but the new visibility value 
      // hasn't changed.
      if (visibility == frameworkElement.Visibility)
      {
        return baseValue;
      }

      // If element is not hooked by our attached property, stop here
      if (!IsHookedElement(frameworkElement))
      {
        return baseValue;
      }

      // Update animation flag
      // If animation already started, don't restart it (otherwise, infinite loop)
      if (UpdateAnimationStartedFlag(frameworkElement))
      {
        return baseValue;
      }

      // If we get here, it means we have to start fade in or fade out animation. 
      // In any case return value of this method will be Visibility.Visible, 
      // to allow the animation.
      var doubleAnimation = new DoubleAnimation
                            {
                              Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration))
                            };

      // When animation completes, set the visibility value to the requested 
      // value (baseValue)
      doubleAnimation.Completed += (sender, eventArgs) =>
                                   {
                                     if (visibility == Visibility.Visible)
                                     {
                                       // In case we change into Visibility.Visible, the correct value 
                                       // is already set, so just update the animation started flag
                                       UpdateAnimationStartedFlag(frameworkElement);
                                     }
                                     else
                                     {
                                       // This will trigger value coercion again 
                                       // but UpdateAnimationStartedFlag() function will reture true 
                                       // this time, thus animation will not be triggered. 
                                       if (BindingOperations.IsDataBound(frameworkElement, UIElement.VisibilityProperty))
                                       {
                                         // Set visiblity using bounded value
                                         var bindingValue = BindingOperations.GetBinding(frameworkElement,
                                                                                         UIElement.VisibilityProperty);
                                         BindingOperations.SetBinding(frameworkElement,
                                                                      UIElement.VisibilityProperty,
                                                                      bindingValue);
                                       }
                                       else
                                       {
                                         // No binding, just assign the value
                                         frameworkElement.Visibility = visibility;
                                       }
                                     }
                                   };

      if (visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
      {
        // Fade out by animating opacity
        doubleAnimation.From = 1.0;
        doubleAnimation.To = 0.0;
      }
      else
      {
        // Fade in by animating opacity
        doubleAnimation.From = 0.0;
        doubleAnimation.To = 1.0;
      }

      // Start animation
      frameworkElement.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);

      // Make sure the element remains visible during the animation
      // The original requested value will be set in the completed event of 
      // the animation
      return Visibility.Visible;
    }

    private static bool IsHookedElement(FrameworkElement frameworkElement)
    {
      return _hookedElements.ContainsKey(frameworkElement);
    }

    private static bool UpdateAnimationStartedFlag(FrameworkElement frameworkElement)
    {
      var animationStarted = _hookedElements[frameworkElement];
      _hookedElements[frameworkElement] = !animationStarted;

      return animationStarted;
    }
  }
}