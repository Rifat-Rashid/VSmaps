using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianMapRender
{
    class AlphaValueParser
    {
        private String FilePath;
        private System.IO.StreamReader FileReader;
        public List<double> AlphaValueData { get; }

        public AlphaValueParser() { }

        /// <param name="filePath">File path of alpha values</param>
        public AlphaValueParser(String filePath)
        {
            this.FilePath = filePath;
            this.FileReader = new System.IO.StreamReader(FilePath);
            AlphaValueData = new List<double>();
        }

        public void ParseFile()
        {
            //int LineNumber = 0;
            String Line;
            int Counter = 0;
            while((Line = FileReader.ReadLine()) != null)
            {
                string[] SplicedValues = Line.Split(',');
                for(int i = 0; i < SplicedValues.Length; i++)
                {
                    decimal CurrentAlphaValue= decimal.Parse(SplicedValues[i], System.Globalization.NumberStyles.Float);
                    AlphaValueData.Add((double)CurrentAlphaValue);
                }
                Counter++;
            }
            Console.WriteLine("LINE COUNT >> " + Counter);
        }

        public void PrintAlphaValues()
        {
            foreach(Object o in AlphaValueData)
            {
                Console.WriteLine(o);
            }
        }
    }


}
