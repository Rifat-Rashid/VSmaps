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

        public ArrayList alphaValues;
        public ArrayList latitudeValues;
        public ArrayList longitudeValues;

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

       

    }
}
