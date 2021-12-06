using System.IO;
using System.Text;
using System.Xml;

namespace TodoApp2.Core
{
    public class TextDeserializer
    {
        protected static class XmlElements
        {
            public const string Txt = "txt";
            public const string Bold = "b";
            public const string Italic = "i";
            public const string Underlined = "u";
        }

        protected bool Bold { get; set; }
        protected bool UnderLined { get; set; }
        protected bool Italic { get; set; }


        public string DeserializeText(string rawText)
        {
            StringBuilder sb = new StringBuilder();

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(rawText)))
            {
                // First node is always the XML definition
                if (xmlReader.Read())
                {
                    xmlReader.ReadStartElement(XmlElements.Txt);

                    do
                    {
                        // End element
                        if (xmlReader.NodeType == XmlNodeType.EndElement &&
                            xmlReader.Name == XmlElements.Txt)
                        {
                            return sb.ToString();
                        }

                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Text:
                            case XmlNodeType.Whitespace:
                            {
                                sb.Append(xmlReader.Value);
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
            }

            return sb.ToString();
        }

        protected void ResetFormatState()
        {
            Bold = false;
            Italic = false;
            UnderLined = false;
        }

        protected void ReadFormatEnd(XmlReader xmlReader)
        {
            ReadFormat(xmlReader, false);
        }

        protected void ReadFormatStart(XmlReader xmlReader)
        {
            ReadFormat(xmlReader, true);
        }

        private void ReadFormat(XmlReader xmlReader, bool value)
        {
            switch (xmlReader.Name)
            {
                case XmlElements.Bold:
                    Bold = value;
                    break;
                case XmlElements.Italic:
                    Italic = value;
                    break;
                case XmlElements.Underlined:
                    UnderLined = value;
                    break;
            }
        }

    }
}
