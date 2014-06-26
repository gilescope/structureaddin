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
            
            return forrest;
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
