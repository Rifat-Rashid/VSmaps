using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device;
using System.Device.Location;

namespace GaussianMapRender
{
    class BitmapCalculator
    {
        public BitmapCalculator()
        {

        }
        public double calculateDistance(double startLat, double startLng, double endLat, double endLng)
        {
            GeoCoordinate coord_1 = new GeoCoordinate(startLat, startLng);
            GeoCoordinate coord_2 = new GeoCoordinate(endLat, endLng);

            return coord_1.GetDistanceTo(coord_2);
        }
        public double calculateBitmapWidth(double distance, double maxDistance, double imageWidth)
        {
            Console.WriteLine(distance + " " + maxDistance);
            double ratio = distance / maxDistance;
            Console.WriteLine("RATIO: " + ratio);
            double width = imageWidth * ratio;
            Console.WriteLine("WIDTH OF BITMAP: " + width);
            return width;
        }
        public double calculateBitmapHeight(double distance, double maxDistance, double imageHeight)
        {
            double ratio = distance / maxDistance;
            double height = imageHeight * ratio;
            return height;
        }
    }
}
