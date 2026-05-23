using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voron.AdaptiveWrapPanelDemo
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public static class ItemsControlPanelBehavior
    {
        public static readonly DependencyProperty PanelProperty =
            DependencyProperty.RegisterAttached(
                "Panel",
                typeof(Panel),
                typeof(ItemsControlPanelBehavior),
                new PropertyMetadata(null));

        public static void SetPanel(DependencyObject element, Panel value)
            => element.SetValue(PanelProperty, value);

        public static Panel GetPanel(DependencyObject element)
            => (Panel)element.GetValue(PanelProperty);

        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(ItemsControlPanelBehavior),
                new PropertyMetadata(false, OnEnableChanged));

        public static void SetEnable(DependencyObject element, bool value)
            => element.SetValue(EnableProperty, value);

        public static bool GetEnable(DependencyObject element)
            => (bool)element.GetValue(EnableProperty);

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl itemsControl))
                return;

            if ((bool)e.NewValue)
            {
                itemsControl.Loaded += ItemsControl_Loaded;
            }
            else
            {
                itemsControl.Loaded -= ItemsControl_Loaded;
            }
        }

        private static void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsControl = (ItemsControl)sender;

            itemsControl.ApplyTemplate();

            var presenter = FindVisualChild<ItemsPresenter>(itemsControl);
            if (presenter == null)
                return;

            var panel = VisualTreeHelper.GetChild(presenter, 0) as Panel;

            SetPanel(itemsControl, panel);
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
