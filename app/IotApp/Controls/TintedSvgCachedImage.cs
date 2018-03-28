using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Xamarin.Forms;

namespace IotApp.Controls
{
    public class TintedSvgCachedImage : SvgCachedImage
    {
        public static BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(TintedSvgCachedImage), Color.Transparent, propertyChanged: UpdateColor);

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        private static void UpdateColor(BindableObject bindable, object oldColor, object newColor)
        {
            var oldcolor = (Color)oldColor;
            var newcolor = (Color)newColor;

            if (!oldcolor.Equals(newcolor))
            {
                var view = (TintedSvgCachedImage)bindable;
                var transformations = new System.Collections.Generic.List<ITransformation>() {
                    new TintTransformation((int)(newcolor.R * 255), (int)(newcolor.G * 255), (int)(newcolor.B * 255), (int)(newcolor.A * 255)) {
                        EnableSolidColor = true
                    }
                };
                view.Transformations = transformations;
            }
        }
    }
}
