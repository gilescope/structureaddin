using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using StructureInterfaces;

namespace StructureSource
{
    public class Forrest : List<ITree>
    {
        readonly Dictionary<int, Tree> nodeLookup = new Dictionary<int, Tree>();

        public void Add(int id, Tree tree)
        {
            nodeLookup.Add(id, tree);    
        }

        public ITree FindById(int id)
        {
            return nodeLookup[id];
        }        
    }

    public class Tree : ITree
    {
        readonly List<ITree> children = new List<ITree>();
        
        private Tree(Forrest forrest, ITree previous, Entry entry)
        {
            Id = entry.Id;
            Level = entry.Level;
            Children = children;
            Previous = previous;
            Included = true;

            forrest.Add(entry.Id, this);
        }

        public int Id { get; private set; }

        public ILineItem LineItem { get; set; }

        public IEnumerable<ITree> Children { get; private set; }

        public bool Included { get; set; }

        private class Entry
        {
            public Entry(string entry)
            {
                string[] nodeBits = entry.Split(':');
                Id = Convert.ToInt32(nodeBits[0]);
                Level = Convert.ToInt32(nodeBits[1]);
            }

            internal int Level { get; private set; }
            internal int Id { get; private set; }
        }

        public static Forrest ParseForrest(string forrestString)
        {
            var forrest = new Forrest();
            string[] nodes = forrestString.Split(',');

            var nodeStack = new Stack<Tree>();
            
            for (int i = 0; i < nodes.Length; i++)
            {
                var entry = new Entry(nodes[i]);
                if (nodeStack.Count == 0)
                {
                    Debug.Assert(entry.Level == 0);

                    var newRoot = new Tree(forrest, null, entry);
                    nodeStack.Push(newRoot);
                    forrest.Add(newRoot);
                }
                else
                {
                    if (entry.Level > nodeStack.Peek().Level)
                    {
                        var child = nodeStack.Peek().AddChild(forrest, entry);
                        nodeStack.Push(child);
                        continue;
                    }

                    while (nodeStack.Peek().Level > entry.Level)
                    {
                        nodeStack.Pop();
                    }
                    var top = nodeStack.Pop();
                    if (nodeStack.Count > 0)
                    {
                        nodeStack.Peek().AddChild(forrest, entry);
                        nodeStack.Push(top);
                    }
                    else
                    {
                        Debug.Assert(entry.Level == 0);
                        var newRoot = new Tree(forrest, forrest.Last(), entry);
                        forrest.Add(newRoot);
                        nodeStack.Push(newRoot);
                    }
                }
            }
            return forrest;
        }

        private Tree AddChild(Forrest forrest, Entry entry)
        {
            Debug.Assert(entry.Level == Level + 1);
            var tree = new Tree(forrest, null, entry);
            children.Add(tree);
            return tree;
        }

        public int Level { get; private set; }
        
        public ITree Previous { get; private set; }
    }

    public class LineItem: ILineItem
    {
        public LineItem(XElement xJIRA)
        {
            Key = TryGet(xJIRA,"key");
            Summary = TryGet(xJIRA, "fields/summary");
            Resolution = TryGet(xJIRA,"fields/resolution/name");
            DueDate = TryGetAsDate(xJIRA, "fields/duedate");
            Assignee = TryGet(xJIRA, "fields/assignee/name");
            OriginalEstimateInDays = TryGetAsDouble(xJIRA, "fields/timeoriginalestimate");
        }

        private string TryGet(XElement xJIRA, string xpath)
        {
            var selectedElement = xJIRA.XPathSelectElement(xpath);
            return selectedElement == null ? null : selectedElement.Value;
        }

        private double? TryGetAsDouble(XElement xJIRA, string xpath)
        {
            var dateString = TryGet(xJIRA, xpath);
            if (dateString == null)
                return null;

            double result;
            if (Double.TryParse(dateString, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result / 60 ;
            
            return null;
        }

        private DateTime? TryGetAsDate(XElement xJIRA, string xpath)
        {
            var dateString = TryGet(xJIRA, xpath);
            if (dateString == null)
                return null;

            //2014-05-22
            DateTime result;
            if (!DateTime.TryParseExact(
                    dateString, 
                    "yyyy-MM-dd", 
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal,
                    out result))
            {
                return null;
            }

            return result;
        }

        public double? OriginalEstimateInDays { get; private set; }

        public string Assignee { get; private set; }

        public string Resolution { get; private set; }

        public DateTime? DueDate { get; private set; }

        public string Summary { get; private set; }

        public string Key { get; private set; }
    }
}