using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using System.IO;
using LiveCharts.Defaults;
using System.Windows.Media;
using LiveCharts.Geared;

namespace LogViewer
{
    public partial class Form1 : Form
    {
        List<string> headerlist = new List<string>();
        List<string> fieldlist = new List<string>();
        List<double> timelist = new List<double>();
        List<List<double>> datalist = new List<List<double>>();
        List<Series> serieslist = new List<Series>();
        System.Windows.Media.Brush[] Brushes =

        {
            System.Windows.Media.Brushes.Red,
            System.Windows.Media.Brushes.Blue,
            System.Windows.Media.Brushes.Green
         };

        public Form1()
        {
            InitializeComponent();
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string line;
                    bool donefieldnames = false;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            headerlist.Add(line);
                        }
                        else
                        {
                            if (!donefieldnames)
                            {
                                parsefieldnames(line);                              
                                donefieldnames = true;
                                int i;
                                for (i=0; i<fieldlist.Count; i++)
                                {
                                    datalist.Add ( new List<double>());
                                }
                            }
                            else
                            {
                                ParseDataLine(line);
                            }
                        }
                    }
                    for (int i=0; i < fieldlist.Count; i++ )
                        {
                        serieslist.Add(CreateSeries(i));
                        }
                }
            }
        }
        private void parsefieldnames(string line)
        {
            string[] fieldnames = line.Split(',');
            
            fieldlist = fieldnames.Skip(1).ToList();
            checkedListBox1.Items.AddRange(fieldlist.ToArray());
        }
        private void ParseDataLine(string line)
        {
            string[] datavalues = line.Split(',');

            timelist.Add(Convert.ToDouble(datavalues[0]));
            int i = 0;
            foreach (string datavalue in datavalues.Skip(1))
            {
                datalist[i].Add(Convert.ToDouble(datavalue ));
                
                i++;
            }

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            AddToGraph(e);
        }

        private void AddToGraph(ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                // MessageBox.Show(e.Index.ToString() + " Checked");
                
            }
            else
            {
                // MessageBox.Show(e.Index.ToString() + " Unchecked");
            }
        }

        private void UpdateChart()
        {
            cartesianChart1.Visible = false;
            int seriescount = 0;
            cartesianChart1.AxisX.Clear();
            cartesianChart1.AxisX.Add(new Axis { Title = "Time (s)" });
            cartesianChart1.AxisY.Clear();
            cartesianChart1.Series.Clear();
            cartesianChart1.Zoom = ZoomingOptions.X;
            cartesianChart1.DisableAnimations = true;
            for (int i=0; i<fieldlist.Count; i++)
            {
                
                bool st = checkedListBox1.GetItemChecked(i);
                if (st)
                {
                    Series s = serieslist[i];
                    AxisPosition ap = ((seriescount & 1) == 0) ? AxisPosition.RightTop : AxisPosition.LeftBottom;
                    cartesianChart1.AxisY.Add(new Axis { Title = fieldlist[i], Position = ap, Foreground = Brushes[seriescount % Brushes.Length] });
                    s.Stroke = Brushes[seriescount % Brushes.Length];
                    s.ScalesYAt = seriescount;
                    cartesianChart1.Series.Add(s);
                    
                    seriescount++;

                }
                cartesianChart1.Visible = true;
                
            }
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private Series CreateSeries(int i)
        {

            List<ObservablePoint> lop = new List<ObservablePoint>();
            for (int j=0; j<datalist[i].Count; j++)
            {
                ObservablePoint op = new ObservablePoint(timelist[j], datalist[i][j]);
                lop.Add (op);
                
            }
            Series s = new LineSeries { Values = new ChartValues<ObservablePoint>(lop.ToArray()),
                PointGeometry = null,
                StrokeThickness = 1,
                LineSmoothness = 1,
                Fill = System.Windows.Media.Brushes.Transparent,
                Title = fieldlist[i]
            };
            return (s);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateChart();
        }
    }

}
