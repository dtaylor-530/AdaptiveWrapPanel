using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Voron.AdaptiveWrapPanel;


namespace Voron.AdaptiveWrapPanelDemo
{
    public class DemoItem : INotifyPropertyChanged
    {
        private double width;
        private double height;
        private double minWidth;
        private double maxWidth;
        private double minHeight;
        private double maxHeight;
        private HorizontalAlignment horizontalAlignment;
        private VerticalAlignment verticalAlignment;
        private Brush background;
        private ColumnBreakBehavior columnBreakBehavior;
        private string debugText;
        private string text;

        public double Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    width = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    height = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double MinWidth
        {
            get => minWidth;
            set
            {
                if (minWidth != value)
                {
                    minWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double MaxWidth
        {
            get => maxWidth;
            set
            {
                if (maxWidth != value)
                {
                    maxWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double MinHeight
        {
            get => minHeight;
            set
            {
                if (minHeight != value)
                {
                    minHeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double MaxHeight
        {
            get => maxHeight;
            set
            {
                if (maxHeight != value)
                {
                    maxHeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                if (horizontalAlignment != value)
                {
                    horizontalAlignment = value;
                    RaisePropertyChanged();
                }
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                if (verticalAlignment != value)
                {
                    verticalAlignment = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Brush Background
        {
            get => background;
            set
            {
                if (background != value)
                {
                    background = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ColumnBreakBehavior ColumnBreakBehavior
        {
            get => columnBreakBehavior;
            set
            {
                if (columnBreakBehavior != value)
                {
                    columnBreakBehavior = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                RaisePropertyChanged();
            }
        }

        public string DebugText
        {
            get { return debugText; }
            set
            {
                debugText = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
