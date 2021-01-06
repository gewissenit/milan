#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.CostAccounting
{
  public static class Utils
  {
    public static void CloneCostObserver(ICostObserver clone, ICostObserver master)
    {
      clone.Amount = master.Amount;
      clone.LossRatio = master.LossRatio;
      clone.Currency = master.Currency;
      clone.Category = master.Category;
    }
  }
}