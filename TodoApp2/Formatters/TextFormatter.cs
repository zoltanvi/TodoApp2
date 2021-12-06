using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using TodoApp2.Core;

namespace TodoApp2
{
    public class TextFormatter : TextDeserializer
    {

        public void FormatText(TextBlock textBlock)
        {
            string rawText = textBlock.Text;
            ResetFormatState();

            if (!string.IsNullOrWhiteSpace(rawText))
            {

                // Create the textBlock inline collection from scratch
                textBlock.Inlines.Clear();

                using (XmlReader xmlReader = XmlReader.Create(new StringReader(rawText)))
                {
                    // First node is always the XML definition
                    if (xmlReader.Read())
                    {
                        FormatText(xmlReader, textBlock);
                    }
                }
            }
        }

        private void FormatText(XmlReader xmlReader, TextBlock textBlock)
        {
            xmlReader.ReadStartElement(XmlElements.Txt);

            do
            {
                // End element
                if (xmlReader.NodeType == XmlNodeType.EndElement && 
                    xmlReader.Name == XmlElements.Txt)
                {
                    return;
                }

                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Text:
                    {
                        textBlock.Inlines.Add(GetRun(xmlReader.Value));
                        break;
                    }
                    case XmlNodeType.Whitespace:
                    {
                        textBlock.Inlines.Add(xmlReader.Value);
                        break;
                    }
                    case XmlNodeType.Element:
                    {
                        ReadFormatStart(xmlReader);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        ReadFormatEnd(xmlReader);
                        break;
                    }
                }

            } while (xmlReader.Read());
        }

        private Run GetRun(string value)
        {
            TextDecorationCollection textDecorations = new TextDecorationCollection();

            if (UnderLined)
            {
                textDecorations.Add(TextDecorations.Underline);
            }

            return new Run(value)
            {
                FontWeight = Bold ? FontWeights.Bold : FontWeights.Normal,
                FontStyle = Italic ? FontStyles.Italic : FontStyles.Normal,
                TextDecorations = textDecorations
            };
        }

    }
}
