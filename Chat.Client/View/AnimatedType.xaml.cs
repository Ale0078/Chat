using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat.Client.View
{
    public partial class AnimatedType : UserControl
    {
        public static readonly DependencyProperty EllipseWidthProperty;
        public static readonly DependencyProperty EllipseHeightProperty;
        public static readonly DependencyProperty EllipseMarginProperty;
        public static readonly DependencyProperty EllipseColorProperty;

        static AnimatedType() 
        {
            EllipseWidthProperty = DependencyProperty.Register(
                name: nameof(EllipseWidth),
                propertyType: typeof(double),
                ownerType: typeof(AnimatedType));

            EllipseHeightProperty = DependencyProperty.Register(
                name: nameof(EllipseHeight),
                propertyType: typeof(double),
                ownerType: typeof(AnimatedType));

            EllipseMarginProperty = DependencyProperty.Register(
                name: nameof(EllipseMargin),
                propertyType: typeof(Thickness),
                ownerType: typeof(AnimatedType));

            EllipseColorProperty = DependencyProperty.Register(
                name: nameof(EllipseColor),
                propertyType: typeof(Color),
                ownerType: typeof(AnimatedType));
        }

        public AnimatedType()
        {
            InitializeComponent();
        }

        public double EllipseWidth 
        {
            get => (double)GetValue(EllipseWidthProperty);
            set => SetValue(EllipseWidthProperty, value);
        }

        public double EllipseHeight 
        {
            get => (double)GetValue(EllipseHeightProperty);
            set => SetValue(EllipseHeightProperty, value);
        }

        public Thickness EllipseMargin 
        {
            get => (Thickness)GetValue(EllipseMarginProperty);
            set => SetValue(EllipseMarginProperty, value);
        }

        public Color EllipseColor 
        {
            get => (Color)GetValue(EllipseColorProperty);
            set => SetValue(EllipseColorProperty, value);
        }
    }
}
