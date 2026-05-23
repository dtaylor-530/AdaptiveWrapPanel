using System.Windows;
using System.Windows.Controls;

namespace Voron.AdaptiveWrapPanel
{
    //[ContentProperty(nameof(Children))]
    public class AdaptiveWrapPanel : ScrollViewer
    {

#if DEBUG
        public static bool Debug { get; set; }
#endif

        #region DPs

      
        #endregion

        private ColumnWrapPanel Panel { get; set; }

        public AdaptiveWrapPanel()
        {
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Panel = VisualHelpers.FindVisualChild<ColumnWrapPanel>(this.Content as ItemsControl);
            if (Panel != null)
            {
                Panel.MeasureConstraint = constraint;
            }
            return base.MeasureOverride(constraint);
        }
    }
}
