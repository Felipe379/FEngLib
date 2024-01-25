using Xceed.Wpf.Toolkit.PropertyGrid;

namespace FEngViewer.WPF.UIHelpers;

public interface IEditable
{
    PropertyDefinitionCollection GetPropertyDefinitions();
}