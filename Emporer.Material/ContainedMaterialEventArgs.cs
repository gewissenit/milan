using System;

namespace Emporer.Material
{
  public class ContainedMaterialEventArgs : EventArgs
  {
    public IContainedMaterial ContainedMaterial { get; set; }

    public ContainedMaterialEventArgs(IContainedMaterial containedMaterial)
    {
      ContainedMaterial = containedMaterial;
    }
  }
}