using System.Collections.Generic;

namespace StructureInterfaces
{
    public interface ITree
    {
        int Id
        {
            get;
        }

        ILineItem LineItem { get; }

        IEnumerable<ITree> Children { get; }

        bool Included { get; set; }

        int Level { get; }
        ITree Previous { get; }
    }
}