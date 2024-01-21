using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FEngRender.GL;
using FEngViewer.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using SharpGL.WPF;

namespace FEngViewer.WPF.Views
{
    /// <summary>
    /// Interaction logic for PackageView.xaml
    /// </summary>
    public partial class PackageView : UserControl
    {
        private GLRenderTreeRenderer _glRenderTreeRenderer;

        public PackageView()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = App.Current.Services.GetService<PackageViewModel>();
            }
        }
    }

    public class MessageTreeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
        {
            if (item is CollectionViewGroup)
                return (DataTemplate)((FrameworkElement)container).FindResource("MessageGroupDataTemplate");
            if (item is MessageDefinitionViewModel)
                return (DataTemplate)((FrameworkElement)container).FindResource("MessageDataTemplate");
            return base.SelectTemplate(item, container);
        }
    }
}
