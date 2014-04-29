/* Atomic Autobiography App: by Artun Kircali, 4/29/14
 * ---------------------------------------------------
 * Program Class:
 * Contains Main()
 * Runs my application.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atomic_Object_Job_Application
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new page1());
        }
    } 
}
