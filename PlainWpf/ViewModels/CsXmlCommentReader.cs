using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Controls;

namespace PlainWpf.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CsXmlCommentReader
    {
        private string xmlUri;

        /// <summary>
        /// ToDo
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="readElement"></param>
        /// <param name="endElement"></param>
        /// <param name="readAttribute"></param>
        /// <param name="readText"></param>
        public CsXmlCommentReader(string filePath, Action<string> readElement = null, Action<string> endElement = null, Action<string, string> readAttribute = null, Action<string> readText = null)
        {
            xmlUri = filePath;
            this.readElement = readElement;
            this.endElement = endElement;
            this.readAttribute = readAttribute;
            this.readText = readText;
        }

        /// <summary>
        /// ToDo
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>非同期のTaskハンドラ</returns>
        public void Read(string filePath = null)
        {
            xmlUri = filePath ?? xmlUri;
            ReadXml();
        }

        private Action<string> readElement;
        private Action<string> ReadElement { get { return readElement; } }

        private Action<string> endElement;
        private Action<string> EndElement { get { return endElement; } }

        private Action<string, string> readAttribute;
        private Action<string, string> ReadAttribute { get { return readAttribute; } }

        private Action<string> readText;
        private Action<string> ReadText { get { return readText; } }

        /// <summary>
        /// ToDo
        /// </summary>
        /// <returns></returns>
        private void ReadXml()
        {
            XmlTextReader xmlReader;
            try
            {
                xmlReader = new XmlTextReader(xmlUri);
            }
            catch (Exception e)
            {
                throw e;
            }
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        try
                        {
                            ReadElement?.Invoke(xmlReader.Name);
                        }
                        catch
                        {
                            throw new CsXmlCommentReaderException();
                        }
                        bool thereAttribute = xmlReader.MoveToFirstAttribute();
                        while (thereAttribute)
                        {
                            try
                            {
                                ReadAttribute?.Invoke(xmlReader.Name, xmlReader.Value);
                            }
                            catch
                            {
                                throw new CsXmlCommentReaderException();
                            }
                            thereAttribute = xmlReader.MoveToNextAttribute();
                        }
                        break;
                    case XmlNodeType.EndElement:
                        try
                        {
                            EndElement?.Invoke(xmlReader.Name);
                        }
                        catch
                        {
                            throw new CsXmlCommentReaderException();
                        }
                        break;
                    case XmlNodeType.Text:
                        try
                        {
                            ReadText?.Invoke(xmlReader.Value);
                        }
                        catch
                        {
                            throw new CsXmlCommentReaderException();
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CsXmlCommentReaderException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CsXmlCommentReaderException()
            : base("Error : Class CsXmlCommentReader")
        {
        }
    }
}
