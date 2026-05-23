using System.Windows.Controls;

namespace Voron.AdaptiveWrapPanel
{

    public class LengthDefinition : ColumnDefinition
    {
        public new double Offset { get; protected internal set; }
        public new double ActualWidth { get; protected internal set; }
    }
}
