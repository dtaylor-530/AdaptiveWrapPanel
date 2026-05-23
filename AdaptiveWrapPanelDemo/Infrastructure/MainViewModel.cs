using System.Collections.ObjectModel;
using System.Windows.Controls;
using Voron.AdaptiveWrapPanel;


namespace Voron.AdaptiveWrapPanelDemo
{
    public class MainViewModel
    {
        public GeneratorSettings GeneratorSettings { get; set; } = new GeneratorSettings();

        public ObservableCollection<DemoItem> Items { get; } = new ObservableCollection<DemoItem>();

        public ObservableCollection<ColumnDefinition> ColumnDefinitions { get; } = new ObservableCollection<ColumnDefinition>();


        public ColumnWrapPanel Panel { get; set; }
    //{
    //    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
    //    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
    //    //HorizontalContentAlignment = HorizontalAlignment.Stretch,
    //    //VerticalContentAlignment = VerticalAlignment.Stretch
    //};

    }
}
