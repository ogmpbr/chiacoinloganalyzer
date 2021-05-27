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

namespace xch_analyze
{
    public partial class Form1 : Form
    {

        private const Int16 TOTAL_STAGE = 10;
        List<logdata> items = null;
        Timer myTimer = null;
        int index;
        int type;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {

                    txtDir.Text = fbd.SelectedPath;
                    start(fbd.SelectedPath);
                
                }
            }


        }

        private void start(string dir)
        {

            items = new List<logdata>();
            logdata obj = null;
            Int16 stage = 0;

            string[] fileEntries = Directory.GetFiles(dir, "*.txt");

            foreach (string fileName in fileEntries) {

                using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {

                        obj = new logdata();
                        obj.filename = Path.GetFileName(fileName);
                        obj.datestart = File.GetCreationTime(fileName);
                        obj.datelastchange = File.GetLastWriteTime(fileName);
                        string line = string.Empty;
                        stage = 0;

                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();

                            if (line.Contains("Plot size is:"))
                            {
                                obj.plotsize = Convert.ToInt32(line.Replace("Plot size is:", "").Trim());
                                stage++;
                            }

                            if (line.Contains("Buffer size is:"))
                            {
                                obj.buffersize = Convert.ToInt32(line.Replace("Buffer size is:", "").Replace("MiB", "").Trim());
                                stage++;
                            }

                            if (line.Contains("Using ") && line.Contains(" buckets"))
                            {
                                obj.buckets = Convert.ToInt32(line.Replace("Using ", "").Replace(" buckets", "").Trim());
                                stage++;
                            }

                            if (line.Contains("threads of stripe size"))
                            {
                                string part1 = line.Replace("Using ", "").Replace("threads of stripe size", "").Trim();
                                string[] part2 = part1.Split(" ");
                                obj.threads = Convert.ToInt32(part2[0].Trim());
                                stage++;
                            }

                            if (line.Contains("Time for phase 1 = "))
                            {
                                string part1 = line.Replace("Time for phase 1 = ", "");
                                string[] part2 = part1.Split("seconds.");
                                obj.phase1 = calculateTime(Convert.ToDouble(part2[0].Trim()));
                                stage++;
                            }

                            if (line.Contains("Time for phase 2 = "))
                            {
                                string part1 = line.Replace("Time for phase 2 = ", "");
                                string[] part2 = part1.Split("seconds.");
                                obj.phase2 = calculateTime(Convert.ToDouble(part2[0].Trim()));
                                stage++;
                            }

                            if (line.Contains("Time for phase 3 = "))
                            {
                                string part1 = line.Replace("Time for phase 3 = ", "");
                                string[] part2 = part1.Split("seconds.");
                                obj.phase3 = calculateTime(Convert.ToDouble(part2[0].Trim()));
                                stage++;
                            }

                            if (line.Contains("Time for phase 4 = "))
                            {
                                string part1 = line.Replace("Time for phase 4 = ", "");
                                string[] part2 = part1.Split("seconds.");
                                obj.phase4 = calculateTime(Convert.ToDouble(part2[0].Trim()));
                                stage++;
                            }

                            if (line.Contains("Copy time = "))
                            {
                                string part1 = line.Replace("Copy time = ", "");
                                string[] part2 = part1.Split("seconds.");
                                obj.copytime = calculateTime(Convert.ToDouble(part2[0].Trim()));
                                stage++;
                            }

                            if (line.Contains("Total time = "))
                            {
                                string part1 = line.Replace("Total time = ", "");
                                string[] part2 = part1.Split("seconds.");
                                obj.total = calculateTime(Convert.ToDouble(part2[0].Trim()));
                                stage++;
                            }

                        }
                    }
                }

                obj.avg = (obj.phase1 + obj.phase2 + obj.phase3 + obj.phase4) / 4;
                obj.complete = (TOTAL_STAGE == stage);
                items.Add(obj);

            }

            index = 0;
            dataBinding();

        }

        private double calculateTime(double value)
        {

            if (btnMinutes.Checked)
                return value / 1000 / 60;
            if (btnHours.Checked)
                return value / 1000 / 60 / 60;

            return value;

        }

        private void btnHours_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDir.Text))
                start(txtDir.Text);
        }

        private void btnMinutes_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDir.Text))
                start(txtDir.Text);
        }

        private void btnSeconds_CheckedChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtDir.Text))
                start(txtDir.Text);

        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            index = e.ColumnIndex;
            dataBinding();
        }        

        private void dataBinding()
        {


            if (index == 0)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.filename).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.filename).ToList();
                    type = 0;
                }

            if (index == 1)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.plotsize).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.plotsize).ToList();
                    type = 0;
                }

            if (index == 2)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.buffersize).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.buffersize).ToList();
                    type = 0;
                }

            if (index == 3)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.buckets).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.buckets).ToList();
                    type = 0;
                }

            if (index == 4)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.threads).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.threads).ToList();
                    type = 0;
                }

            if (index == 5)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.phase1).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.phase1).ToList();
                    type = 0;
                }


            if (index == 6)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.phase2).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.phase2).ToList();
                    type = 0;
                }

            if (index == 7)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.phase3).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.phase3).ToList();
                    type = 0;
                }


            if (index == 8)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.phase4).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.phase4).ToList();
                    type = 0;
                }

            if (index == 9)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.total).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.total).ToList();
                    type = 0;
                }

            if (index == 10)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.total).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.total).ToList();
                    type = 0;
                }

            if (index == 11)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.copytime).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.copytime).ToList();
                    type = 0;
                }

            if (index == 12)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.datestart).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.datestart).ToList();
                    type = 0;
                }

            if (index == 13)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.datelastchange).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.datelastchange).ToList();
                    type = 0;
                }

            if (index == 14)
                if (type == 0)
                {
                    dataGridView1.DataSource = items.OrderBy(x => x.complete).ToList();
                    type = 1;
                }
                else
                {
                    dataGridView1.DataSource = items.OrderByDescending(x => x.complete).ToList();
                    type = 0;
                }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (myTimer == null)
            {
                myTimer = new Timer();
                myTimer.Interval = (60 * 1000);
                myTimer.Tick += new EventHandler(myTimer_Tick);
                myTimer.Start();
            }else
            {
                myTimer.Stop();
                myTimer = null;
            }
            
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            if (type == 0) type = 1; else type = 0;
            dataBinding();
        }

    }
}
