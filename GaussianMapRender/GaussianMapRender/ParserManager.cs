using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// manages the parsing of alpha values and gps cooridnates
namespace GaussianMapRender
{
    class ParserManager
    {
        public CoordinateParser coordinateParser;
        public AlphaValueParser alphaValueParser;

        public List<double> alphaValues;
        public List<double> latitudeValues;
        public List<double> longitudeValues;

        string RootURL = @"C:\User\DevWork\Desktop\example\";

        public ParserManager()
        {
            this.coordinateParser = new CoordinateParser(RootURL + "lat.txt", RootURL + "long.txt");
            this.alphaValueParser = new AlphaValueParser(RootURL + "p_1.txt");
        }

        public void execute()
        {
            coordinateParser.ParseFile();
            alphaValueParser.ParseFile();
            alphaValues = alphaValueParser.AlphaValueData;
            latitudeValues = coordinateParser.LatitudeCoordinates;
            longitudeValues = coordinateParser.LongitudeCoordinates;
        }
		public double getMin(List<double> a)
		{
			double min = 10;
			for (int i = 0; i < a.Count; i++)
			{
				min = (min >= a[i]) ? min : a[i];
			}
			return min;
		}
		public double getMax(List<double> a)
		{
			double max = 0;
			for (int i = 0; i < a.Count; i++)
			{
				max = (max <= a[i]) ? max : a[i];
			}
			return max;
		}

       

    }
}
