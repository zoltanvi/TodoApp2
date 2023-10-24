// See: https://github.com/albi005/MaterialColorUtilities
using System.Collections.Generic;

namespace TodoApp2.Material
{
    /// <summary>
    /// A convenience class for retrieving colors that are constant in hue and
    /// chroma, but vary in tone.
    /// </summary>
    public class TonalPalette
    {
        private readonly Dictionary<uint, uint> _cache = new Dictionary<uint, uint>();
        private readonly double _hue;
        private readonly double _chroma;

        /// <summary>Creates tones using the HCT hue and chroma from a color.</summary>
        /// <param name="argb">ARGB representation of a color.</param>
        /// <returns>Tones matching that color's hue and chroma.</returns>
        public static TonalPalette FromInt(uint argb)
        {
            Hct hct = Hct.FromInt(argb);
            return FromHueAndChroma(hct.Hue, hct.Chroma);
        }

        /// <summary>Creates tones from a defined HCT hue and chroma.</summary>
        /// <param name="hue">HCT hue</param>
        /// <param name="chroma">HCT chroma</param>
        /// <returns>Tones matching hue and chroma.</returns>
        public static TonalPalette FromHueAndChroma(double hue, double chroma) => new TonalPalette(hue, chroma);

        public TonalPalette(double hue, double chroma)
        {
            _hue = hue;
            _chroma = chroma;
        }

        /// <summary>Creates an ARGB color with HCT hue and chroma of this TonalPalette instance, and the provided HCT tone.</summary>
        /// <param name="tone">HCT tone, measured from 0 to 100.</param>
        /// <returns>ARGB representation of a color with that tone.</returns>
        public uint Tone(uint tone)
            => _cache.TryGetValue(tone, out uint value)
                ? value
                : _cache[tone] = Hct.From(_hue, _chroma, tone).ToInt();

        /// <summary>Creates an ARGB color with HCT hue and chroma of this TonalPalette instance, and the provided HCT tone.</summary>
        /// <param name="tone">HCT tone, measured from 0 to 100.</param>
        /// <returns>ARGB representation of a color with that tone.</returns>
        public uint this[uint tone] => Tone(tone);
    }
}
