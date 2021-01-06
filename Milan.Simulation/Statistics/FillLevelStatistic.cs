#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
	public class FillLevelStatistic
	{
		public FillLevelStatistic(string storage, string productType, int capacity, Func<double> getCurrentTime)
		{
			Storage = storage;
			ProductType = productType;
			Capacity = capacity;
			Values = new HistoricalValueStore<int>(getCurrentTime);
		}

		public string Storage { get; private set; }
		public string ProductType { get; private set; }
		public HistoricalValueStore<int> Values { get; private set; }

		/*TODO: Remove
		 * This is used to calculate Full/Available/Empty states.
		 * It cant be retrieved otherwise by the consumer of this class
		 * (with a bit more effort) and should possibly be removed (SOC!)
		 * 
		 * This is the only non-keyish property besides the value store. 
		 */
		public int Capacity { get; private set; }
	}
}