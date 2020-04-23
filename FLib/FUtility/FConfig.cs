using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FLib.FUtility
{
    /// <summary>
    /// Standard class to save e retrive the app configurations
    /// </summary>
    public class FConfig
    {
        const string BASE_CONFIG_FILE = "config.xml";
        const string ROOT_ELEMENT = "settings";
        private string _configFileName = "";

        public string FileName
        {
            get
            {
                if (_configFileName == "")
                    return System.AppContext.BaseDirectory + "\\" + BASE_CONFIG_FILE;
                else return System.AppContext.BaseDirectory + "\\" + _configFileName;
            }
            set
            {
                _configFileName = value;
            }
        }

        public bool saveParam(string name, string value)
        {
            bool result = false;
            XmlWriter writer = null;
            XDocument xDocument = null;
           // Create an XmlWriterSettings object with the correct options. 
           XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = @"\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;

            if (!File.Exists(FileName))
            {
                try
                {
                    // Create the XmlWriter object and write some content.
                    writer = XmlWriter.Create(FileName, settings);
                    writer.WriteStartDocument();
                    writer.WriteStartElement(ROOT_ELEMENT);
                    writer.WriteElementString(name, value);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    result = true;
                }
                
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
            else
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(FileName);
                    XmlNode configNode = doc.SelectSingleNode(ROOT_ELEMENT);
                    XmlNode currentNode = configNode.SelectSingleNode(name);
                    if (currentNode == null) {
                        XmlElement elem = doc.CreateElement(name);
                        elem.InnerText = value;
                        configNode.AppendChild(elem);
                    }
                    else {
                        
                        currentNode.InnerText = value;
                        
                    }

                    doc.Save(FileName);
                    result = true;

                }
                
                finally
                {
                }
            }
            return result;
        }

        public bool getParam(string name,out string value)
        {
            XmlReader reader = null;
            string xmlContent = "";
            bool result = false;

            try
            {

                // Create an XmlWriterSettings object with the correct options. 
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                
                // Create the XmlWriter object and write some content.
                reader = XmlReader.Create(FileName, settings);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == name)
                        {
                            xmlContent =  reader.ReadString().ToString();
                            result = true;
                        }
                    }
                }
            }catch (Exception ex) { }
            finally
            {
                if (reader != null) 
                    reader.Close();
            }




            value = xmlContent;
            return result;

        }

    }


    

}
