using System;
using Milan.Simulation.Statistics;

namespace Milan.Simulation.Common.Observers
{
	public class DurationBasedStatistic: HistoricalValueStore<TimeSpan>
	{
		public DurationBasedStatistic(Func<double> getCurrentTime)
			: base(ConvertToDouble, ConvertFromDouble, getCurrentTime)
		{
		}


		private static double ConvertToDouble(TimeSpan value)
		{
			return Convert.ToDouble(value.Ticks);
		}

		private static TimeSpan ConvertFromDouble(double value)
		{
			var ticks = Convert.ToInt64(value);
			return TimeSpan.FromTicks(ticks);
		}
	}
}