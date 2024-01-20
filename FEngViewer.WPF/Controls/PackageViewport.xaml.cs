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
using FEngRender.Data;
using FEngRender.GL;
using SharpGL.WPF;

namespace FEngViewer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for PackageViewport.xaml
    /// </summary>
    public partial class PackageViewport : UserControl
    {
        private GLRenderTreeRenderer _renderer = null!;
        private TextureProvider _textureProvider;

        public static readonly DependencyProperty RenderTreeProperty = DependencyProperty.Register(nameof(RenderTree), typeof(RenderTree), typeof(PackageViewport), new PropertyMetadata(RenderTreeChangedCallback));

        private static void RenderTreeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PackageViewport packageViewport)
            {
                packageViewport._renderer.SetTree(packageViewport.RenderTree);
            }
        }

        public RenderTree RenderTree
        {
            get => (RenderTree)GetValue(RenderTreeProperty);
            set => SetValue(RenderTreeProperty, value);
        }

        public PackageViewport()
        {
            InitializeComponent();
            _textureProvider = new TextureProvider();
            _textureProvider.LoadTextures(@"G:\Software\Games\NFS\Most Wanted (2005)\All FNGs\textures");
        }

        private void GlControl_OnOpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
        {
            _renderer = new GLRenderTreeRenderer(GlControl.OpenGL, _textureProvider);
            _renderer.PrepareRender();
        }

        private void GlControl_OnOpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            _renderer.Render(true, 1.0f);
        }
    }
}
