using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Util;
using Avails.Xamarin.Controls.CurvedCornersLabel;
using PersonalTrainerWorkouts.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using LabelRenderer = Xamarin.Forms.Platform.Android.FastRenderers.LabelRenderer;

[assembly: ExportRenderer(typeof(CurvedCornersLabel)
                        , typeof(CurvedCornersLabelRenderer))]
namespace PersonalTrainerWorkouts.Droid.Renderers
{
    public class CurvedCornersLabelRenderer : LabelRenderer
    {
        private GradientDrawable _gradientBackground;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (CurvedCornersLabel)Element;
            if (view == null) return;

            // creating gradient drawable for the curved background
            _gradientBackground = new GradientDrawable();
            _gradientBackground.SetShape(ShapeType.Rectangle);
            _gradientBackground.SetColor(view.CurvedBackgroundColor.ToAndroid());

            // Thickness of the stroke line
            _gradientBackground.SetStroke(4, view.CurvedBackgroundColor.ToAndroid());

            // Radius for the curves
            _gradientBackground.SetCornerRadius(DpToPixels(Context
                                                         , Convert.ToSingle(view.CurvedCornerRadius)));

            // set the background of the label
            Control.SetBackground(_gradientBackground);
        }

        /// <summary>
        /// Device Independent Pixels to Actual Pixels conversion
        /// </summary>
        /// <param name="context"></param>
        /// <param name="valueInDp"></param>
        /// <returns></returns>
        private static float DpToPixels(Context context, float valueInDp)
        {
            var metrics = context?.Resources?.DisplayMetrics;

            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }

    }
}
