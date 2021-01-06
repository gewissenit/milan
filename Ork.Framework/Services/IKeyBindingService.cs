using System;

namespace Ork.Framework.Services
{
  public interface IKeyBindingService
  {
    void SubscribeFor(string keySequence, object subscriber, Action action);
    void UnsubscribeFor(string keySequence, object subscriber);
    void ReceiveKeyReleased(string releasedKey, string sender);
    void ReceiveKeyPressed(string pressedKey, string sender);
  }
}