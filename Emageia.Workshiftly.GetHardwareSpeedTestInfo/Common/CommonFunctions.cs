using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Common
{
    public class CommonFunctions
    {
        public static void LogWriteLines(string fuction)
        {
            try
            {
                var paths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var subFolderPath = System.IO.Path.Combine(paths, "Workshiftly Client");
                var subFolder = System.IO.Path.Combine(subFolderPath, "NewFeature.txt");

                string Date = "\n \n" + DateTime.Now.ToString();

                //string name = string.Format("{0,20}{1,8}{2,18}{3,30}{4,26}",
                //        "Date", "Type", "Fuction", "Result", "Error Description");
                string name2 = string.Format("{0,20}{1,20}",
                        Date, fuction);

                string[] line = new string[] { name2 };

                File.AppendAllLines(subFolder, line);



                //Console.WriteLine("{0,26}{1,8}{2,26}",
                //        "Argument", "Digits", "Result");


            }
            catch (Exception ex)
            {

            }
        }
    }
}
