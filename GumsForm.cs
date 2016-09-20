using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gums
{
    public partial class GumsForm : Form
    {

        public GumsForm()
        {


            InitializeComponent();
            ReadCSV(getLogsName());

            

            //dataGridView1.DataSource = dt;

        }

        private string getLogsName()
        {
            DateTime thisDay = DateTime.Today;
            String currentDateStr = monthCalendar1.SelectionRange.Start.ToString("yyyyMMdd");

            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String gumsPath = pathDesktop + "\\Gums";

            if (Directory.Exists(gumsPath))
            {
                // On affiche les dates en gras
                //monthCalendar1.RemoveAllBoldedDates();
                DirectoryInfo diTop = new DirectoryInfo(gumsPath);
                foreach (var fi in diTop.EnumerateFiles())
                {
                    string[] nameDetails = fi.Name.Split('_');
                    if (nameDetails.Length == 3)
                    {
                        String dateStr = nameDetails[1];
                        DateTime dateC = DateTime.ParseExact(dateStr, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                        monthCalendar1.AddBoldedDate(dateC);
                    }
                }


                Console.WriteLine("---> Directory.Exists : " + gumsPath);
                String path = gumsPath + "\\whatDidYouDo_" + currentDateStr + "_duration.csv";
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return "";
        }

        private void ReadCSV(string FileName)
        {

            try
            {

                if (FileName == "")
                {
                    return;
                }

                dataGridView1.Rows.Clear();

                //no try/catch - add these in yourselfs or let exception happen
                String[] csvData = File.ReadAllLines(FileName);


                //populate the DataTable
                for (int i = 0; i < csvData.Length; i++)
                {


                    String[] row= csvData[i].Split(';');

                    if (row.Length == 7) {
                        dataGridView1.Rows.Add(row);
                    }

                }
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
            }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            Console.WriteLine("---> Selection Changed");

            toolStripStatusLabel1.Text = "";

            int durationTotal = 0;

            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
            {
                String durationStr = dataGridView1.SelectedRows[i].Cells[0].Value.ToString();
                String[] durationDetails = durationStr.Split(':');
                if (durationDetails.Length == 3)
                {
                    int heureC = Int32.Parse(durationDetails[0]);
                    int minuteC = Int32.Parse(durationDetails[1]);
                    int secondeC = Int32.Parse(durationDetails[2]);
                    int durationC = heureC * 3600 + minuteC * 60 + secondeC;

                    durationTotal = durationTotal + durationC;
                }
                Console.WriteLine("---> Selected : " + durationTotal + " " + durationStr);
            } 

            int heure = 0;
            int min = 0;
            int sec = 0;

            if (durationTotal < 60)
            {
                sec = durationTotal;
            }
            else {
                if (durationTotal < 3600)
                {
                    min = durationTotal / 60;
                    sec = durationTotal % 60;
                }
                else {
                    heure = durationTotal / 3600;
                    int secRestantes = durationTotal % 3600;
                    min = secRestantes / 60;
                    sec = secRestantes % 60;
                }
            }
            String heureStr = (heure < 10) ? "0" + heure : "" + heure;
            String minStr = (min < 10) ? "0" + min : "" + min;
            String secStr = (sec < 10) ? "0" + sec : "" + sec;

            String durationTotalStr = heureStr + ":" + minStr + ":" + secStr;

            Console.WriteLine(durationTotalStr);
            toolStripStatusLabel1.Text = durationTotalStr;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            ReadCSV(getLogsName());
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

            ReadCSV(getLogsName());
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
