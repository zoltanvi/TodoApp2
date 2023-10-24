// See: https://github.com/albi005/MaterialColorUtilities
using System;

namespace TodoApp2.Material
{
    /// <summary>
    /// Contains tonal palettes for the key colors.
    /// </summary>
    public class CorePalette
    {
        public TonalPalette Primary { get; set; }
        public TonalPalette Secondary { get; set; }
        public TonalPalette Tertiary { get; set; }
        public TonalPalette Neutral { get; set; }
        public TonalPalette NeutralVariant { get; set; }
        public TonalPalette Error { get; set; }

        /// <summary>Creates an empty core palette.</summary>
        public CorePalette()
        {
        }

        /// <summary>Create key tones from a color using the default strategy.</summary>
        /// <param name="seed">ARGB representation of a color.</param>
        public static CorePalette Of(uint seed) => Of(seed, ThemeStyle.TonalSpot);

        /// <summary>Create content key tones from a color.</summary>
        /// <param name="seed">ARGB representation of a color.</param>
        public static CorePalette ContentOf(uint seed) => Of(seed, ThemeStyle.Content);

        /// <summary>Create key tones from a color.</summary>
        /// <param name="seed">ARGB representation of a color.</param>
        /// <param name="style">
        /// The strategy that decides what hue and chroma the created tonal palettes should have.
        /// </param>
        public static CorePalette Of(uint seed, ThemeStyle style)
        {
            CorePalette corePalette = new CorePalette();
            corePalette.Fill(seed, style);
            return corePalette;
        }

        public virtual void Fill(uint seed, ThemeStyle style = ThemeStyle.TonalSpot)
        {
            Hct hct = Hct.FromInt(seed);
            double hue = hct.Hue;
            double chroma = hct.Chroma;

            // From https://android.googlesource.com/platform/frameworks/base/+/5ecdfa15559482676402d61463cc51faeb6e18c8/packages/SystemUI/monet/src/com/android/systemui/monet/ColorScheme.kt#158
            switch (style)
            {
                case ThemeStyle.Spritz:
                    Primary = new TonalPalette(hue, 12);
                    Secondary = new TonalPalette(hue, 8);
                    Tertiary = new TonalPalette(hue, 16);
                    Neutral = new TonalPalette(hue, 2);
                    NeutralVariant = new TonalPalette(hue, 2);
                    break;
                case ThemeStyle.TonalSpot:
                    Primary = new TonalPalette(hue, 36);
                    Secondary = new TonalPalette(hue, 16);
                    Tertiary = new TonalPalette(hue + 60, 24);
                    Neutral = new TonalPalette(hue, 4);
                    NeutralVariant = new TonalPalette(hue, 8);
                    break;
                case ThemeStyle.Vibrant:
                    Primary = new TonalPalette(hue, CHROMA_MAX_OUT);
                    Secondary = new TonalPalette(
                        MathUtils.RotateHue(hue, (0, 18), (41, 15), (61, 10), (101, 12), (131, 15), (181, 18), (251, 15), (301, 12), (360, 12)),
                        24);
                    Tertiary = new TonalPalette(
                        MathUtils.RotateHue(hue, (0, 35), (41, 30), (61, 20), (101, 25), (131, 30), (181, 35), (251, 30), (301, 25), (360, 25)),
                        32);
                    Neutral = new TonalPalette(hue, 10);
                    NeutralVariant = new TonalPalette(hue, 12);
                    break;
                case ThemeStyle.Expressive:
                    Primary = new TonalPalette(hue + 240, 40);
                    Secondary = new TonalPalette(
                        MathUtils.RotateHue(hue, (0, 45), (21, 95), (51, 45), (121, 20), (151, 45), (191, 90), (271, 45), (321, 45), (360, 45)),
                        24);
                    Tertiary = new TonalPalette(
                        MathUtils.RotateHue(hue, (0, 120), (21, 120), (51, 20), (121, 45), (151, 20), (191, 15), (271, 20), (321, 120), (360, 120)),
                        32);
                    Neutral = new TonalPalette(hue + 15, 8);
                    NeutralVariant = new TonalPalette(hue + 15, 12);
                    break;
                case ThemeStyle.Rainbow:
                    Primary = new TonalPalette(hue, 48);
                    Secondary = new TonalPalette(hue, 16);
                    Tertiary = new TonalPalette(hue + 60, 24);
                    Neutral = new TonalPalette(hue, 0);
                    NeutralVariant = new TonalPalette(hue, 0);
                    break;
                case ThemeStyle.FruitSalad:
                    Primary = new TonalPalette(hue - 50, 48);
                    Secondary = new TonalPalette(hue - 50, 36);
                    Tertiary = new TonalPalette(hue, 36);
                    Neutral = new TonalPalette(hue, 10);
                    NeutralVariant = new TonalPalette(hue, 16);
                    break;
                case ThemeStyle.Content:
                    Primary = new TonalPalette(hue, chroma);
                    Secondary = new TonalPalette(hue, chroma * 0.33);
                    Tertiary = new TonalPalette(hue, chroma * 0.66);
                    Neutral = new TonalPalette(hue, chroma * 0.0833);
                    NeutralVariant = new TonalPalette(hue, chroma * 0.1666);
                    break;
                case ThemeStyle.Monochromatic:
                    Primary = new TonalPalette(hue, chroma);
                    Secondary = new TonalPalette(hue, chroma * 0);
                    Tertiary = new TonalPalette(hue, chroma * 0);
                    Neutral = new TonalPalette(hue, chroma * 0);
                    NeutralVariant = new TonalPalette(hue, chroma * 0);
                    break;
                case ThemeStyle.Clock:
                    Primary = new TonalPalette(hue, ChromaBound(chroma, 20.0, CHROMA_MAX));
                    Secondary = new TonalPalette(hue, ChromaBound(chroma * 0.85, 17.0, 40.0));
                    Tertiary = new TonalPalette(hue, ChromaBound(chroma + 20, 50.0, CHROMA_MAX));

                    // Not used
                    Neutral = new TonalPalette(hue, 0);
                    NeutralVariant = new TonalPalette(hue, 0);
                    break;
                case ThemeStyle.ClockVibrant:
                    Primary = new TonalPalette(hue, ChromaBound(chroma, 70.0, CHROMA_MAX));
                    Secondary = new TonalPalette(hue + 20, ChromaBound(chroma, 70.0, CHROMA_MAX));
                    Tertiary = new TonalPalette(hue + 60, ChromaBound(chroma, 70.0, CHROMA_MAX));

                    // Not used
                    Neutral = new TonalPalette(hue, 0);
                    NeutralVariant = new TonalPalette(hue, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
            Error = new TonalPalette(25, 84);
        }

        private static double ChromaBound(double chroma, double min, double max) => Math.Min(Math.Max(chroma, min), max);


        private const double CHROMA_MAX_OUT = 130.0;
        private const double CHROMA_MAX = 120.0;
        private const double CHROMA_MIN = 0.0;
    }
}
