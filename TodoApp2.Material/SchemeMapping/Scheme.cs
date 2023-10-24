// See: https://github.com/albi005/MaterialColorUtilities
using System;
using System.Collections.Generic;

namespace TodoApp2.Material
{
    /// <summary>
    /// Represents a Material color scheme, a mapping of color roles to colors.
    /// </summary>
    /// <typeparam name="TColor">The type of the named colors.</typeparam>
    public class Scheme<TColor>
    {
        public TColor Primary { get; set; }
        public TColor OnPrimary { get; set; }
        public TColor PrimaryContainer { get; set; }
        public TColor OnPrimaryContainer { get; set; }
        public TColor Secondary { get; set; }
        public TColor OnSecondary { get; set; }
        public TColor SecondaryContainer { get; set; }
        public TColor OnSecondaryContainer { get; set; }
        public TColor Tertiary { get; set; }
        public TColor OnTertiary { get; set; }
        public TColor TertiaryContainer { get; set; }
        public TColor OnTertiaryContainer { get; set; }
        public TColor Error { get; set; }
        public TColor OnError { get; set; }
        public TColor ErrorContainer { get; set; }
        public TColor OnErrorContainer { get; set; }
        public TColor Background { get; set; }
        public TColor OnBackground { get; set; }
        public TColor Surface { get; set; }
        public TColor OnSurface { get; set; }
        public TColor SurfaceVariant { get; set; }
        public TColor OnSurfaceVariant { get; set; }
        public TColor Outline { get; set; }
        public TColor Shadow { get; set; }
        public TColor InverseSurface { get; set; }
        public TColor InverseOnSurface { get; set; }
        public TColor InversePrimary { get; set; }
        public TColor Surface1 { get; set; }
        public TColor Surface2 { get; set; }
        public TColor Surface3 { get; set; }
        public TColor Surface4 { get; set; }
        public TColor Surface5 { get; set; }
        public TColor SurfaceDim { get; set; }
        public TColor SurfaceBright { get; set; }
        public TColor SurfaceContainerLowest { get; set; }
        public TColor SurfaceContainerLow { get; set; }
        public TColor SurfaceContainer { get; set; }
        public TColor SurfaceContainerHigh { get; set; }
        public TColor SurfaceContainerHighest { get; set; }
        public TColor OutlineVariant { get; set; }


        /// <summary>Converts the Scheme into a new one with a different color type</summary>
        public Scheme<TResult> Convert<TResult>(Func<TColor, TResult> convert)
        {
            return Convert<TResult>(convert, new Scheme<TResult>());
        }

        /// <summary>Maps the Scheme's colors onto an existing Scheme object with a different color type</summary>
        public Scheme<TResult> Convert<TResult>(Func<TColor, TResult> convert, Scheme<TResult> result)
        {
            result.Primary = convert(Primary);
            result.OnPrimary = convert(OnPrimary);
            result.PrimaryContainer = convert(PrimaryContainer);
            result.OnPrimaryContainer = convert(OnPrimaryContainer);
            result.Secondary = convert(Secondary);
            result.OnSecondary = convert(OnSecondary);
            result.SecondaryContainer = convert(SecondaryContainer);
            result.OnSecondaryContainer = convert(OnSecondaryContainer);
            result.Tertiary = convert(Tertiary);
            result.OnTertiary = convert(OnTertiary);
            result.TertiaryContainer = convert(TertiaryContainer);
            result.OnTertiaryContainer = convert(OnTertiaryContainer);
            result.Error = convert(Error);
            result.OnError = convert(OnError);
            result.ErrorContainer = convert(ErrorContainer);
            result.OnErrorContainer = convert(OnErrorContainer);
            result.Background = convert(Background);
            result.OnBackground = convert(OnBackground);
            result.Surface = convert(Surface);
            result.OnSurface = convert(OnSurface);
            result.SurfaceVariant = convert(SurfaceVariant);
            result.OnSurfaceVariant = convert(OnSurfaceVariant);
            result.Outline = convert(Outline);
            result.Shadow = convert(Shadow);
            result.InverseSurface = convert(InverseSurface);
            result.InverseOnSurface = convert(InverseOnSurface);
            result.InversePrimary = convert(InversePrimary);
            result.Surface1 = convert(Surface1);
            result.Surface2 = convert(Surface2);
            result.Surface3 = convert(Surface3);
            result.Surface4 = convert(Surface4);
            result.Surface5 = convert(Surface5);
            result.SurfaceDim = convert(SurfaceDim);
            result.SurfaceBright = convert(SurfaceBright);
            result.SurfaceContainerLowest = convert(SurfaceContainerLowest);
            result.SurfaceContainerLow = convert(SurfaceContainerLow);
            result.SurfaceContainer = convert(SurfaceContainer);
            result.SurfaceContainerHigh = convert(SurfaceContainerHigh);
            result.SurfaceContainerHighest = convert(SurfaceContainerHighest);
            result.OutlineVariant = convert(OutlineVariant);
            return result;
        }

        public virtual IEnumerable<KeyValuePair<string, TColor>> Enumerate()
        {
            yield return new KeyValuePair<string, TColor>("Primary", Primary);
            yield return new KeyValuePair<string, TColor>("OnPrimary", OnPrimary);
            yield return new KeyValuePair<string, TColor>("PrimaryContainer", PrimaryContainer);
            yield return new KeyValuePair<string, TColor>("OnPrimaryContainer", OnPrimaryContainer);
            yield return new KeyValuePair<string, TColor>("Secondary", Secondary);
            yield return new KeyValuePair<string, TColor>("OnSecondary", OnSecondary);
            yield return new KeyValuePair<string, TColor>("SecondaryContainer", SecondaryContainer);
            yield return new KeyValuePair<string, TColor>("OnSecondaryContainer", OnSecondaryContainer);
            yield return new KeyValuePair<string, TColor>("Tertiary", Tertiary);
            yield return new KeyValuePair<string, TColor>("OnTertiary", OnTertiary);
            yield return new KeyValuePair<string, TColor>("TertiaryContainer", TertiaryContainer);
            yield return new KeyValuePair<string, TColor>("OnTertiaryContainer", OnTertiaryContainer);
            yield return new KeyValuePair<string, TColor>("Error", Error);
            yield return new KeyValuePair<string, TColor>("OnError", OnError);
            yield return new KeyValuePair<string, TColor>("ErrorContainer", ErrorContainer);
            yield return new KeyValuePair<string, TColor>("OnErrorContainer", OnErrorContainer);
            yield return new KeyValuePair<string, TColor>("Background", Background);
            yield return new KeyValuePair<string, TColor>("OnBackground", OnBackground);
            yield return new KeyValuePair<string, TColor>("Surface", Surface);
            yield return new KeyValuePair<string, TColor>("OnSurface", OnSurface);
            yield return new KeyValuePair<string, TColor>("SurfaceVariant", SurfaceVariant);
            yield return new KeyValuePair<string, TColor>("OnSurfaceVariant", OnSurfaceVariant);
            yield return new KeyValuePair<string, TColor>("Outline", Outline);
            yield return new KeyValuePair<string, TColor>("Shadow", Shadow);
            yield return new KeyValuePair<string, TColor>("InverseSurface", InverseSurface);
            yield return new KeyValuePair<string, TColor>("InverseOnSurface", InverseOnSurface);
            yield return new KeyValuePair<string, TColor>("InversePrimary", InversePrimary);
            yield return new KeyValuePair<string, TColor>("Surface1", Surface1);
            yield return new KeyValuePair<string, TColor>("Surface2", Surface2);
            yield return new KeyValuePair<string, TColor>("Surface3", Surface3);
            yield return new KeyValuePair<string, TColor>("Surface4", Surface4);
            yield return new KeyValuePair<string, TColor>("Surface5", Surface5);
            yield return new KeyValuePair<string, TColor>("SurfaceDim", SurfaceDim);
            yield return new KeyValuePair<string, TColor>("SurfaceBright", SurfaceBright);
            yield return new KeyValuePair<string, TColor>("SurfaceContainerLowest", SurfaceContainerLowest);
            yield return new KeyValuePair<string, TColor>("SurfaceContainerLow", SurfaceContainerLow);
            yield return new KeyValuePair<string, TColor>("SurfaceContainer", SurfaceContainer);
            yield return new KeyValuePair<string, TColor>("SurfaceContainerHigh", SurfaceContainerHigh);
            yield return new KeyValuePair<string, TColor>("SurfaceContainerHighest", SurfaceContainerHighest);
            yield return new KeyValuePair<string, TColor>("OutlineVariant", OutlineVariant);
        }
    }
}
