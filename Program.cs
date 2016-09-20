using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gums
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GumsSurvey gs = new GumsSurvey();
            gs.startSurvey();

            GumsNotifyIcon gni = new GumsNotifyIcon(gs);

            Properties.Settings.Default.Save();
                

            Application.Run();
 
        }


    }
}
