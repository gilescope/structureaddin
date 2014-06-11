using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using Microsoft.Office.Interop.MSProject;
using StructureAddinAPI;

namespace StructureAddIn
{
    /**
     * The addin is capable of rendering a model into a MPP file.
     * 
     * Responsibilities:
     *   * Only this dll should talk to MS Project.
     */
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            var ribbon = new StructureAddinRibbon();
            ribbon.OnImport += ribbon_OnImport;
            return ribbon;
        }

        void ribbon_OnImport(StructureInterfaces.AddinSettings settings)
        {
            var api = new API(settings);

            //TODO can improve: push to UI as combo
            var selectedStructure = api.GetAvailableStructures().First(s=>s.Name.Equals(settings.Structure));
            var forrest = api.GetStructure(selectedStructure);

            var project = Application.ActiveProject;
            //                projects = projectApp.Projects;
            //                project = projects.Add(false, null, false);
            //              project.SaveAs("MyProj");
            //project.Activate();
            
            new Renderer().Render(project, forrest);
        }

        public static void StartProject()
        {
            Application projectApp = null;
            Projects projects = null;
            Project project = null;

            try {
            //if ms prject not started, start it.
                projectApp = new Application();
                projectApp.FileNew(null, null, false, true);
                project = projectApp.ActiveProject;
//                projects = projectApp.Projects;
//                project = projects.Add(false, null, false);
  //              project.SaveAs("MyProj");
                //project.Activate();
                
                Task newtask = project.Tasks.Add("Friday");

                // Enumerate the tasks
                foreach (Task task in project.Tasks)
                {
                    string name = task.Name;

                    // Project stores the number of minutes in a workday, so 8 hours per workday * 60 = 480. 480 is a project "day"
                    int duration_in_days = Int32.Parse(task.Duration.ToString()) / 480;

                    DateTime start = DateTime.Parse(task.Start.ToString());
                    DateTime finish = DateTime.Parse(task.Finish.ToString());
                    double percent_complete = Double.Parse(task.PercentComplete.ToString());
                    //DateTime actual_finish = DateTime.Parse(task.ActualFinish.ToString());

                    // Do something with each task here
                }

                projectApp.Quit(PjSaveType.pjDoNotSave);
            }
            finally
            {
                if (project != null) Marshal.ReleaseComObject(project);
                if (projects != null) Marshal.ReleaseComObject(projects);
                if (projectApp != null) Marshal.ReleaseComObject(projectApp);
            }
        }
    }
}
