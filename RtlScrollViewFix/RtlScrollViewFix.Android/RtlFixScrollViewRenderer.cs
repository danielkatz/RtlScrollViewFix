using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ScrollView), typeof(RtlScrollViewFix.Droid.RtlFixScrollViewRenderer))]
namespace RtlScrollViewFix.Droid
{
    public class RtlFixScrollViewRenderer : ScrollViewRenderer
    {
        LayoutDirection currentDirection;

        public RtlFixScrollViewRenderer(Context context) : base(context)
        {
            currentDirection = LayoutDirection;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            UpdateFlowDirection();

            if (e.OldElement != null)
            {
                e.NewElement.PropertyChanged -= OnElementPropertyChanged;
            }

            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnElementPropertyChanged;
            }
        }

        private void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
            {
                UpdateFlowDirection();
            }
        }

        void UpdateFlowDirection()
        {
            if (Element is IVisualElementController controller)
            {
                LayoutDirection = controller.EffectiveFlowDirection.IsLeftToRight()
                    ? LayoutDirection.Ltr
                    : LayoutDirection.Rtl;
            }
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            if (Element is Xamarin.Forms.ScrollView scrollView && (scrollView.Orientation == ScrollOrientation.Horizontal || scrollView.Orientation == ScrollOrientation.Both))
            {
                var horizontal = (HorizontalScrollView)GetChildAt(0);
                var container = (ViewGroup)horizontal.GetChildAt(0);

                if (LayoutDirection == LayoutDirection.Rtl)
                {
                    if (container.ChildCount == 1)
                    {
                        var content = (ViewGroup)container.GetChildAt(0);
                        var desiredWidth = content.Width;

                        container.Layout(0, 0, content.Width, content.Height);
                        content.Layout(0, 0, desiredWidth, content.Height);
                    }
                }

                if (LayoutDirection != currentDirection)
                {
                    currentDirection = LayoutDirection;

                    horizontal.ScrollX = LayoutDirection == LayoutDirection.Ltr
                        ? 0
                        : container.Right;
                }
            }
        }
    }
}