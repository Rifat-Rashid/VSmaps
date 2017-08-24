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
            this.coordinateParser = new CoordinateParser(@"C:\Users\tejas\Desktop\lat.txt", @"C:\Users\tejas\Desktop\long.txt");
            this.alphaValueParser = new AlphaValueParser(@"C:\Users\tejas\Desktop\p_1.txt");
        }

        public void execute()
        {
            coordinateParser.ParseFile();
            alphaValueParser.ParseFile();
            alphaValues = alphaValueParser.AlphaValueData;
            latitudeValues = coordinateParser.LatitudeCoordinates;
            longitudeValues = coordinateParser.LongitudeCoordinates;
            double min = getMin(alphaValues);
            double max = getMax(alphaValues);
            Console.WriteLine("MAX >>> " + getMax(alphaValues));
            Console.WriteLine("MIN >>> " + getMin(alphaValues));
            for (int i = 0; i < alphaValues.Count; i++)
            {
                alphaValues[i] = scaleAlphaValue(alphaValues[i], 0, 255, min, max);
                Console.WriteLine(alphaValues[i]);
            }
            Console.WriteLine(getMin(alphaValues) + " " + getMax(alphaValues));

            /*for(int i = 0; i < alphaValues.Count; i++)
            {
                Console.WriteLine(alphaValues[i].ToString("0.0000000"));
            }*/
        }
		public double getMin(List<double> a)
		{
			double min = 0;
			for (int i = 0; i < a.Count; i++)
			{
				min = (min >= a[i]) ? a[i] : min;
			}
			return min;
		}
		public double getMax(List<double> a)
		{
            double max = 0;
            double index = 0;
            for (int i = 0; i < a.Count; i++)
            {
                if (max < a[i])
                {
                    max = a[i];
                    index = i;
                }
            }
            return max;
		}
		public void scale(List<double> a, double min, double max)
		{
			double scale = 255.0 / max;
			for (int i = 0; i < a.Count; i++)
			{
				a[i] *= scale;
			}
		}
        public double scaleAlphaValue(double alphaValue, double minAllowed, double maxAllowed, double min, double max)
        {
            return (maxAllowed - minAllowed) * (alphaValue - min) / (max - min) + minAllowed;
        }
    }
}
