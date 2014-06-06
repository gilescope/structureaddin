using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.MSProject;
using StructureInterfaces;

namespace StructureAddIn
{
    /**
     * Responsible for creating and styling the project tasks.
     */
    internal class Renderer
    {
        public void Render(Project project, List<ITree> forrest)
        {
            project.Title = forrest[0].LineItem.Summary;

            foreach (var tree in forrest)
            {
                PreOrderVisitor(tree, node =>
                {                    
                    Task newtask = project.Tasks.Add(node.LineItem.Summary);
                    newtask.OutlineLevel = (short)(node.Level + 1);
                    
                    if (node.LineItem.Resolution != null)
                        newtask.PercentComplete = "100%";
                    
                    if (node.LineItem.Assignee != null)
                        newtask.ResourceNames = node.LineItem.Assignee;

                    if (node.LineItem.OriginalEstimateInDays != null)
                        newtask.Duration = node.LineItem.OriginalEstimateInDays;

                    if (node.LineItem.DueDate != null)
                        newtask.Finish = node.LineItem.DueDate;
                });
            }

            project.Application.FilterApply("Incomplete Tasks");
            project.Application.LevelNow(true);
            project.Application.OutlineIndent();
        }

        private void PreOrderVisitor(ITree tree, Action<ITree> visit)
        {
            if (!tree.Included) 
                return;
            
            visit(tree);

            foreach (var child in tree.Children)
            {
                PreOrderVisitor(child, visit);
            }
        }
    }
}