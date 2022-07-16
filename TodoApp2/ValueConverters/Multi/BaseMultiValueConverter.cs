using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TodoApp2
{
    /// <summary>
    /// A base value converter that allows direct XAML usage
    /// </summary>
    /// <typeparam name="T">The type of this value converter</typeparam>
    public abstract class BaseMultiValueConverter<T> : MarkupExtension, IMultiValueConverter
    where T : class, new()
    {
        /// <summary>
        /// A single static instance of this value converter
        /// </summary>
        private static T s_Converter;

        /// <summary>
        /// Provides a static instance of the value converter
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return s_Converter ?? (s_Converter = new T());
        }

        /// <summary>
        /// The method to convert multiple values to one
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}