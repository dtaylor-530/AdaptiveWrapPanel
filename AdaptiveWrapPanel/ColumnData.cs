using System.Windows.Controls;
using System.Windows;

namespace Voron.AdaptiveWrapPanel
{

    internal class ColumnData
    {
        public ColumnData() { }

        public ColumnData(ColumnDefinition definition)
        {
            this.Minimum = definition.MinWidth;
            this.Value = definition.Width;
            this.Maximum = definition.MaxWidth;
        }

        public double Minimum { get; set; }
        public GridLength Value { get; set; } = GridLength.Auto;
        public double Maximum { get; set; } = double.PositiveInfinity;
        public double Offset { get; set; }
        public double ActualValue { get; set; }
        public bool Final { get; set; }
        public GridUnitType Type => Value.GridUnitType;
        public bool IsAuto => Type == GridUnitType.Auto;
        public bool IsAbsolute => Type == GridUnitType.Pixel;
        public bool IsStar => Type == GridUnitType.Star;
    }
}
	
