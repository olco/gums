using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gums
{
    class GumsNotifyIcon
    {
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;


        private GumsSurvey gs;

        private GumsForm gf;

        public GumsNotifyIcon(GumsSurvey _gs)
        {

            gs = _gs;

            gf = new GumsForm();

            contextMenu1 = new System.Windows.Forms.ContextMenu();
            menuItem1 = new System.Windows.Forms.MenuItem();
            menuItem2 = new System.Windows.Forms.MenuItem();
            menuItem3 = new System.Windows.Forms.MenuItem();
            menuItem4 = new System.Windows.Forms.MenuItem();
            menuItem5 = new System.Windows.Forms.MenuItem();
            notifyIcon1 = new System.Windows.Forms.NotifyIcon();

            // Initialize contextMenu1
            contextMenu1.MenuItems.AddRange(
                 new System.Windows.Forms.MenuItem[] { menuItem1, menuItem2, menuItem3, menuItem4, menuItem5
            });

            
            // Initialize menuItem1
            menuItem1.Index = 4;
            menuItem1.Text = "E&xit Gums";
            menuItem1.Click += new System.EventHandler(menuItem1_Click);

            // Initialize menuItem2
            menuItem2.Index = 3;
            menuItem2.Text = "Show logs";
            menuItem2.Click += new System.EventHandler(menuItem2_Click);

            // Initialize menuItem3
            menuItem3.Index = 2;
            menuItem3.Text = "Pause survey";
            menuItem3.Click += new System.EventHandler(menuItem3_Click);

            // Initialize menuItem4
            menuItem4.Index = 1;
            menuItem4.Text = "Continue survey";
            menuItem4.Enabled = false;
            menuItem4.Click += new System.EventHandler(menuItem4_Click);

            // Initialize menuItem5
            menuItem5.Index = 0;
            menuItem5.Text = "Open details";
            menuItem5.Click += new System.EventHandler(menuItem5_Click);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            notifyIcon1.Icon = new System.Drawing.Icon("gums.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon1.ContextMenu = contextMenu1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "Gums";
            notifyIcon1.Visible = true;

        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            notifyIcon1.Visible = false;
            notifyIcon1 = null;
            Application.Exit();
        }

        private void menuItem2_Click(object Sender, EventArgs e)
        {
            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gumsPath = pathDesktop + "\\Gums";
            System.Diagnostics.Process.Start("explorer.exe", gumsPath);
        }

        private void menuItem3_Click(object Sender, EventArgs e)
        {
            menuItem3.Enabled = false;
            menuItem4.Enabled = true;
            gs.pauseSurvey();
        }

        private void menuItem4_Click(object Sender, EventArgs e)
        {
            menuItem3.Enabled = true;
            menuItem4.Enabled = false;
            gs.continueSurvey();

        }

        private void menuItem5_Click(object Sender, EventArgs e)
        {
            try
            {
                gf.Show();

            }
            catch (Exception ex)
            {

            }

        }

        //

    }
}
