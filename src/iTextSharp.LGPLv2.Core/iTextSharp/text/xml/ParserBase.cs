using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace iTextSharp.text.xml
{
    /// <summary>
    /// The  ParserBase -class provides XML document parsing.
    /// </summary>
    public abstract class ParserBase
    {
        /// <summary>
        /// This method gets called when characters are encountered.
        /// </summary>
        /// <param name="content">an array of characters</param>
        /// <param name="start">the start position in the array</param>
        /// <param name="length">the number of characters to read from the array</param>
        public abstract void Characters(string content, int start, int length);

        /// <summary>
        /// This method gets called when an end tag is encountered.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="lname"></param>
        /// <param name="name">the name of the tag that ends</param>
        public abstract void EndElement(string uri, string lname, string name);

        public void Parse(XmlDocument xDoc)
        {
            string xml = xDoc.OuterXml;
            StringReader stringReader = new StringReader(xml);

            XmlReader reader = XmlReader.Create(stringReader);
            Parse(reader);
        }

        public void Parse(XmlReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            string namespaceUri = reader.NamespaceURI;
                            string name = reader.Name;
                            bool isEmpty = reader.IsEmptyElement;
                            Hashtable attributes = new Hashtable();
                            if (reader.HasAttributes)
                            {
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    attributes.Add(reader.Name, reader.Value);
                                }
                            }
                            StartElement(namespaceUri, name, name, attributes);
                            if (isEmpty)
                            {
                                EndElement(namespaceUri,
                                    name, name);
                            }
                            break;
                        case XmlNodeType.EndElement:
                            EndElement(reader.NamespaceURI,
                                reader.Name, reader.Name);
                            break;
                        case XmlNodeType.Text:
                            Characters(reader.Value, 0, reader.Value.Length);
                            break;
                        // There are many other types of nodes, but
                        // we are not interested in them
                        case XmlNodeType.Whitespace:
                            Characters(reader.Value, 0, reader.Value.Length);
                            break;
                    }
                }
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
        }

        /// <summary>
        /// Begins the process of processing an XML document
        /// </summary>
        /// <param name="url">the XML document to parse</param>
        public void Parse(string url)
        {
            var stringReader = new StringReader(File.ReadAllText(url));
            var reader = XmlReader.Create(stringReader);
            Parse(reader);
        }

        /// <summary>
        /// This method gets called when a start tag is encountered.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="lname"></param>
        /// <param name="name">the name of the tag that is encountered</param>
        /// <param name="attrs">the list of attributes</param>
        public abstract void StartElement(string uri, string lname, string name, Hashtable attrs);
    }
}
