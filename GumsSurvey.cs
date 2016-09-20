using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace Gums
{
    class GumsSurvey
    {
        static System.Timers.Timer tmr = new System.Timers.Timer();

        static String lastKey;

        static Dictionary<String, int> statistiques = new Dictionary<String, int>();

        static String gumsPath;

        static int lastTick;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public GumsSurvey()
        {
            tmr.Interval = 1000; 
            tmr.Elapsed += surveyHandler;
        }

        public void pauseSurvey()
        {
            tmr.Stop();
        }

        public void continueSurvey()
        {
            lastTick = (Environment.TickCount & Int32.MaxValue);
            tmr.Start();
        }

        public void startSurvey()
        {
            DateTime thisDay = DateTime.Today;
            String currentDateStr = thisDay.ToString("yyyyMMdd");

            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            gumsPath = pathDesktop + "\\Gums";

            if (Directory.Exists(gumsPath)) {

                Console.WriteLine("---> Directory.Exists : " + gumsPath);

                if (File.Exists(gumsPath + "\\whatDidYouDo_" + currentDateStr + "_duration.csv"))
                {
                    Console.WriteLine("---> File.Exists : " + gumsPath + "\\whatDidYouDo_" + currentDateStr + "_duration.csv");

                    String[] lines = File.ReadAllLines(gumsPath + "\\whatDidYouDo_" + currentDateStr + "_duration.csv");

                    Console.WriteLine("---> Lines : " + lines.Length);

                    foreach (String line in lines)
                    {
                        if (line != "")
                        {
                            Console.WriteLine("---> Line : "+line);
                            String[] lineDetails = line.Split(';');
                            if (lineDetails.Length > 2)
                            {
                                String durationStr = lineDetails[0];
                                String key = "";
                                for (int i = 1; i < lineDetails.Length; i++)
                                {
                                    key = (key == "") ? lineDetails[i] : key + ";"+ lineDetails[i];
                                }

                                String[] durationDetails = durationStr.Split(':');
                                if (durationDetails.Length == 3)
                                {
                                    int heure   = Int32.Parse(durationDetails[0]);
                                    int minute  = Int32.Parse(durationDetails[1]);
                                    int seconde = Int32.Parse(durationDetails[2]);

                                    int duration = heure * 3600 + minute * 60 + seconde;

                                    statistiques.Add(key, duration);
                                }
                                Console.WriteLine(key);
                            }
                            
                        }
                    }
                }
            }

            lastTick = (Environment.TickCount & Int32.MaxValue);

            tmr.Start();
        }

        private static void surveyHandler(Object source, ElapsedEventArgs e)
        {
            

            tmr.Stop();
            try
            { 
                IntPtr fg = GetForegroundWindow(); 
                int processId = 0;
                int threadId = GetWindowThreadProcessId(fg, out processId);

                Process process = Process.GetProcessById((int)processId);

                //Console.WriteLine("---> Name  : " + process.ProcessName);
                //Console.WriteLine("---> Id    : " + process.Id);
                //Console.WriteLine("---> Hndw  : " + process.MainWindowHandle);
                //Console.WriteLine("---> Mod   : " + process.MainModule.FileName);
                //Console.WriteLine("---> Title : " + process.MainWindowTitle);
                //Console.WriteLine("-------------------------------------------");

                DateTime thisDay = DateTime.Today;
                String currentDateStr = thisDay.ToString("yyyyMMdd");
                String  key = currentDateStr+";"+ process.ProcessName + ";" + process.Id + ";" + process.MainWindowHandle + ";" + process.MainWindowTitle  + ";" + process.MainModule.FileName  ;

                //Console.WriteLine(currentDateStr);

                if (statistiques.ContainsKey(key)){
                    int value = statistiques[key];
                    int currentTick = (Environment.TickCount & Int32.MaxValue);
                    int inc = (currentTick - lastTick) / 1000;
                    statistiques[key] = value + inc;
                    lastTick = currentTick;
                }
                else {
                    statistiques.Add(key, 1);
                }        

                List<String> result  = new List<String>();
                List<String> toClean = new List<String>();

                String statStr = "";
                foreach (var item in statistiques)
                {

                    String myKey = item.Key;
                    int myValue = item.Value;

                    // Clés à virer
                    if (!myKey.Contains(currentDateStr))
                    {
                        toClean.Add(myKey);
                    }
                    else {
                        int heure = 0;
                        int min = 0;
                        int sec = 0;

                        if (myValue < 60)
                        {
                            sec = myValue;
                        }
                        else {
                            if (myValue < 3600)
                            {
                                min = myValue / 60;
                                sec = myValue % 60;
                            }
                            else {
                                heure = myValue / 3600;
                                int secRestantes = myValue % 3600;
                                min = secRestantes / 60;
                                sec = secRestantes % 60;
                            }
                        }
                        String heureStr = (heure < 10) ? "0" + heure : "" + heure;
                        String minStr = (min < 10) ? "0" + min : "" + min;
                        String secStr = (sec < 10) ? "0" + sec : "" + sec;

                        result.Add(heureStr + ":" + minStr + ":" + secStr + ";" + myKey + "");
                    }
                }

                result = result.OrderByDescending(i => i).ToList();

                foreach (String toRemove in toClean) {
                    statistiques.Remove(toRemove);
                }

                foreach (String toPrint in result) {
                    statStr = statStr  + "\r\n" + toPrint; 
                }
                
                if (!Directory.Exists(gumsPath))
                {
                    Console.WriteLine("-> Create directory : "+  gumsPath);
                    Directory.CreateDirectory(gumsPath);
                }

                TextWriter tw = new StreamWriter(gumsPath+ "\\whatDidYouDo_" + currentDateStr + "_duration.csv");
                tw.WriteLine(statStr);
                tw.Close();

                if (lastKey != key)
                {
                    DateTime currentTime = DateTime.Now;
                    TextWriter w = File.AppendText(gumsPath + "\\whatDidYouDo_" + currentDateStr + "_time.csv");
  
                    String timeStr = currentTime.ToString("HH:mm:ss") + ";" + key ;
                    w.WriteLine(timeStr);
                    w.Close();
                }

                lastKey = key;

            }
            catch (Exception exception)
            {
                Console.WriteLine("-> Exception : "+ exception.Message);
                //MessageBox.Show(exception.Message, "Gums", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
                tmr.Start();
            }

            
        }
    }
}
