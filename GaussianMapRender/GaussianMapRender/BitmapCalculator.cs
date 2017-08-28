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
            GeoCoordinate coord_1 = new GeoCoordinate(startLat, startLng + 90.0);
            GeoCoordinate coord_2 = new GeoCoordinate(endLat, endLng + 90.0);

            return coord_1.GetDistanceTo(coord_2);
        }
    }
}
