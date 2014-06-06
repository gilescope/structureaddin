/*
 * This DLL should not implement anything and should not depend on anything.
 */

namespace StructureInterfaces
{
    public class Structure
    {
        public Structure(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
    }
}
