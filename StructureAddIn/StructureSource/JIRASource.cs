using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using StructureInterfaces;

namespace StructureSource
{
    public class Structures : List<Structure>
    {
        public Structures(XElement root)
        {
            foreach (var xstructure in root.XPathSelectElements("//structures/item"))
            {
                int id = Convert.ToInt32(xstructure.XPathSelectElement("./id").Value);
                string name = xstructure.XPathSelectElement("./name").Value;
                Add(new Structure(id, name));
            }            
        }
    }

    /**
     * This dll will provide the information about a structure and it's associated jiras.
     * 
     * Responsible for:
     *  * Not letting any other application 'see' JIRA.
     */
    public class JIRASource
    {
        private readonly string baseURL;
        private readonly string user;
        private readonly SecureString pass;

        public JIRASource(string baseURL, string user, SecureString pass)
        {
            this.baseURL = baseURL;
            this.user = user;
            this.pass = pass;
        }

        public Structures GetAvailableStructures()
        {
            string structureInfo = baseURL + "rest/structure/1.0/structure/";
            Stream results = GetURL(structureInfo, user, pass);
            XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(results, new XmlDictionaryReaderQuotas());
            XElement root = XElement.Load(reader);

            return new Structures(root);
        }

        public List<ITree> GetForrest(Structure structure)
        {
            //Get forrest
            string url = baseURL + "rest/structure/1.0/structure/" + structure.Id.ToString(CultureInfo.InvariantCulture).Trim() + "/forest";
            Stream results= GetURL(url, user, pass);
            XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(results, new XmlDictionaryReaderQuotas());

            //Parse forrest
            XElement root = XElement.Load(reader);
            string forrest = root.XPathSelectElement("//formula").Value;
            Forrest result = Tree.ParseForrest(forrest);

            //Now get JIRAS
            string jql = "issue in structure(\""+ structure.Name +"\")";
            url = baseURL + "rest/api/2/search?jql="+HttpUtility.UrlEncode(jql)+"&maxResults=5000";
            results = GetURL(url, user, pass);
            reader = JsonReaderWriterFactory.CreateJsonReader(results, new XmlDictionaryReaderQuotas());
            root = XElement.Load(reader);

            var jiras = root.XPathSelectElements("//issues//item");
            foreach (var xJIRA in jiras)
            {
                int jiraId = Convert.ToInt32(xJIRA.XPathSelectElement("id").Value);
                var tree = (Tree)result.FindById(jiraId);
                tree.LineItem = new LineItem (xJIRA);
            }

            return result;
        }

        private Stream GetURL(string url, string user, SecureString pass)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "GET";

            string base64Credentials = GetEncodedCredentials(user, pass);
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                return response.GetResponseStream();
            }
            catch// (WebException webEx)
            {
//                if ((webEx.Response.Headers["CODE"]) == "503")
//                {
//                    //MessageBox.Show("JIRA server unavailable.")
//                    return null;
//                }
                throw;
            }
        }

        private string GetEncodedCredentials(string m_Username, SecureString m_Password)
        {
            string mergedCredentials = string.Format("{0}:{1}", m_Username, SecureHelper.ToString(m_Password));
            byte[] byteCredentials = Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
    }
}
