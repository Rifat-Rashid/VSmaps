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
            this.coordinateParser = new CoordinateParser(@"C:\Users\tejas\Documents\GitHub\VSmaps\Data\lat.txt", @"C:\Users\tejas\Documents\GitHub\VSmaps\Data\long.txt");
            this.alphaValueParser = new AlphaValueParser(@"C:\Users\tejas\Documents\GitHub\VSmaps\Data\p_1.txt");
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
            /*for (int i = 0; i < longitudeValues.Count; i++)
            {
                Console.WriteLine(longitudeValues[i]);
            }*/
            //Console.WriteLine(getMax(longitudeValues));
            //Console.WriteLine("MAX >>> " + getMax(alphaValues));
            //Console.WriteLine("MIN >>> " + getMin(alphaValues));
            /*for (int i = 0; i < alphaValues.Count; i++)
            {
                alphaValues[i] = scaleAlphaValue(alphaValues[i], 0, 255, min, max);
                Console.WriteLine(alphaValues[i]);
            }
            Console.WriteLine(getMin(alphaValues) + " " + getMax(alphaValues));*/

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
		public void scale(List<double> alphaValues)
		{
            double scale = scaleAlphaValue(alphaValues);
			for (int i = 0; i < alphaValues.Count; i++)
			{
                alphaValues[i] = scale * alphaValues[i];
			}
		}
        public double scaleAlphaValue(List<double> alphaValues)
        {
            double alphaMax = 255;
            double alphaMin = 0;
            double probabilityMax = getMax(alphaValues);
            double probabilityMin = getMin(alphaValues);

            double m = (alphaMax - alphaMin) / (probabilityMax - probabilityMin);

            return m;
        }
    }
}
