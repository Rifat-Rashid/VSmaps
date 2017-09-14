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
using System.Device.Location;

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
            //TestDataFiles();
        }

        private void gmap_Load(object sender, EventArgs e)
        {

        }

        private void SaveMap_Click(object sender, EventArgs e)
        {
            TestDataFiles();
            /*
            Image g = gmap.ToImage();
            try
            {
                g.Save(@"C:\Users\tejas\Desktop\source.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
        }

        public Image getGmapImage()
        {
            Image img = gmap.ToImage();
            Console.WriteLine(img.Width);
            Console.WriteLine(img.Height);
            return img;
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
        private void renderBitmaps(List<double> lats, List<double> lngs, List<double> alphaValues, Image img)
        {
            List<Bitmap> miniBitmaps = new List<Bitmap>();
            PointF topLeft = new PointF();
            PointF topRight = new PointF();
            PointF botLeft = new PointF();
            PointF botRight = new PointF();
            //lng = x, lat = y
            topLeft.X = (float)gmap.ViewArea.LocationTopLeft.Lng;
            topLeft.Y = (float)gmap.ViewArea.LocationTopLeft.Lat;
            topRight.X = (float)(gmap.ViewArea.LocationTopLeft.Lng + gmap.ViewArea.WidthLng);
            topRight.Y = topLeft.Y;
            botRight.X = (float)gmap.ViewArea.LocationRightBottom.Lng;
            botRight.Y = (float)gmap.ViewArea.LocationRightBottom.Lat;
            botLeft.X = (float)(gmap.ViewArea.LocationRightBottom.Lng - gmap.ViewArea.WidthLng);
            botLeft.Y = botRight.Y;
            Console.WriteLine("TL: " + topLeft.ToString());
            Console.WriteLine("TR: " + topRight.ToString());
            Console.WriteLine("BR: " + botRight.ToString());
            Console.WriteLine("BL: " + botLeft.ToString());


            BitmapCalculator bitmapCalculator = new BitmapCalculator();
            //test
            // first param: start latitude
            // second param: start longitude
            // third param: end latitude
            // fourth param: end longitude
            // calculateDistance method calculates distance in meters between two lat lngs
            double widthDistance = bitmapCalculator.calculateDistance((float)lats[0], (float)lngs[0], (float)lats[0], (float)lngs[1]);
            double maxWidthDistance = bitmapCalculator.calculateDistance(lats[0], lngs[0], lats[0], lngs[lngs.Count - 1]);
            double heightDistance = bitmapCalculator.calculateDistance((float)lats[0], (float)lngs[0], (float)lats[1], (float)lngs[0]);
            double maxHeightDistance = bitmapCalculator.calculateDistance(lats[0], lngs[0], lats[lats.Count - 1], lngs[0]);
            int width = (int)bitmapCalculator.calculateBitmapWidth(widthDistance, maxWidthDistance, img.Width);
            int height = (int)bitmapCalculator.calculateBitmapHeight(heightDistance, maxHeightDistance, img.Height);
            //---------------------------------------------------------------------------------------------------------
            // test logic v0.0.1
            PointF firstCoordinate = new PointF((float)lats[0], (float)lngs[0]);
            PointF secondCoordinate = new PointF((float)lats[0], (float)lngs[1]);

            // calculate distance
            GeoCoordinate coord_1 = new GeoCoordinate(firstCoordinate.X, firstCoordinate.Y);
            GeoCoordinate coord_2 = new GeoCoordinate(secondCoordinate.X, secondCoordinate.Y);

            GeoCoordinate coord_3 = new GeoCoordinate(topLeft.Y, topLeft.X);
            GeoCoordinate coord_4 = new GeoCoordinate(topRight.Y, topRight.X);
            // returns distance in meters accroding to docs
            // @docs: https://msdn.microsoft.com/en-us/library/system.device.location.geocoordinate.getdistanceto(v=vs.110).aspx
            double distance = coord_1.GetDistanceTo(coord_2);
            double screenDistance = coord_3.GetDistanceTo(coord_4);

            Console.WriteLine("Distance between coord1 and 2: "+ distance);
            Console.WriteLine("screen distance: " + screenDistance);

            // print check
            //Console.WriteLine("Distance: " + distance);

            // GET screen dimensions
            Image bitmap = gmap.ToImage();
            double bitmapWidth = bitmap.Width;
            double bitmapHeight = bitmap.Height;

            Console.WriteLine("BITMAP WIDTH: " + bitmapWidth);

            // calculate ratios (meters per pixel?)
            double ratio = screenDistance/bitmapWidth; // meters/pixels
            Console.WriteLine(screenDistance);

            // print results
            Console.WriteLine("Matrix Shift: " + distance / ratio);
                    Bitmap b = new Bitmap((int)Math.Ceiling(distance/ratio), (int)Math.Ceiling(distance/ratio));
                    Graphics g = Graphics.FromImage(b);
                    Color c = Color.FromArgb(255, 255, 0, 0);
                    Brush brush = new SolidBrush(c);///@replace
                    g.FillRectangle(brush, 0, 0, b.Width, b.Height);
                    //Console.WriteLine(alphaValues[i + j]);
                    GMapOverlay markers = new GMapOverlay("markers");
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(firstCoordinate.X, firstCoordinate.Y),
                        b);
                    markers.Markers.Add(marker);
                    gmap.Overlays.Add(markers);

            

            Console.WriteLine(width + " " + height);
            for (int i = 0; i < lats.Count; i++)
            {
                for (int j = 0; j < lngs.Count; j++)
                {
                    // JANK BITMAP CREATION CODE
                    /*Bitmap b = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(b);
                    Color c = Color.FromArgb((int)Math.Round(alphaValues[i + j]), 255, 0, 0);
                    Brush brush = new SolidBrush(c);///@replace
                    g.FillRectangle(brush, 0, 0, b.Width, b.Height);
                    //Console.WriteLine(alphaValues[i + j]);
                    GMapOverlay markers = new GMapOverlay("markers");
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lats[i], lngs[j]),
                        new Bitmap(rectangle((int) alphaValues[i + j], width, height)));
                    markers.Markers.Add(marker);
                    gmap.Overlays.Add(markers);*/

                    miniBitmaps.Add(rectangle((int)alphaValues[i + j], width, height));
                }
            }
            
        }

        // method meant for testing lat long data files
        public void TestDataFiles()
        {
            string URL = @"C:\Users\DevWork\Desktop\";
            string URL_ALPHA_VALUES = URL + "1.txt";

            ParserManager P = new ParserManager();
            P.execute();
            List<double> lats = P.latitudeValues;
            List<double> lngs = P.longitudeValues;
            List<double> alphaValues = P.alphaValues;
            double min = P.getMin(alphaValues);
            double max = P.getMax(alphaValues);
            P.scale(alphaValues);

            /*for(int i = 0; i < lats.Count; i++)
            {
                for(int j = 0; j < lngs.Count; j++)
                {
                    PointF p = new PointF((float)lats[i], (float)lngs[j]);
                    addGPSPoint(p);
                }
            }*/
            Image img = getGmapImage();
            renderBitmaps(lats, lngs, alphaValues, img);

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
        private Bitmap rectangle(double alphaValue, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            Color c = Color.FromArgb(((int)(alphaValue)), 255, 0, 0);
            Brush b = new SolidBrush(c);
            g.FillRectangle(b, 0, 0, width, height);
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
