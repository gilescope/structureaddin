using System;

namespace StructureInterfaces
{
    public interface ILineItem
    {
        double? OriginalEstimateInDays { get; }

        string Assignee { get; }

        string Resolution { get; }

        DateTime? DueDate { get; }

        string Summary { get; }

        string Key { get;  }
    }
}