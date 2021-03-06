﻿using System;
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

        public Forrest GetAndParseForrest(Structure structure)
        {
            //Get forrest
            string url = baseURL + "rest/structure/1.0/structure/" +
                         structure.Id.ToString(CultureInfo.InvariantCulture).Trim() + "/forest";
            Stream results = GetURL(url, user, pass);
            XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(results, new XmlDictionaryReaderQuotas());

            //Parse forrest
            XElement root = XElement.Load(reader);
            string forrest = root.XPathSelectElement("//formula").Value;
            return Tree.ParseForrest(forrest);
        }

        public Forrest GetForrest(Structure structure, string JQLFilter)
        {
            Forrest result = GetAndParseForrest(structure);

            //Now get JIRAS
            XElement selectedJiras = GetJIRAs(structure, JQLFilter);
            BindJIRAsToForrest(selectedJiras, result);

            return result;
        }

        private XElement GetJIRAs(Structure structure, string JQLFilter)
        {
            string jql = "issue in structure(\"" + structure.Name + "\")";
            if (!string.IsNullOrWhiteSpace(JQLFilter))
            {
                jql = JQLFilter;
            }

            string url = baseURL + "rest/api/2/search?jql=" + HttpUtility.UrlEncode(jql) + "&maxResults=5000";
            var results = GetURL(url, user, pass);
            var reader = JsonReaderWriterFactory.CreateJsonReader(results, new XmlDictionaryReaderQuotas());
            var root = XElement.Load(reader);
            return root;
        }

        public static void BindJIRAsToForrest(XElement root, Forrest forrest)
        {
            var jiras = root.XPathSelectElements("//issues//item");
            foreach (var xJIRA in jiras)
            {
                int jiraId = Convert.ToInt32(xJIRA.XPathSelectElement("id").Value);
                var tree = (Tree) forrest.FindById(jiraId);
                tree.Included = true;
                tree.LineItem = new LineItem(xJIRA);
            }

            MinimiseForrest(forrest);
        }

        /// <summary>
        /// If a subtree in a structure is selected, but no parent of it is then it should be
        /// pulled up to top level.
        /// </summary>
        /// <param name="forrest"></param>
       static private void MinimiseForrest(Forrest forrest)
        {
            var result = new List<ITree>();

            AddSelected(forrest, result);

            forrest.Clear();
            forrest.AddRange(result);

            foreach (var tree in forrest.Children)
            {
                ResetLevel(tree, 0);
            }
       }

        static private void ResetLevel(ITree forrest, int level)
        {
            ((Tree)forrest).Level = level;
            foreach (var child in forrest.Children)
            {
                ResetLevel(child, level + 1);
            }
        }

        static private void AddSelected(IEnumerable<ITree> children, List<ITree> result)
        {
            foreach (var child in children)
            {
                if (child.Included)
                {
                    result.Add(child);
                }
                else
                {
                    AddSelected(child.Children, result);
                }
            }
        }

        private static void PruneTreesWithNothingINcluded(Forrest forrest)
        {
            var toRemove = new List<ITree>();

            foreach (var tree in forrest)
            {
                if (!tree.AnyIncluded)
                {
                    toRemove.Add(tree);
                    continue;
                }
            }

            toRemove.ForEach(tree => forrest.Remove(tree));
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
