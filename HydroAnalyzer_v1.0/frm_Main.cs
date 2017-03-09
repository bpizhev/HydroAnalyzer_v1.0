using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;


namespace HydroAnalyzer_v0._4
{
    public partial class frm_Main : Form
    {
        private MySqlConnection connection;

        public frm_Main(string ip, string db, string user, string pass)
        {          
            InitializeComponent();

            string MyConString = "SERVER=" + ip + ";" +
               "DATABASE=" + db + ";" +
               "UID=" + user + ";" +
               "PASSWORD=" + pass + ";Character Set=utf8;";
            connection = new MySqlConnection(MyConString);
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.WindowState = FormWindowState.Maximized;

            //Импорт на номер на станция от базата.
            string command = "select Station from spisakhyd";
           
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;

            comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;

            comboBox3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox3.AutoCompleteSource = AutoCompleteSource.ListItems;


            MySqlDataAdapter da = new MySqlDataAdapter(command, connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                string rowz = string.Format("{0}", row.ItemArray[0]);
                comboBox1.Items.Add(rowz);
                comboBox1.AutoCompleteCustomSource.Add(row.ItemArray[0].ToString());
            }   
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string command1 = "select year(Dat) FROM hydgod where Station=" + comboBox1.Text;
            MySqlDataAdapter da1 = new MySqlDataAdapter(command1, connection);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox2.SelectedItem = -1;
            comboBox3.SelectedItem = -1;

            foreach (DataRow row in dt1.Rows)
            {
                string rowz = string.Format("{0}", row.ItemArray[0]);
                comboBox2.Items.Add(rowz);
                comboBox3.Items.Add(rowz);
                comboBox2.AutoCompleteCustomSource.Add(row.ItemArray[0].ToString());
                comboBox3.AutoCompleteCustomSource.Add(row.ItemArray[0].ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series["Avg"].Points.Clear();
            chart1.Series["Min"].Points.Clear();
            chart1.Series["Max"].Points.Clear();

            chart2.Series["Avg"].Points.Clear();
            chart2.Series["Min"].Points.Clear();
            chart2.Series["Max"].Points.Clear();

            chart3.Series["Avg"].Points.Clear();

            var StartDate = comboBox2.Text;
            var EndDate = comboBox3.Text;
            var eDate = Convert.ToInt32(EndDate);
            var sDate = Convert.ToInt32(StartDate);
            if (StartDate != "" && EndDate != "" && sDate > eDate)
            {
                MessageBox.Show("Грешка! Въвели сте по-голяма начална дата от крайната!");
                return;
            }

            else if (StartDate != "" && EndDate != "" && sDate == eDate)
            {
                MessageBox.Show("Грешка! Въвели сте една и съща начална и крайна дата !");
                return;
            }

            string command2 = "select God_MinQ,God_AverQ,God_MaxQ,year(Dat) from hydgod where station='"
                + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '" 
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString() 
                + "'group by year(dat),month(Dat)";

            string command3 = "select VkolMin,VkolSre,VkolMax,year(Dat) from hydmes where station='"
                + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '"
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString()
                + "' group by year(dat),month(Dat)";

            string command4 = "select vkol from hyddnev where station='"
                + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '"
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString()
                + "'order by Dat";

            //За Data Grid View от hyddnev

            int year1 = int.Parse(comboBox2.SelectedItem.ToString());
            int year2 = int.Parse(comboBox3.SelectedItem.ToString());

            string command5 = "";
            for (int y = year1; y <= year2; y++)
            {
                if (y > year1)
                {
                    command5 += " UNION ";
                }
                command5 += "SELECT 'Въведени' AS 'Въведени/Изчислени', Station AS '№ на станция', year(Dat) AS 'Година', 'НМ' AS 'НМ/СР/НГ', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=1) AS 'Януари', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=2) AS 'Февруари', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=3) AS 'Март', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=4) AS 'Април', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=5) AS 'Май', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=6) AS 'Юни', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=7) AS 'Юли', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=8) AS 'Август', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=9) AS 'Септември', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=10) AS 'Октомври', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=11) AS 'Ноември', (SELECT CAST(vkolmin AS DECIMAL(7,3)) FROM hydmes WHERE Station= '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat)=12) AS 'Декември'"
                + "FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y
                + "  UNION"
                + "  SELECT 'Изчислени', Station, year(Dat), 'НМ', (SELECT CAST(min(vkol) AS DECIMAL(7,3))  FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 1 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 2 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 3 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 4 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 5 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 6 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 7 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 8 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 9 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 10 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 11 LIMIT 1), (SELECT CAST(min(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 12 LIMIT 1)"
                + "FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y
                + "  UNION"
                + "  SELECT 'Въведени', Station, year(Dat), 'СР', (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 1), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 2), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 3), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 4), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 5), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 6), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 7), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 8), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 9), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 10), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 11), (SELECT CAST(vkolsre AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 12)"
                + "FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y
                + "  UNION"
                + "  SELECT 'Изчислени', Station, year(Dat), 'СР', (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 1 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 2 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 3 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 4 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 5 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 6 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 7 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 8 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 9 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 10 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 11 LIMIT 1), (SELECT CAST(avg(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 12 LIMIT 1)"
                + "FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y
                + "  UNION"
                + "  SELECT 'Въведени', Station, year(Dat), 'НГ', (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 1), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 2), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 3), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 4), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 5), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 6), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 7), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 8), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 9), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 10), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 11), (SELECT CAST(vkolmax AS DECIMAL(7,3)) FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 12)"
                + "FROM hydmes WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y
                + "  UNION"
                + "  SELECT 'Изчислени', Station, year(Dat), 'НГ', (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 1 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 2 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 3 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 4 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 5 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 6 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 7 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 8 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 9 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 10 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 11 LIMIT 1), (SELECT CAST(max(vkol) AS DECIMAL(7,3)) FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y + " and month(dat) = 12 LIMIT 1)"
                + "FROM hyddnev WHERE Station = '" + comboBox1.SelectedItem.ToString() + "' and year(Dat) = " + y;
                //(SELECT CAST(max(vkol) AS DECIMAL(7,3))
                //(SELECT CAST(vkolmax AS DECIMAL(7,3))
            }
            command5 += "  group by year(dat)";

            string command6 = "select avg(Vkol) from hyddnev where Station='" + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '"
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString()
                + "'group by year(dat),month(dat)";

            string command7 = "select max(Vkol) from hyddnev where Station='" + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '"
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString()
                + "'group by year(dat),month(dat)";

            //За Data Grid View от hydmes
            string command8 = "select VkolMin from hydmes where Station='" + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '"
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString()
                + "'group by year(dat),month(dat)";

            string command9 = "select VkolSre from hydmes where Station='" + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '"
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString()
                + "'group by year(dat),month(dat)";

            string command10 = "select VkolMax from hydmes where Station='" + comboBox1.SelectedItem.ToString() + "' and year(Dat) >= '"
                + comboBox2.SelectedItem.ToString() + "' and year(Dat) < '" + comboBox3.SelectedItem.ToString()
                + "'group by year(dat),month(dat)";


            MySqlDataAdapter da3 = new MySqlDataAdapter(command3, connection);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);

            string s1 = "";

            chart1.ChartAreas[0].AxisX.Minimum = double.NaN;
            chart1.ChartAreas[0].AxisX.Maximum = double.NaN;
            chart1.ChartAreas[0].RecalculateAxesScale();

            chart1.Series["Min"].BorderWidth = 3;
            chart1.Series["Avg"].BorderWidth = 3;
            chart1.Series["Max"].BorderWidth = 3;

            int i = 0;
            int startYear= 0;
            int.TryParse(StartDate, out startYear);
            DateTime dtStart = new DateTime(startYear, 01, 01);
            chart1.Series["Min"].XValueType = ChartValueType.DateTime;
            chart1.Series["Avg"].XValueType = ChartValueType.DateTime;
            chart1.Series["Max"].XValueType = ChartValueType.DateTime;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months;
            chart1.ChartAreas[0].AxisX.IntervalOffset = 0;

            foreach (DataRow row in dt3.Rows)
            {
                chart1.Series["Avg"].Points.AddXY(dtStart.AddMonths(i), row.ItemArray[1]);
                chart1.Series["Max"].Points.AddXY(dtStart.AddMonths(i), row.ItemArray[2]);
                chart1.Series["Min"].Points.AddXY(dtStart.AddMonths(i), row.ItemArray[0]);
                  
                i++;

                string rowz = string.Format("Min Q - {0}" + Environment.NewLine + "Avg Q - {1}"
                   + Environment.NewLine + "Max Q - {2}" + Environment.NewLine + "Year - {3}" + Environment.NewLine
                   + Environment.NewLine,
                   row.ItemArray[0], row.ItemArray[1], row.ItemArray[2],
                   row.ItemArray[3]);
                s1 += "" + rowz;
            }

           //MessageBox.Show(s1);

            MySqlDataAdapter da2 = new MySqlDataAdapter(command2, connection);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            string s = "";

            i = 0;
            
            int startYear1 = 0;
            int.TryParse(StartDate, out startYear1);
            DateTime dtStart1 = new DateTime(startYear1, 01, 01);
            chart2.Series["Min"].XValueType = ChartValueType.DateTime;
            chart2.Series["Avg"].XValueType = ChartValueType.DateTime;
            chart2.Series["Max"].XValueType = ChartValueType.DateTime;
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy";
            chart2.ChartAreas[0].AxisX.Interval = 1;
            chart2.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Years;
            chart2.ChartAreas[0].AxisX.IntervalOffset = 1; 

            chart2.Series["Min"].MarkerSize = 10;
            chart2.Series["Avg"].MarkerSize = 10;
            chart2.Series["Max"].MarkerSize = 10;

            foreach (DataRow row in dt2.Rows)
             {
                   chart2.Series["Min"].Points.AddXY(dtStart1.AddYears(i), row.ItemArray[0]);
                chart2.Series["Avg"].Points.AddXY(dtStart1.AddYears(i), row.ItemArray[1]);
                chart2.Series["Max"].Points.AddXY(dtStart1.AddYears(i), row.ItemArray[2]);
                   i++;

                string rowz = string.Format("Min Q - {0}" + Environment.NewLine + "Avg Q - {1}"
                   + Environment.NewLine + "Max Q - {2}" + Environment.NewLine + "Year - {3}" + Environment.NewLine
                   + Environment.NewLine,
                   row.ItemArray[0], row.ItemArray[1], row.ItemArray[2],
                   row.ItemArray[3]);
                s += "" + rowz;                                   
            }

            //MessageBox.Show(s);

            MySqlDataAdapter da4 = new MySqlDataAdapter(command4, connection);
            DataTable dt4 = new DataTable();
            da4.Fill(dt4);


            string s2 = "";
            i = 0;

            int startYear2 = 0;
            int.TryParse(StartDate, out startYear2);
            DateTime dtStart2 = new DateTime(startYear2, 01, 01);
            chart3.Series["Avg"].XValueType = ChartValueType.DateTime;
            chart3.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chart3.ChartAreas[0].AxisX.Interval = 1;
            chart3.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
            chart3.ChartAreas[0].AxisX.IntervalOffset = 1;
            
            chart3.Series["Avg"].MarkerSize = 10;

            foreach (DataRow row in dt4.Rows)
            {
                chart3.Series["Avg"].Points.AddXY(dtStart2.AddDays(i), row.ItemArray[0]);
                i++;

                string rowz = string.Format("Avg Q - {0}" + Environment.NewLine, row.ItemArray[0]);
                s2 += "" + rowz;
            }

            /*
            // zoom на третата графика
            Axis ax = chart3.ChartAreas[0].AxisX;
            ax.ScaleView.Size = 30;
            ax.ScaleView.Position = 30;
            chart3.Show();
            */

            // чистя двата комбобокса след натискане на бутона
            comboBox2.Text = "";
            comboBox3.Text = "";

            
            MySqlDataAdapter da5 = new MySqlDataAdapter(command5, connection);
            using (DataTable dt5 = new DataTable())
            {
                da5.Fill(dt5);
                dataGridView1.DataSource = dt5.DefaultView;
            }
            

        }

        private void frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            connection.Close();
            Application.ExitThread();
        }
    }
}

