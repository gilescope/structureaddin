using System.Collections.Generic;
/**
 * This is the only way tests can talk to the system.
 */
using System.Xml.Linq;
using StructureInterfaces;
using StructureSource;

namespace StructureAddinAPI
{
    public class API
    {
        readonly JIRASource jiraSource;

        public API(AddinSettings settings)
        {
            if (settings != null) 
                jiraSource = new JIRASource(settings.JIRAURL,settings.Username, settings.Password);
        }

        public List<ITree> GetStructure(Structure structure, string JQLFilter = null)
        {
            var forrest = jiraSource.GetForrest(structure, JQLFilter);

            return forrest;
        }

        public Forrest BindJIRAsToForrest(Forrest forrest, XElement selectedJiras)
        {
            JIRASource.BindJIRAsToForrest(selectedJiras, forrest);
            MinimiseForrest(forrest);
            return forrest;
        }

        /// <summary>
        /// If a subtree in a structure is selected, but no parent of it is then it should be
        /// pulled up to top level.
        /// </summary>
        /// <param name="forrest"></param>
        private void MinimiseForrest(Forrest forrest)
        {
            var result = new List<ITree>();

            AddSelected(forrest, result);

            //TODO recalc level...?

            forrest.Clear();
            forrest.AddRange(result);
        }

        private void AddSelected(IEnumerable<ITree> children, List<ITree> result)
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

        public Forrest ParseForrest(string forrest)
        {
            return Tree.ParseForrest(forrest);
        }

        public List<Structure> GetAvailableStructures()
        {
            return jiraSource.GetAvailableStructures();
        }
    }
}
