using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Voron.AdaptiveWrapPanel
{
    //public partial class ColumnWrapPanel
    //{
    /// <summary>
    /// A column based layout panel, that automatically
    /// wraps to new column when required. The user
    /// may also create a new column before an element
    /// using the 
    /// </summary>
    public partial class ColumnWrapPanel : Panel
    {
        //private ColumnWrapPanel PanelContainer { get; }

        public static readonly DependencyProperty ColumnBreakBehaviorProperty = DependencyProperty.RegisterAttached(
          "ColumnBreakBehavior", typeof(ColumnBreakBehavior), typeof(ColumnWrapPanel), new FrameworkPropertyMetadata(updateLayout)
          {
              DefaultValue = ColumnBreakBehavior.Default,
              AffectsArrange = true,
              AffectsMeasure = true
          });

        private static void updateLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateLayout(d, e);
        }

        public ObservableCollection<ColumnDefinition> ColumnDefinitions { get; }
            = new ObservableCollection<ColumnDefinition>();

        public static void SetColumnBreakBehavior(UIElement element, ColumnBreakBehavior value)
        {
            element.SetValue(ColumnBreakBehaviorProperty, value);
        }

        public static ColumnBreakBehavior GetColumnBreakBehavior(UIElement element)
        {
            return (ColumnBreakBehavior)element.GetValue(ColumnBreakBehaviorProperty);
        }

        public ColumnWrapPanel()
        {
            ColumnDefinitions.CollectionChanged += (sender, args) =>
            {
                InvalidateMeasure();
                InvalidateArrange();
            };
        }

        public ColumnBreakBehavior DefaultBreakBehavior
        {
            get { return (ColumnBreakBehavior)GetValue(DefaultBreakBehaviorProperty); }
            set { SetValue(DefaultBreakBehaviorProperty, value); }
        }

   


        public static readonly DependencyProperty ChildFlowDirectionProperty =
            DependencyProperty.Register(nameof(ChildFlowDirection), typeof(ExpandDirection),
                typeof(ColumnWrapPanel), new PropertyMetadata(ExpandDirection.Down, UpdateLayout));

        private static void UpdateLayout(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is ColumnWrapPanel panel)
            {
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }

 
        public static readonly DependencyProperty DefaultBreakBehaviorProperty =
            DependencyProperty.Register(nameof(DefaultBreakBehavior), typeof(ColumnBreakBehavior), typeof(ColumnWrapPanel),
                new PropertyMetadata(ColumnBreakBehavior.Default, UpdateLayout));

        /// <summary>
        /// In development
        /// </summary>
        public ExpandDirection ChildFlowDirection
        {
            get { return (ExpandDirection)GetValue(ChildFlowDirectionProperty); }
            set { SetValue(ChildFlowDirectionProperty, value); }
        }



        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(nameof(VerticalContentAlignment), typeof(VerticalAlignment), typeof(ColumnWrapPanel), new PropertyMetadata());




        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalContentAlignment), typeof(HorizontalAlignment), typeof(ColumnWrapPanel), new PropertyMetadata());


        public Size MeasureConstraint
        {
            get { return (Size)GetValue(MeasureConstraintProperty); }
            set { SetValue(MeasureConstraintProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MeasureConstraint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasureConstraintProperty =
            DependencyProperty.Register(nameof(MeasureConstraint), typeof(Size), typeof(ColumnWrapPanel), new PropertyMetadata(updateLayout));


        private Size CalcPlacement(double overflowBorder, Size constraint, MeasureData[] data)
        {
            int firstInLine = 0;
            double accumulatedHeight = 0;
            var panelSize = new Size();
            bool newColumn = true;
            int columnIndex = -1;

            for (int i = 0; i < data.Length; i++)
            {
                if (newColumn)
                {
                    columnIndex++;
                    accumulatedHeight = 0;
                    firstInLine = i;
                }

                data[i].Overflow = accumulatedHeight + data[i].DesiredSize.Height - overflowBorder;

                //need to switch to another column
                if (!newColumn && data[i].ColumnBreakBehavior != ColumnBreakBehavior.DenyBreak && (
                        data[i].ColumnBreakBehavior == ColumnBreakBehavior.ForceNewColumn ||
                        (data[i].ColumnBreakBehavior == ColumnBreakBehavior.PreferNewColumn && !data[i].IgnoreBreak) ||
                        (data[i].Overflow > 0)))
                {
                    CompleteColumn(firstInLine, i, columnIndex, ref panelSize, constraint, data);
                    columnIndex++;
                    accumulatedHeight = data[i].DesiredSize.Height;
                    firstInLine = i;
                }
                else //continue to accumulate a column
                {
                    accumulatedHeight += data[i].DesiredSize.Height;
                    newColumn = false;
                }

                if (data[i].VerticalAlignment == VerticalAlignment.Stretch && !data[i].IgnoreVerticalStretch
                    && (i == data.Length - 1 || data[i + 1].ColumnBreakBehavior != ColumnBreakBehavior.DenyBreak))
                //if (InternalChildren[i].DesiredSize.Height >= overflowBorder)
                {
                    CompleteColumn(firstInLine, i + 1, columnIndex, ref panelSize, constraint, data);
                    newColumn = true;
                }

            }

            if (!newColumn)
                CompleteColumn(firstInLine, data.Length, columnIndex, ref panelSize, constraint, data);

            return panelSize;
        }

        private bool Shrink(Size visibleConstraint, MeasureData[] data, ref bool breaksOnly)
        {
            double freeSpace = 0;
            int bestIndex = -1;

            for (int i = 0; i < data.Length; i++)
            {
                double value = 0;
                if (breaksOnly)
                {
                    if (i == 0 || data[i].ColumnBreakBehavior != ColumnBreakBehavior.PreferNewColumn || data[i].IgnoreBreak)
                        continue;

                    value = visibleConstraint.Height - (data[i - 1].Rect.Top + data[i - 1].DesiredSize.Height);
                }
                else
                {
                    if (data[i].VerticalAlignment != VerticalAlignment.Stretch || data[i].IgnoreVerticalStretch)
                        continue;

                    value = data[i].Rect.Height;
                }


                if (value > freeSpace)
                {
                    freeSpace = value;
                    bestIndex = i;
                }
            }
            if (bestIndex != -1)
            {
                if (breaksOnly)
                    data[bestIndex].IgnoreBreak = true;
                else
                    data[bestIndex].IgnoreVerticalStretch = true;

                return true;
            }
            if (breaksOnly)
            {
                breaksOnly = false;
                return Shrink(visibleConstraint, data, ref breaksOnly);
            }
            return false;
        }

        private void CompleteColumn(int elStart, int elEnd, int columnIndex, ref Size panelSize, Size constraint, MeasureData[] data)
        {
            if (elEnd - elStart <= 0)
                return;

            double width = 0;
            double minHeightSum = 0;
            double stretchElementsCount = 0;
            for (int i = elStart; i < elEnd; i++)
            {
                data[i].ColumnIndex = columnIndex;
                width = Math.Max(width, data[i].DesiredSize.Width);
                minHeightSum += data[i].DesiredSize.Height;
                if (data[i].VerticalAlignment == VerticalAlignment.Stretch)
                {
                    stretchElementsCount++;
                }
            }

            double extraHeight = Math.Max(0, constraint.Height - minHeightSum);

            double top = 0;

            if (stretchElementsCount > 0)
            {
                extraHeight /= stretchElementsCount;
            }
            else
            {
                switch (VerticalContentAlignment)
                {
                    case VerticalAlignment.Stretch:
                        extraHeight /= (elEnd - elStart);
                        break;
                    case VerticalAlignment.Center:
                        top += extraHeight / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        top += extraHeight;
                        break;
                }
            }

            for (int i = elStart; i < elEnd; i++)
            {
                var y = top;
                var height = data[i].DesiredSize.Height;

                if (stretchElementsCount > 0)
                {
                    if (data[i].VerticalAlignment == VerticalAlignment.Stretch)
                    {
                        height += extraHeight;
                    }
                }
                else if (VerticalContentAlignment == VerticalAlignment.Stretch)
                {
                    switch (data[i].VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            y += extraHeight / 2;
                            break;
                        case VerticalAlignment.Bottom:
                            y += extraHeight;
                            break;
                    }
                    top += extraHeight;
                }
                top += height;

                var newRect = new Rect
                {
                    X = panelSize.Width,
                    Y = y,
                    Width = width,
                    Height = height
                };

                data[i].Rect = newRect;

            }

            panelSize.Height = Math.Max(top, panelSize.Height);
            panelSize.Width += width;
        }

        private void CalcColumnWidthes(MeasureData[] data, double widthConstrait)
        {
            var columnDefinitions = ColumnDefinitions.ToArray();
            if (data.Length <= 0)
                return;

            var columnData = new ColumnData[data[data.Length - 1].ColumnIndex + 1];
            for (int i = 0; i < columnData.Length; i++)
            {
                columnData[i] = i < columnDefinitions.Length ?
                new ColumnData(columnDefinitions[i]) : new ColumnData();
            }

            foreach (MeasureData child in data
                .Where(child => child.ColumnIndex >= columnDefinitions.Length))
            {
                columnData[child.ColumnIndex].Minimum =
                    Math.Max(child.DesiredSize.Width, columnData[child.ColumnIndex].Minimum);
            }

            bool stretchAuto = !columnData.Any(column => column.Value.IsStar) &&
                               HorizontalContentAlignment == HorizontalAlignment.Stretch;

            foreach (ColumnData column in columnData)
            {
                if (column.Value.IsAuto && stretchAuto)
                {
                    column.Value = new GridLength(1, GridUnitType.Star);
                }
                else if (!column.IsStar)
                {
                    column.ActualValue = column.IsAuto ? column.Minimum :
                        Math.Min(column.Maximum, Math.Max(column.Minimum, column.Value.Value));
                    column.Final = true;
                }
            }

            DistributeStarWidth(columnData, widthConstrait);

            double extraSpace = Math.Max(0, widthConstrait - columnData.Sum(column => column.ActualValue));
            double accumulatedWidth = 0;
            foreach (ColumnData column in columnData)
            {
                column.Offset = accumulatedWidth;
                switch (HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Center:
                        column.Offset += extraSpace / 2;
                        break;
                    case HorizontalAlignment.Right:
                        column.Offset += extraSpace;
                        break;
                }
                accumulatedWidth += column.ActualValue;
            }

            for (var i = 0; i < data.Length; i++)
            {
                var rect = data[i].Rect;
                rect.X = columnData[data[i].ColumnIndex].Offset;
                rect.Width = data[i].DesiredSize.Width;

                switch (data[i].HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        rect.X += (columnData[data[i].ColumnIndex].ActualValue - rect.Width) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        rect.X += columnData[data[i].ColumnIndex].ActualValue - rect.Width;
                        break;
                    case HorizontalAlignment.Stretch:
                        rect.Width = columnData[data[i].ColumnIndex].ActualValue;
                        break;
                }

                data[i].Rect = rect;
            }
        }

        private static void DistributeStarWidth(ColumnData[] columnData, double widthConstrait)
        {
            double starSum = columnData.Where(column => column.Value.IsStar).Sum(column => column.Value.Value);
            double minimumSum = columnData.Where(column => column.IsStar).Sum(column => column.Minimum);
            double starWidthPixelSum = Math.Max(0, widthConstrait -
                                            columnData.Where(column => column.Final).Sum(column => column.ActualValue));
            double extraSpace = starWidthPixelSum - minimumSum;

            bool reDistribute = true;
            while (reDistribute)
            {
                reDistribute = false;

                var spaceNeeded = 0d;
                foreach (ColumnData column in columnData)
                {
                    if (column.Final || !column.IsStar)
                        continue;

                    var desiredValue = column.Value.Value / starSum * starWidthPixelSum;

                    column.ActualValue = Math.Max(0, desiredValue - column.Minimum);
                    spaceNeeded += column.ActualValue;
                }

                foreach (ColumnData column in columnData)
                {
                    if (column.Final || !column.IsStar)
                        continue;

                    column.ActualValue = column.Minimum +
                        (extraSpace > 0 ? (column.ActualValue / spaceNeeded * extraSpace) : 0);

                    if (column.ActualValue >= column.Maximum)
                    {
                        extraSpace -= column.Maximum - column.Minimum;
                        starWidthPixelSum -= column.Maximum;
                        starSum -= column.Value.Value;
                        column.ActualValue = column.Maximum;
                        column.Final = true;
                        reDistribute = true;
                        break;
                    }
                }
            }
        }

        private MeasureData[] measureData = new MeasureData[0];

        //public ColumnWrapPanel(ColumnWrapPanel owner)
        //{
        //	PanelContainer = owner;
        //}

        private void TranslateExpandDirection(ref Size constraint, ref Size panelSize, MeasureData[] data, bool translateBack)
        {
            if (!translateBack)
            {
                for (var i = 0; i < data.Length; i++)
                {
                    switch (ChildFlowDirection)
                    {
                        case ExpandDirection.Up:

                            break;
                        case ExpandDirection.Left:
                            break;
                        case ExpandDirection.Right:
                            data[i].DesiredSize = new Size(data[i].DesiredSize.Height, data[i].DesiredSize.Width);
                            break;
                    }
                }
                if (ChildFlowDirection == ExpandDirection.Right)
                    constraint = new Size(constraint.Height, constraint.Width);
            }
            else
            {
                for (var i = 0; i < data.Length; i++)
                {
                    switch (ChildFlowDirection)
                    {
                        case ExpandDirection.Up:
                            data[i].Rect = Rect.Offset(data[i].Rect, 0, panelSize.Height - data[i].Rect.Height - 2 * data[i].Rect.Top);
                            break;
                        case ExpandDirection.Left:
                            //data[i].Rect = Rect.Offset(data[i].Rect, panelSize.Width - data[i].Rect.Width - 2 * data[i].Rect.Left, 0);
                            break;
                        case ExpandDirection.Right:
                            data[i].Rect = new Rect
                            {
                                X = data[i].Rect.Y,
                                Y = data[i].Rect.X,
                                Width = data[i].Rect.Height,
                                Height = data[i].Rect.Width
                            };
                            //data[i].DesiredSize = new Size(data[i].DesiredSize.Height, data[i].DesiredSize.Width);
                            break;
                    }
                }
                if (ChildFlowDirection == ExpandDirection.Right)
                {
                    constraint = new Size(constraint.Height, constraint.Width);
                    panelSize = new Size(panelSize.Height, panelSize.Width);
                }

            }
        }

        private Size MeasureArrange(Size constraint, bool arrange)
        {
            measureData = measureData.Length == InternalChildren.Count ?
                measureData : new MeasureData[InternalChildren.Count];

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                measureData[i] = new MeasureData
                {
                    ColumnBreakBehavior = GetColumnBreakBehavior(InternalChildren[i]),
                    DesiredSize = InternalChildren[i].DesiredSize
                };

                measureData[i].ColumnBreakBehavior = measureData[i].ColumnBreakBehavior == ColumnBreakBehavior.Default
                    ? DefaultBreakBehavior
                    : measureData[i].ColumnBreakBehavior;

                var frameworkElement = InternalChildren[i] as FrameworkElement;
                if (frameworkElement != null)
                {
                    measureData[i].HorizontalAlignment = frameworkElement.HorizontalAlignment;
                    measureData[i].VerticalAlignment = frameworkElement.VerticalAlignment;
                }

                if (!arrange)
                    InternalChildren[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                //constraint.Width - panelSize.Width,
                //constraint.Height));
            }
            Size panelSize = new Size();
            TranslateExpandDirection(ref constraint, ref panelSize, measureData, false);

            panelSize = CalcPlacement(constraint.Height, constraint, measureData);

            bool widthFit = panelSize.Width < constraint.Width;
            if (!widthFit)
            {
                if (!TryFitByShrinkFill(constraint, ref panelSize, ref measureData))
                {
                    var lowBorder = panelSize.Height;
                    panelSize = CalcPlacement(double.PositiveInfinity, constraint, measureData);
                    var highBorder = panelSize.Height;

                    // Is it possible to fit in width?
                    if (panelSize.Width < constraint.Width)
                    {
                        // Binary search first border height fitting in width

                        const double epsilon = 1d; // rational accuracy
                        while ((highBorder - lowBorder) > epsilon || panelSize.Width > constraint.Width)
                        {
                            var currentBorder = lowBorder + (highBorder - lowBorder) / 2d;

                            panelSize = CalcPlacement(currentBorder, constraint, measureData);

                            if (panelSize.Width < constraint.Width)
                                highBorder = currentBorder;
                            else
                                lowBorder = currentBorder;
                        }
                    }
                }
            }

            CalcColumnWidthes(measureData, constraint.Width);

            TranslateExpandDirection(ref constraint, ref panelSize, measureData, true);

            if (arrange)
            {
                for (var i = 0; i < InternalChildren.Count; i++)
                {
                    InternalChildren[i].Arrange(measureData[i].Rect);
                }
            }

            panelSize.Height = Math.Max(constraint.Height, panelSize.Height);
            panelSize.Width = Math.Max(constraint.Width, panelSize.Width);

#if DEBUG
            if (Debug)
            {
                for (var i = 0; i < InternalChildren.Count; i++)
                {
                    var el = InternalChildren[i] as FrameworkElement;
                    if (el != null)
                        el.DataContext = measureData[i];
                }
            }
#endif
            return panelSize;
        }


        public bool Debug { get; set; }

        private bool TryFitByShrinkFill(Size constraint,
            ref Size panelSize, ref MeasureData[] data)
        {
            var bestData = (MeasureData[])measureData.Clone();
            Size bestPanelSize = panelSize;
            bool widthFit = false;
            bool mode = true;
            while (Shrink(constraint, data, ref mode))
            {
                panelSize = CalcPlacement(constraint.Height, constraint, measureData);

                if (panelSize.Width <= constraint.Width && (!widthFit ||
                                                                   (bestPanelSize.Height > constraint.Height &&
                                                                    panelSize.Height < bestPanelSize.Height)))
                {
                    bestData = (MeasureData[])measureData.Clone();
                    bestPanelSize = panelSize;

                    widthFit = true;
                }
            }

            if (widthFit)
            {
                data = bestData;
                panelSize = bestPanelSize;
            }
            return widthFit;
        }

        // From MSDN : When overridden in a derived class, measures the 
        // size in layout required for child elements and determines a
        // size for the FrameworkElement-derived class
        protected override Size MeasureOverride(Size constraint)
        {
#if DEBUG
            if (Debug)
                System.Diagnostics.Debug.WriteLine($"Measure {constraint} {MeasureConstraint}");
#endif
            //return PanelContainer.MeasureConstraint;
            return MeasureArrange(MeasureConstraint, false);
            //new Size(
            //		Math.Min(constraint.Width, PanelContainer.MeasureConstraint.Width),
            //		Math.Min(constraint.Height, PanelContainer.MeasureConstraint.Height)), false);
        }

        //From MSDN : When overridden in a derived class, positions child
        //elements and determines a size for a FrameworkElement derived
        //class.
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
#if DEBUG
            if (Debug)
                System.Diagnostics.Debug.WriteLine($"Arrange {arrangeBounds} {MeasureConstraint}");
#endif
            return MeasureArrange(
                new Size(
                    Math.Min(arrangeBounds.Width, MeasureConstraint.Width),
                    Math.Min(arrangeBounds.Height, MeasureConstraint.Height)), true);
        }
    }
    //}
}