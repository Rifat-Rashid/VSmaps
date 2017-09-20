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
using System.Diagnostics;

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
        private void RenderBitmaps(List<double> lats, List<double> lngs, List<double> alphaValues, Image img)
        {
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

            Console.WriteLine("Distance between coord1 and 2: " + distance);
            Console.WriteLine("screen distance: " + screenDistance);

            // GET screen dimensions
            Image bitmap = gmap.ToImage();
            double bitmapWidth = bitmap.Width;
            double bitmapHeight = bitmap.Height;

            Console.WriteLine("BITMAP WIDTH: " + bitmapWidth);

            // calculate ratios (meters per pixel?)
            double ratio = screenDistance / bitmapWidth; // meters/pixels
            Console.WriteLine(screenDistance);
            Console.WriteLine(width + " " + height);

            Bitmap[,] preStitchedCollection = new Bitmap[lats.Count, lngs.Count];
            for (int i = 0; i < lats.Count; i++)
            {
                for (int j = 0; j < lngs.Count; j++)
                {
                    // add bitmap to collection for stitching process
                    preStitchedCollection[i, j] = getAlphaMap((int)alphaValues[i + j], width, height);
                }
            }

            StitchedBitmap(preStitchedCollection);
        }

        /// <summary>
        /// Given a 2D array of segmented bitmaps, this method will stitch all of them together into one super bitmap.
        /// </summary>
        /// <param name="bitmapCollection">2D Array of segmented bitmaps</param>
        public void StitchedBitmap(Bitmap[,] bitmapCollection)
        {
            // width and height can be calculated ahead of time @bitmapCollection creation
            int totalWidth = 0;
            int totalHeight = 0;

            // GetUpperBound() method is very slow. Call this once outside of loop to save time.
            int bitmapCollectionI = bitmapCollection.GetUpperBound(0);
            int bitmapCollectionJ = bitmapCollection.GetUpperBound(1);

            // loop through collection to find width and height of newly created bitmap
            for (int i = 0; i <= bitmapCollectionI; i++)
            {
                for (int j = 0; j <= bitmapCollectionJ; j++)
                {
                    try
                    {
                        // counting width (i)
                        if (i == 0)
                            totalWidth += bitmapCollection[i, j].Width;
                        // counting height (j)
                        if (j == 0)
                            totalHeight += bitmapCollection[i, j].Height;
                    }
                    catch (NullReferenceException nullRefrenceException)
                    {
                        Console.WriteLine("Null refrence in bitmap stitching method");
                        Console.WriteLine(nullRefrenceException.ToString());
                    }
                    catch (Exception e)  // for generic exceptions
                    {
                        Console.WriteLine("Generic exception");
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            Debug.WriteLine("BitmapStitchedSize: W:" + totalWidth + " H:" + totalHeight);
            Bitmap superBitmap = new Bitmap(totalWidth, totalHeight);
            int currentY = 0;   // keeps track of y coordinate
            int iterates = 0;

            // loop for copying micro bitmaps onto super bitmap
            // @SOURCE: https://stackoverflow.com/questions/9616617/c-sharp-copy-paste-an-image-region-into-another-image
            using (Graphics g = Graphics.FromImage(superBitmap))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, totalWidth, totalHeight);
                // copy bitmap collection onto super bitmap
                for (int i = 0; i <= bitmapCollectionI; i++)
                {
                    int currentX = 0;   // keeps track of x coordinate
                    for (int j = 0; j <= bitmapCollectionJ; j++)
                    {
                        Bitmap sampledBitmap = bitmapCollection[i, j];
                        g.DrawImage(sampledBitmap, currentX, currentY);
                        currentX += sampledBitmap.Width;
                        if (j == bitmapCollectionJ)
                        {
                            currentY += sampledBitmap.Height;
                            Console.WriteLine(currentY);
                        }     
                    }
                    currentX = 0;
                }
            }

            // casting superbitmap to a image for saving on disk
            Image superImage = (Image)superBitmap;
            try
            {
                superImage.Save(@"C:\Users\Rashid\Documents\GitHub\VSmaps\Data\testIMG\test" + iterates + ".png");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            Image img = getGmapImage();
            RenderBitmaps(lats, lngs, alphaValues, img);

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

        /// <summary>
        /// creates bitmap with size width, height and fills it in with passed in alpha value
        /// </summary>
        /// <param name="alphaValue"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>returns type BitMap</returns>
        public Bitmap getAlphaMap(double alphaValue, int width, int height)
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
