using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;
using System.Threading;
using System.Collections;

namespace GaussianMapRender
{
    public partial class Form1 : Form
    {
        public int MAP_ZOOM = 5;    // default zoom of map

        private const String DEFAULT_LOCATION = "Seattle, Washington";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.SetPositionByKeywords(DEFAULT_LOCATION);
            gmap.ShowCenter = false;
            gmap.Zoom = 5;
            TestDataFiles();
        }

        private void gmap_Load(object sender, EventArgs e)
        {

        }

        private void SaveMap_Click(object sender, EventArgs e)
        {
            Image g = gmap.ToImage();
            try
            {
                g.Save(@"C:\Users\DevWork\Desktop\source.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void saveGMapToDisk(String filePath)
        {
            Image g = gmap.ToImage();
            try
            {
                g.Save(@"C:\Users\ZachCheu\Desktop\test.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void userDialogBox()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("Selected Path: " + folderDialog.SelectedPath);
                }
            }
        }

        private void openDialog_button_Click(object sender, EventArgs e)
        {
            string latData = System.IO.File.ReadAllText(@"C:\Users\DevWork\Desktop\lat.txt");
            string lngData = System.IO.File.ReadAllText(@"C:\Users\DevWork\Desktop\long.txt");

            string[] latDataArray = latData.Split(',');
            string[] lngDataArray = lngData.Split(',');

            Console.WriteLine(latDataArray.Length);
            Console.WriteLine(lngDataArray.Length);

            lngDataArray.ToList().ForEach(Console.WriteLine);
            latDataArray.ToList().ForEach(Console.WriteLine);

            for (int i = 0; i < latDataArray.Length; i++)    // loop to get latitudes
            {
                for (int j = 0; j < lngDataArray.Length; j++)   // loop to get longitudes
                {
                    PointF coordinatePoint = new PointF(float.Parse(latDataArray[i]), float.Parse(lngDataArray[j]));
                    addGPSPoint(coordinatePoint);
                }
            }
        }

        private void addGPSPoint(PointF point)
        {
            int iconWidth = 5;  // icon width
            int iconHeight = 5; // icon height
            GMapOverlay markers = new GMapOverlay("markers");
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(point.X, point.Y),
             new Bitmap(circle(iconWidth, iconHeight)));
            markers.Markers.Add(marker);
            gmap.Overlays.Add(markers);
        }

        // method meant for testing lat long data files
        public void TestDataFiles()
        {
            string URL = @"C:\Users\DevWork\Desktop\example\p_";
            // reads everything in one line
            string latData = System.IO.File.ReadAllText(@"C:\Users\DevWork\Desktop\lat.txt");
            string lngData = System.IO.File.ReadAllText(@"C:\Users\DevWork\Desktop\long.txt");
            ArrayList alphaValues = new ArrayList();
            string URL_ALPHA_VALUES = URL + "1.txt";

            // AVParser => alpha value parser class
            AlphaValueParser AVParser = new AlphaValueParser(URL_ALPHA_VALUES);
            AVParser.ParseFile();
            AVParser.PrintAlphaValues();
        }

        // returns bitmap: circle img
        private Bitmap circle(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            Color c = Color.FromArgb(50, 0, 0, 0);
            Brush b = new SolidBrush(c);
            g.FillEllipse(b, 0, 0, width, height);
            return bmp;
        }

        private void zoomplus_btn_Click(object sender, EventArgs e)
        {
            MAP_ZOOM++;
            gmap.Zoom = MAP_ZOOM;
        }

        private void zoomminus_btn_Click(object sender, EventArgs e)
        {
            MAP_ZOOM--;
            if (MAP_ZOOM > 1)
            {
                gmap.Zoom = MAP_ZOOM;
            }
        }
    }
}
