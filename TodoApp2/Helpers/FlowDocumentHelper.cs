using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace TodoApp2.Helpers
{
    /// <summary>
    /// Helper class for <see cref="FlowDocument"/> serialization and deserialization into and from XML.
    /// </summary>
    public static class FlowDocumentHelper
    {
        public static string EmptySerializedDocument { get; } = XamlWriter.Save(new FlowDocument());
        public static IEnumerable<Block> EmptyFlowDocumentBlocks { get; } = new FlowDocument().Blocks;

        public static string SerializeDocument(FlowDocument document)
        {
            int index = 0;
            string result = XamlWriter.Save(document);
            List<string> documentItems = GetDocumentItems(document);
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(result);
            
            foreach (XmlElement xmlElement in xmlDoc.DocumentElement.ChildNodes)
            {
                if (xmlElement != null)
                {
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (IsValidNode(xmlNode))
                        {
                            string item = documentItems[index++];

                            // Fix the serialization bug that comes from XamlWriter.Save()
                            if (xmlNode.InnerText != item)
                            {
                                xmlNode.InnerText = item;
                            }
                        }
                    }
                }
            }

            string fixedResult = xmlDoc.OuterXml;
            return fixedResult;
        }

        public static FlowDocument DeserializeDocument(string xml)
        {
            FlowDocument document = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                if (XamlReader.Load(xmlReader) is FlowDocument flowDocument)
                {
                    document = flowDocument;
                }
            }
            return document;
        }

        public static List<string> GetDocumentItems(FlowDocument document)
        {
            List<string> documentItems = new List<string>();

            foreach (Block block in document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    AddInlines(documentItems, paragraph);
                }
            }

            return documentItems;
        }

        private static void AddInlines(List<string> documentItems, Paragraph paragraph)
        {
            foreach (Inline inline in paragraph.Inlines)
            {
                AddInline(documentItems, inline);
            }
        }

        private static void AddInlines(List<string> documentItems, Span span)
        {
            foreach (Inline inline in span.Inlines)
            {
                AddInline(documentItems, inline);
            }
        }

        private static void AddInline(List<string> documentItems, Inline inline)
        {
            if (inline is Run run && !string.IsNullOrEmpty(run.Text))
            {
                documentItems.Add(run.Text);
            }
            else if (inline is Span span)
            {
                AddInlines(documentItems, span);
            }
        }

        private static bool IsValidNode(XmlNode xmlNode)
        {
            return !string.IsNullOrEmpty(xmlNode.InnerText) &&
                   (xmlNode.Name == "Run" ||
                   xmlNode.Name == "Span" ||
                   xmlNode.Name == "#text");
        }
    }
}
