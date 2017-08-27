using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianMapRender
{
    class BitmapCalculator
    {
        private double calculateDistance(double startLat, double startLng, double endLat, double endLng)
        {
            double radius = 6371;
            double dLat = toRadian(endLat - startLat);
            double dLng = toRadian(endLng - startLng);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(toRadian(startLat)) * Math.Cos(toRadian(endLat)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = radius * c * 1000;
            return d;
        }
        private double toRadian(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
