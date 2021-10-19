using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Chat.Client.ViewDatas
{
    public class AnimationSource : DependencyObject
    {
        private static readonly DependencyProperty ControlNameScopeProperty;

        public static readonly DependencyProperty ControlTypeNameScopeProperty;
        public static readonly DependencyProperty ControlToFindParentProperty;
        public static readonly DependencyProperty AnimationProperty;
        public static readonly DependencyProperty DoesAnimationStartProperty;

        static AnimationSource() 
        {
            ControlNameScopeProperty = DependencyProperty.Register(
                name: "ControlNameScope",
                propertyType: typeof(FrameworkElement),
                ownerType: typeof(AnimationSource));

            ControlTypeNameScopeProperty = DependencyProperty.Register(
                name: nameof(ControlTypeNameScope),
                propertyType: typeof(Type),
                ownerType: typeof(AnimationSource),
                typeMetadata: new PropertyMetadata(
                    propertyChangedCallback: OnControlTypeNameScopeChanged));

            ControlToFindParentProperty = DependencyProperty.Register(
                name: nameof(ControlToFindParent),
                propertyType: typeof(FrameworkElement),
                ownerType: typeof(AnimationSource),
                typeMetadata: new PropertyMetadata(
                    propertyChangedCallback: OnControlToFindParentChanged));

            AnimationProperty = DependencyProperty.Register(
                name: nameof(Animation),
                propertyType: typeof(Storyboard),
                ownerType: typeof(AnimationSource));

            DoesAnimationStartProperty = DependencyProperty.Register(
                name: nameof(DoesAnimationStart),
                propertyType: typeof(bool),
                ownerType: typeof(AnimationSource),
                typeMetadata: new PropertyMetadata(
                    propertyChangedCallback: OnDoesAnimationStartChanged));
        }

        public Type ControlTypeNameScope 
        {
            get => (Type)GetValue(ControlTypeNameScopeProperty);
            set => SetValue(ControlTypeNameScopeProperty, value);
        }

        public FrameworkElement ControlToFindParent 
        {
            get => (FrameworkElement)GetValue(ControlToFindParentProperty);
            set => SetValue(ControlToFindParentProperty, value);
        }

        public Storyboard Animation 
        {
            get => (Storyboard)GetValue(AnimationProperty);
            set => SetValue(AnimationProperty, value);
        }

        public bool DoesAnimationStart 
        {
            get => (bool)GetValue(DoesAnimationStartProperty);
            set => SetValue(DoesAnimationStartProperty, value);
        }

        private static void OnControlTypeNameScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            AnimationSource animation = d as AnimationSource;

            if (animation is null || animation.ControlToFindParent is null)
            {
                return;
            }

            animation.SetValue(ControlNameScopeProperty, FintAnsetorByType(animation.ControlToFindParent, e.NewValue as Type));
        }

        private static void OnControlToFindParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            AnimationSource animation = d as AnimationSource;

            if (animation is null || animation.GetValue(ControlNameScopeProperty) is not null)
            {
                return;
            }

            animation.SetValue(ControlNameScopeProperty, FintAnsetorByType(e.NewValue as DependencyObject, animation.ControlTypeNameScope));
        }

        private static void OnDoesAnimationStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            AnimationSource animation = d as AnimationSource;

            if (animation is null || animation.Animation is null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                if (animation.GetValue(ControlNameScopeProperty) is null)
                {
                    animation.SetValue(ControlNameScopeProperty, FintAnsetorByType(animation.ControlToFindParent, animation.ControlTypeNameScope));
                }

                animation.Animation.Begin(animation.GetValue(ControlNameScopeProperty) as FrameworkElement);
            }
            else 
            {
                animation.Animation.Stop(animation.GetValue(ControlNameScopeProperty) as FrameworkElement);
            }
        }

        private static DependencyObject FintAnsetorByType(DependencyObject start, Type ansetorType) 
        {
            DependencyObject ansetor = start;

            while (ansetor.GetType().FullName != ansetorType.FullName)
            {
                ansetor = LogicalTreeHelper.GetParent(ansetor);

                if (ansetor is null)
                {
                    break;
                }
            }

            if (ansetor is not null)
            {
                return ansetor;
            }

            ansetor = start;

            while (ansetor.GetType().FullName != ansetorType.FullName) 
            {
                ansetor = VisualTreeHelper.GetParent(ansetor);

                if (ansetor is null)
                {
                    return null;
                }
            }

            return ansetor;
        } 
    }
}
