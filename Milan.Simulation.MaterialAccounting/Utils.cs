namespace Milan.Simulation.MaterialAccounting
{
  public static class Utils
  {
    public static void CloneMaterialObserver(IMaterialObserver clone, IMaterialObserver master)
    {
      clone.Amount = master.Amount;
      clone.Unit = master.Unit;
      clone.LossRatio= master.LossRatio;
      clone.Category = master.Category;
      clone.Material = master.Material;
    }
  }
}