using System.Collections.Generic;
/**
 * This is the only way tests can talk to the system.
 */
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

        public List<ITree> GetStructure(Structure structure)
        {
            var forrest = jiraSource.GetForrest(structure);

            return forrest;
        }

        public List<ITree> ParseForrest(string forrest)
        {
            return Tree.ParseForrest(forrest);
        }

        public List<Structure> GetAvailableStructures()
        {
            return jiraSource.GetAvailableStructures();
        }
    }

}
