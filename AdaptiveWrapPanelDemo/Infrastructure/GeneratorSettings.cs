using System;
using System.Windows;
using Voron.AdaptiveWrapPanel;


namespace Voron.AdaptiveWrapPanelDemo
{
    public class GeneratorSettings
	{
		public double MinWidthFrom { get; set; } = 100;
		public double MinWidthTo { get; set; } = 250;
		public double MaxWidthFrom { get; set; } = double.PositiveInfinity;
		public double MaxWidthTo { get; set; } = double.PositiveInfinity;
		public double WidthFrom { get; set; } = double.NaN;
		public double WidthTo { get; set; } = double.NaN;
		public double MinHeightFrom { get; set; } = 100;
		public double MinHeightTo { get; set; } = 250;
		public double MaxHeightFrom { get; set; } = double.PositiveInfinity;
		public double MaxHeightTo { get; set; } = double.PositiveInfinity;
		public double HeightFrom { get; set; } = double.NaN;
		public double HeightTo { get; set; } = double.NaN;
		public int Count { get; set; } = 10;

		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }
		public ColumnBreakBehavior ColumnBreakBehavior { get; set; }

		public static double CustomRange(Random r, double from, double to)
		{
			if (double.IsNaN(from) || double.IsNaN(to))
				return double.NaN;
			if (double.IsPositiveInfinity(from) || double.IsPositiveInfinity(to))
				return double.PositiveInfinity;
			if (double.IsNegativeInfinity(from) || double.IsNegativeInfinity(to))
				return double.NegativeInfinity;
			return from + r.Next((int)(to - from));
		}
	}
}
