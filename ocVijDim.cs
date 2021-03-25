using System; 
using System.Threading;
using System.IO;
using System.Linq;

namespace jzfLib
{
    internal class DimenzionirajOcniVijak
    {
        static int Force;
        static string Material;
        static string mat1 = "CV1A";
        static string mat2 = "CV1B";
        static string mat3 = "CV2A";
        static string mat4 = "CV2B";
        static int[] SigmaEE = { 130, 350 };
        private static int getParasF()
        {
            Console.WriteLine("Get Force: ");
            Force = Convert.ToInt32(Console.ReadLine());
            return Force;
        }
        private static string getParasM()
        {
            Console.WriteLine("Get Material: ");
            Material = Console.ReadLine();
            Console.WriteLine("SigmaR: " + getFromClass(Material)[0] + " MPa");
            Console.WriteLine("SigmaL: " + getFromClass(Material)[1] + " MPa");
            return Material;
        }


        public static int[] getFromClass(string Material)
        {
            Material =  Material.ToUpper();
            int switchint = 0;
            int SigmaR = 1;
            int SigmaL = 1;
            int SigmavAllowed = 100;
            int SigmaAllowed = 1;
            int Pd = 1;
            if (Material == mat1)
            {
                switchint = 1;
            }
            else if (Material == mat2)
            {
                switchint = 2;
            }
            else if (Material == mat3)
            {
                switchint = 3;
            }
            else if (Material == mat4)
            {
                switchint = 4;
            }
            switch(switchint)
            {
                default:
                    break;
                case 1:
                    SigmaR = 210;
                    SigmaL = 370;
                    break;
                case 2:
                    SigmaR = 210;
                    SigmaL = 370;
                    break;
                case 3:
                    SigmaR = 280;
                    SigmaL = 500;
                    break;
                case 4:
                    SigmaR = 400;
                    SigmaL = 500;
                    break;

            }
            if((SigmaL >= 300 && SigmaL < 500))
            {
                SigmavAllowed = 12000 / 100;
                SigmaAllowed = 12000 / 100;
                Pd = 9000 / 100;
            }
            else if((SigmaL >= 500 && SigmaL < 700))
            {
                SigmavAllowed = 15000 / 100;
                SigmaAllowed = 15000 / 100;
                Pd = 12500 / 100;
            }
            int[] Sigma = { SigmaR, SigmaL,SigmavAllowed,SigmaAllowed, Pd };
            return Sigma;
        }
        private static int izrada()
        {
            Console.WriteLine("Kakva je izrada ?:\nGruba Izrada (1)\nNormalna Izrada (2)\nFina Izrada (3)");
            int izrada = Convert.ToInt32(Console.ReadLine());
            switch(izrada)
            {
                case 1:
                    Console.WriteLine("\nIzrada je Gruba.");
                    break;
                case 2:
                    Console.WriteLine("\nIzrada je Normalna.");
                    break;
                case 3:
                    Console.WriteLine("\nIzrada je Fina.");
                    break;
            }
            return izrada;
        }
        private static float[] dimension(int Force,int[] Sigmas, int izrada)
        {
            float Mi = 1f;
            float d = 0f;
            double d2 = 0f;
            float m;
            switch(izrada)
            {

                case 1:
                    Mi = 0.7f;
                break;

                 case 2:
                    Mi = 0.8f;
                break;
                case 3:
                    Mi = 0.9f;
                    
                    break;
            }
            float SigmaL = Sigmas[1];
            float SigmaR = Sigmas[0];
            float Pd = Sigmas[4];
            float Pdd = (Pd * Mi);
            Console.WriteLine("Povrsinski pritisak sa oslabljenjem: " + Pdd);
            float Sd = Sigmas[3];
            float Sdd = (Sd * Mi);
            Console.WriteLine("Dopuseno naprezanje sa oslabljenjem: " + Sdd);
            float Svd = Sigmas[2];
            Console.WriteLine("\n" + Force + " / " + Sdd);
            float Area = Force / Sdd;
            Console.WriteLine("\nPovrsina presjeka je: " + Area);
            //Console.WriteLine("Str(140)/Tablica 71, pronadji nazivni promjer (M)\nUpisi odabrani nazivni promjer: ");
            //int M = Convert.ToInt32(Console.ReadLine());
            float[] Ms =
                {
                0,
                16,
                20,
                24,
                30,
                36,
                42,
                48,
                56,
                64
            };
            double[] Areas =
                {
                0,//0
                144,//16
                225,//20
                324,//24
                519,//30
                759,//36
                1045,//42
                1375,//48
                1905,//56
                2519,//64
            };
            double[] diameter2 =
                {
                0,
                14.701,//16
                18.376,//20
                22.051,//24
                27.727,//30
                33.402,//36
                39.077,//42
                44.752,//48
                52.428,//56
                60.103,//64
            };

            float coeficient = 200;
            float closestArea = (float)Areas.OrderBy(v => Math.Abs((long)v - (Area + coeficient))).First();
            Console.WriteLine("\nNajblizi M je: " + closestArea + "\nIz Tablice 71/140");
            int index = Array.IndexOf(Areas, closestArea);
            d = Ms[index];Console.WriteLine($"d: {d}");
            d2 = diameter2[index];Console.WriteLine($"d2: {d2}");
            m = 0.8f * d;Console.WriteLine($"m: {m}");
            double actualArea = 2 * d2 * m;Console.WriteLine($"2 * {d2} * {m} = {actualArea}");
            float actualPn = (float)(Force / actualArea);Console.WriteLine($"{Force} / {actualArea} =  {actualPn}");
            
            bool bad = false;
            if(actualPn <= Pdd)
            {
                bad = false;
                Console.WriteLine("Povrsinski tlak zadovoljava. ( " + actualPn + " )");
            }
            else
            {
                bad = true;
                Console.WriteLine("Povrsinski tlak previsok, povecanje povrsine potrebno.");
                while (bad)
                {
                    coeficient += 200;
                    closestArea = (float)Areas.OrderBy(v => Math.Abs((long)v - (Area + coeficient))).First();
                    Console.WriteLine("\nNajblizi M je: " + closestArea + "\nIz Tablice 71/140");

                }
            }
            float[] izracun =
                {
                closestArea,
                actualPn,


            };

            return izracun;
        }
        static void Main(string[] args)
        {
            int Force = getParasF() * 1000;
            string Material = getParasM();
            dimension(Force, getFromClass(Material), izrada());

        }
    }
}
