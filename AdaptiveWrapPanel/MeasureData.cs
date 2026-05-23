using System;
using System.Windows;

namespace Voron.AdaptiveWrapPanel
{
    internal struct MeasureData
    {
        public Rect Rect { get; set; }
        public Size DesiredSize { get; set; }
        public ColumnBreakBehavior ColumnBreakBehavior { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public int ColumnIndex { get; set; }
        public bool IgnoreVerticalStretch { get; set; }
        public bool IgnoreBreak { get; set; }
        public double Overflow { get; set; }

        public override string ToString()
        {
            return string.Join(", " + Environment.NewLine,
                $"{nameof(Rect)} {Rect}",
                $"{nameof(DesiredSize)} {DesiredSize}",
                $"{nameof(ColumnIndex)} {ColumnIndex}",
                $"{nameof(ColumnBreakBehavior)} {ColumnBreakBehavior}",
                $"{nameof(HorizontalAlignment)} {HorizontalAlignment}",
                $"{nameof(VerticalAlignment)} {VerticalAlignment}",
                $"{nameof(IgnoreVerticalStretch)} {IgnoreVerticalStretch}",
                $"{nameof(IgnoreBreak)} {IgnoreBreak}",
                $"{nameof(Overflow)} {Overflow}");
        }
    }

    //}
}