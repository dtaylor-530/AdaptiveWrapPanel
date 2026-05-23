using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Voron.AdaptiveWrapPanel;

namespace Voron.AdaptiveWrapPanelDemo
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{

        MainViewModel viewModel = new MainViewModel();


        public MainWindow()
		{
#if DEBUG
			AdaptiveWrapPanel.AdaptiveWrapPanel.Debug = true;
#endif
            this.Content = viewModel;
			InitializeComponent();

			initialiseWindow();


            new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Background, (sender, args) =>
			{
				foreach (DemoItem child in viewModel.Items)
				{
					child.DebugText = child.ToString();
				}
			}, Dispatcher.CurrentDispatcher).Start();

			void initialiseWindow()
			{
                var template = this.Resources["PanelsTemplate"] as DataTemplate;
                var panelWindow = new Window() { Content = viewModel, ContentTemplate = template };
                panelWindow.Closed += (sender, args) => Environment.Exit(0);
                Closed += (sender, args) => Environment.Exit(0);
                panelWindow.Show();
            }
        }


		private void ButtonBase_OnClickRemove(object sender, RoutedEventArgs e)
		{
            //Panel.Children.Remove(((DemoItem)((Button)sender).DataContext).Item);
            viewModel.Items.Remove((DemoItem)((Button)sender).DataContext);
		}

		private int index = 1;
		private void ButtonBase_OnClickAdd(object sender, RoutedEventArgs e)
		{
			try
			{
				var r = new Random();
				for (int i = 0; i < viewModel.GeneratorSettings.Count; i++)
				{
					var newItem = new DemoItem()
					{
						Text = $"A{index}",
						Background = new SolidColorBrush(
							Color.FromRgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255))),

						MinWidth = GeneratorSettings.CustomRange(r, viewModel.GeneratorSettings.MinWidthFrom, viewModel.GeneratorSettings.MinWidthTo),
						MinHeight = GeneratorSettings.CustomRange(r, viewModel.GeneratorSettings.MinHeightFrom, viewModel.GeneratorSettings.MinHeightTo),
						Width = GeneratorSettings.CustomRange(r, viewModel.GeneratorSettings.WidthFrom, viewModel.GeneratorSettings.WidthTo),
						Height = GeneratorSettings.CustomRange(r, viewModel.GeneratorSettings.HeightFrom, viewModel.GeneratorSettings.HeightTo),						
						HorizontalAlignment = viewModel.GeneratorSettings.HorizontalAlignment,
						VerticalAlignment = viewModel.GeneratorSettings.VerticalAlignment,
						ColumnBreakBehavior = viewModel.GeneratorSettings.ColumnBreakBehavior

					};
                    //Panel.Children.Add(newItem.Item);
                    viewModel.Items.Add(newItem);
					index++;
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message);
				Console.WriteLine(exception);
			}
		}

		private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
		{
			viewModel.Panel?.InvalidateMeasure();
			viewModel.Panel?.InvalidateArrange();
		}

		private void ButtonBase_OnClickClearAll(object sender, RoutedEventArgs e)
		{
            viewModel.Items.Clear();
			//Panel.Children.Clear();
		}

		private void ButtonBase_OnClickDelColDef(object sender, RoutedEventArgs e)
		{
			viewModel.Panel.ColumnDefinitions.Remove((ColumnDefinition)((Button)sender).DataContext);
		}

		private void ButtonBase_OnClickAddColDef(object sender, RoutedEventArgs e)
		{
			viewModel.Panel.ColumnDefinitions.Add(new ColumnDefinition());
		}

        private void ToggleButton_OnChecked(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
