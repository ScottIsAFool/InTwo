using System;
using Coding4Fun.Toolkit.Controls;

namespace InTwo.Controls.TimeSpanPicker.DataSource
{
	public class SecondTimeSpanDataSource : TimeSpanDataSource
	{
		public SecondTimeSpanDataSource() : base(30, 1) { }

		public SecondTimeSpanDataSource(int max, int step) : base(max, step) { }

		protected override TimeSpan? GetRelativeTo(TimeSpan relativeDate, int delta)
		{
            // These are required to make sure 0 seconds isn't an option
            if (relativeDate.Seconds == 1 && delta < 0) relativeDate = relativeDate.Subtract(new TimeSpan(0, 0, 1));
            if (relativeDate.Seconds == 30 && delta > 0) relativeDate = new TimeSpan(0, 0, 0);

			return new TimeSpan(relativeDate.Hours, relativeDate.Minutes, ComputeRelativeTo(relativeDate.Seconds, delta));
		}
	}
}
