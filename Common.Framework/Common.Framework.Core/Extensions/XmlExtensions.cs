// <copyright file="XmlExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Common.Framework.Core.Extensions
{
    public static class XmlExtensions
    {
        public static string Beautify(XmlDocument xmlDocument)
        {
            var stringBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                xmlDocument.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        public static string GetChildNode(
            this XmlNode xmlNode,
            string childNodeName)
        {
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.Name == childNodeName)
                {
                    return childNode.InnerText;
                }
            }

            return string.Empty;
        }

        public static string Serialize(this object obj)
        {
            var memoryStream = new MemoryStream();
            var xmlWriterSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };
            var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
            var xmlSerializer = new XmlSerializer(obj.GetType());
            var xmlDocument = new XmlDocument();

            try
            {
                var xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);

                xmlSerializer.Serialize(xmlWriter, obj, xmlSerializerNamespaces);
                memoryStream.Position = 0;
                xmlDocument.Load(memoryStream);
                
                return Beautify(xmlDocument);
            }
            catch (XmlException xmlException)
            {
                throw new XmlException(xmlException.Message);
            }
            finally
            {
                memoryStream.Close();
                memoryStream.Dispose();
            }
        }

        public static string ToHtml(
            this string xmlText,
            string xslFile)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlText);

            var xsl = new XmlDocument();
            xsl.Load(xslFile);

            var xslt = xml.Transform(xsl);
            return xslt;
        }

        public static string Transform(
            this XmlDocument xml,
            XmlDocument xsl)
        {
            using (var xslStringReader = new StringReader(xsl.InnerXml))
            {
                using (var xmlStringReader = new StringReader(xml.InnerXml))
                {
                    using (var xslXmlReader = XmlReader.Create(xslStringReader))
                    {
                        using (var xmlXmlReader = XmlReader.Create(xmlStringReader))
                        {
                            var xslt = new XslCompiledTransform();
                            xslt.Load(xslXmlReader);

                            using (var stringWriter = new StringWriter())
                            {
                                using (var xmlWriter = XmlWriter.Create(stringWriter, xslt.OutputSettings))
                                {
                                    xslt.Transform(xmlXmlReader, xmlWriter);
                                    return stringWriter.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}