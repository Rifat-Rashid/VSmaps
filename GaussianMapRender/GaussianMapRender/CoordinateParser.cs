using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianMapRender
{
    class CoordinateParser
    {
        private String LatitudeFilePath;
        private String LongitudeFilePath;

        public ArrayList LatitudeCoordinates { get; }
        public ArrayList LongitudeCoordinates { get; }

        public CoordinateParser() { }

        /// <param name="latitudeFilePath">path to lat text</param>
        /// <param name="longitudeFilePath">path to long text</param>
        public CoordinateParser(String latitudeFilePath, String longitudeFilePath)
        {
            this.LatitudeFilePath = latitudeFilePath;
            this.LongitudeFilePath = longitudeFilePath;

            LatitudeCoordinates = new ArrayList();
            LongitudeCoordinates = new ArrayList();
        }

        // Parse text file for latitude & longitude coordinates
        public void ParseFile()
        {
            string latitudeData = System.IO.File.ReadAllText(LatitudeFilePath);
            string longitudeData = System.IO.File.ReadAllText(LongitudeFilePath);

            string[] tempLat = latitudeData.Split(',');
            string[] tempLng = longitudeData.Split(',');

            for(int i = 0; i < tempLat.Length; i++)
            {
                LatitudeCoordinates.Add(Double.Parse(tempLat[i]));
            }

            for(int j = 0; j < tempLng.Length; j++)
            {
                LongitudeCoordinates.Add(Double.Parse(tempLng[j]));
            }
        }

        // print Latitude and Longitude coordinates
        public void PrintCoordinates()
        {
            int counter1 = 0;
            int counter2 = 0;
            foreach(Object o in LatitudeCoordinates)
            {
                Console.WriteLine(o);
                counter1++;
            }

            foreach(Object o in LongitudeCoordinates)
            {
                Console.WriteLine(o);
                counter2++;
            }
            Console.WriteLine("LATITUDE COORDINATES >>> " + counter1);
            Console.WriteLine("LONGITUDE COORDINATES >>> " + counter2);

        }

        public ArrayList getLatitudeCoordinates()
        {
            return LatitudeCoordinates;
        }

        public ArrayList getLongitudeCoordinates()
        {
            return LongitudeCoordinates;
        }
    }
}
