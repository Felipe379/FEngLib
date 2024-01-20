using System.Windows;
using System.Windows.Controls;

namespace FEngViewer.WPF.UIHelpers;

public interface INamedEntity
{
    string Name { get; set; }
    bool IsNameExplicit { get; }
}

//public class NamedObjectStyleSelector : StyleSelector
//{
//    public Style? ExplicitNameStyle { get; set; }
//    public Style? DefaultStyle { get; set; }

//    public override Style? SelectStyle(object item, DependencyObject container)
//    {
//        if (item is INamedObject namedObject)
//        {
//            return (namedObject.IsNameExplicit ? ExplicitNameStyle : null) ?? DefaultStyle;
//        }
//        return base.SelectStyle(item, container);
//    }
//}