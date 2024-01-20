using System;
using System.Collections.Generic;
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

namespace FEngViewer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for EntityNameControl.xaml
    /// </summary>
    public partial class EntityName : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(string), typeof(EntityName));
        public static readonly DependencyProperty ExplicitProperty = DependencyProperty.Register(nameof(IsExplicit), typeof(bool), typeof(EntityName));
        
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool IsExplicit
        {
            get => (bool)GetValue(ExplicitProperty);
            set => SetValue(ExplicitProperty, value);
        }

        public EntityName()
        {
            InitializeComponent();
        }
    }
}
