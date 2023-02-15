using System;
using System.Xml;
using System.Xml.XPath;
using Utility.Core.Logging;
using Utility.Toolkit.Diagnostic;

namespace Utility.Toolkit.Xml
{
    public class XmlHelper
    {
        #region Private Structs

        struct XmlNamespace
        {
            public string nsPrefix;
            public string nsValue;
        }

        #endregion

        #region Public Methods

        #region XmlConstruction
        public static XmlDocument ConstructXmlDocument(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return doc;
        }

        public static XmlDocument LoadXmlDocument(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            return doc;
        }
        #endregion

        #region NamespaceManager Load with Namespace

        public static XmlNamespaceManager LoadNamespaceManager(XmlDocument document, string defaultNamespaceName, string defaultNamespaceValue)
        {
            Trace.WriteLine("Entered method (Default namespace method)");

            XmlNamespaceManager nsManager = null;
            try
            {
                nsManager = new XmlNamespaceManager(document.NameTable);
                Trace.WriteLine("Successfully created new XmlNamespaceManager");

                nsManager.AddNamespace(defaultNamespaceName, defaultNamespaceValue);
                Trace.WriteLine("Added default namespace to XmlNamespaceManager");
            }
            catch (XmlException xmlException)
            {
                throw new XmlException("XmlException raised while loading XmlNamespaceManager - error message: "
                    + xmlException.Message);
            }
            catch (Exception generalException)
            {
                throw new Exception("General exception raised while loading XmlNamespaceManager - error message: "
                    + generalException.Message);
            }
            finally
            {
                FileTrace.WriteMemberExit();
            }

            return nsManager;

        }

        public static XmlNamespaceManager LoadNamespaceManager(XmlDocument document, string[] nsNames, string[] nsValues)
        {
            Trace.WriteLine("Entered method (Multiple namespace method) ");

            XmlNamespaceManager nsManager = null;
            try
            {
                nsManager = new XmlNamespaceManager(document.NameTable);
                Trace.WriteLine("Successfully created new XmlNamespaceManager");

                int i = 0;
                foreach (string nsName in nsNames)
                {
                    Trace.WriteLine("Adding prefix " + nsName);
                    Trace.WriteLine("Prefix has namespace " + nsValues[i]);

                    nsManager.AddNamespace(nsName, nsValues[i]);
                    Trace.WriteLine("Successfully added namespace: " + nsValues + " with prefix: " + nsName);

                    i += 1;
                }
            }
            catch (XmlException xmlException)
            {
                throw new XmlException("XmlException raised while loading XmlNamespaceManager - error message: "
                    + xmlException.Message);
            }
            catch (Exception generalException)
            {
                throw new Exception("General exception raised while loading XmlNamespaceManager - error message: "
                    + generalException.Message);
            }
            finally
            {
                FileTrace.WriteMemberExit();
            }

            return nsManager;

        }

        public static XmlNamespaceManager LoadNamespaceManager(XmlDocument document, string nsValues)
        {
            Trace.WriteLine("Entered method (Multiple namespace method) ");

            XmlNamespaceManager nsManager;
            try
            {
                nsManager = new XmlNamespaceManager(document.NameTable);
                Trace.WriteLine("Successfully created new XmlNamespaceManager");

                //nsManager.AddNamespace(String.Empty, nsValues);				
            }
            catch (Exception generalException)
            {
                Trace.WriteException(generalException);
                throw;
            }

            return nsManager;

        }


        #endregion

        #region XPath Helper Functions

        public static XmlNode SelectSingleNode(XmlDocument document, string xPath)
        {
            FileTrace.WriteMemberEntry();

            XmlNamespace xmlNamespace = GetDefaultNamespace(document);
            XmlNamespaceManager nsManager = LoadNamespaceManager(document, xmlNamespace.nsPrefix, xmlNamespace.nsValue);
            XmlNode retVal = document.SelectSingleNode(xPath, nsManager);

            FileTrace.WriteMemberExit();
            return retVal;

        }

        public static XmlNode SelectSingleNode(XmlNode node, string xPath)
        {
            FileTrace.WriteMemberEntry();

            XmlNamespace xmlNamespace = GetDefaultNamespace(node.OwnerDocument);
            XmlNamespaceManager nsManager = LoadNamespaceManager(node.OwnerDocument, xmlNamespace.nsPrefix, xmlNamespace.nsValue);
            XmlNode retVal = node.SelectSingleNode(xPath, nsManager);

            FileTrace.WriteMemberExit();
            return retVal;
        }

        public static XmlNodeList SelectNodes(XmlDocument document, string xPath)
        {
            FileTrace.WriteMemberEntry();

            XmlNodeList nodes = null;
            try
            {
                Trace.WriteLine("Attempting to retrieve default and add namespace");
                XmlNamespace xmlNamespace = GetDefaultNamespace(document);
                XmlNamespaceManager nsManager = LoadNamespaceManager(document, xmlNamespace.nsPrefix, xmlNamespace.nsValue);

                Trace.WriteLine("Attempting to execute XPath: " + xPath);
                nodes = document.SelectNodes(xPath, nsManager);

                Trace.WriteLine("XPath returned " + nodes.Count + " nodes");
            }
            catch (XmlException xmlException)
            {
                throw new XmlException("XmlException raised while executing XPath - error message: "
                    + xmlException.Message);
            }
            catch (Exception generalException)
            {
                throw new Exception("General exception raised while executing XPath - error message: "
                    + generalException.Message);
            }

            FileTrace.WriteMemberExit();
            return nodes;

        }

        public static XmlNodeList SelectNodes(XmlNode node, string xPath)
        {
            FileTrace.WriteMemberEntry();

            XmlNodeList nodes = null;
            try
            {
                Trace.WriteLine("Attempting to retrieve default and add namespace");
                XmlNamespace xmlNamespace = GetDefaultNamespace(node.OwnerDocument);
                XmlNamespaceManager nsManager = LoadNamespaceManager(node.OwnerDocument, xmlNamespace.nsPrefix, xmlNamespace.nsValue);

                Trace.WriteLine("Attempting to execute XPath: " + xPath);
                nodes = node.SelectNodes(xPath, nsManager);

                Trace.WriteLine("XPath returned " + nodes.Count + " nodes");
            }
            catch (XmlException xmlException)
            {
                throw new XmlException("XmlException raised while executing XPath - error message: "
                    + xmlException.Message);
            }
            catch (Exception generalException)
            {
                throw new Exception("General exception raised while executing XPath - error message: "
                    + generalException.Message);
            }

            FileTrace.WriteMemberExit();
            return nodes;
        }

        #endregion

        #region Attributes Helper Functions
        public static XmlAttribute GetAttribute(XmlNode node, string attributeName)
        {
            XmlAttribute attribute = node.Attributes[attributeName];

            return attribute;
        }

        public static object GetAttributeValue(XmlNode node, string attributeName)
        {
            XmlAttribute attribute = GetAttribute(node, attributeName);

            if (attribute != null)
            {
                return attribute.Value;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #endregion

        #region Private Methods

        #region Namespace Retrieve Helper Functions

        private static XmlNamespace GetDefaultNamespace(XmlDocument document)
        {
            FileTrace.WriteMemberEntry();

            XmlNamespace xmlNamespace = new XmlNamespace();

            // Initialise struct with some values
            xmlNamespace.nsPrefix = string.Empty;
            xmlNamespace.nsValue = string.Empty;

            try
            {
                XmlElement rootElement = document.DocumentElement;
                Trace.WriteLine("Successfully reached root of document");

                XPathNavigator nav = rootElement.CreateNavigator();
                if (nav.MoveToFirstNamespace())
                {
                    Trace.WriteLine("Namespace found - Prefix: " + nav.Name + " Namespace: " + nav.Value);

                    xmlNamespace.nsPrefix = nav.Name;
                    xmlNamespace.nsValue = nav.Value;
                }

                Trace.WriteLine("Successfully added namespace details to return var");
            }
            catch (XmlException xmlException)
            {
                throw new XmlException("XmlException raised while searching default namespace - error message: "
                    + xmlException.Message);
            }
            catch (Exception generalException)
            {
                throw new Exception("General exception raised while searching default namespace - error message: "
                    + generalException.Message);
            }

            FileTrace.WriteMemberExit();
            return xmlNamespace;

        }

        #endregion

        #endregion

    }
}