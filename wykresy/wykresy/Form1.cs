using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace wykresy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void SaveResults(string filter, string line)
        {
            if (!File.Exists(@"C://Users//Łukasz//Desktop//Sem 10//magisterka//zbiory ocen do wykresów//" + filter + ".txt"))
            {
                //File.Create(@"C://Users//Łukasz//Desktop//Sem 10//magisterka//zbiory ocen do wykresów//" + filter + ".txt");
                File.Create(@"C://Users//Łukasz//Desktop//Sem 10//magisterka//zbiory ocen do wykresów//" + filter + ".txt").Close();
            }
                TextWriter text = new StreamWriter(@"C://Users//Łukasz//Desktop//Sem 10//magisterka//zbiory ocen do wykresów//" + filter + ".txt", true);
                /*string temp = "Filter name: " + filter.ToString() + "; Mask size: " + size.ToString() + "; Sigma: " + sigma.ToString()
                      + "; Sigma color: " + sigmaColor.ToString() + "; Sigma space: " + sigmaSpace.ToString()
                      + "; Mask weight: " + weight.ToString() + "; MSE: " + data[6].ToString() + "; MSD: " + data[7].ToString() + "; MED: " + data[8].ToString()
                       + "; Marziliano: " + data[9].ToString() + "; Time: " + data[11].ToString();
                temp = temp + "\r\n";*/
                text.Write(line + "\r\n");
                text.Close();
        }

        List<int> rejectedDistortions = new List<int> { 2, 3, 5, 8, 9, 11, 13, 15, 16, 17, 19, 21, 24 };
        List<string> patternsFilters = new List<string> { "Avg", "Gauss", "Median", "Bilateral", "Kuwahara", "Unsharp", "Eq", "Str" };
        List<int> gauss = new List<int> { 1, 6, 11, 16, 21, 26, 31, 36, 41 };
        List<int> bilateral = new List<int> { 2, 12, 22, 32, 42, 52, 62 };
        List<double> unsharp = new List<double> { 0.1, 0.6, 1.1, 1.6, 2.1, 2.6 };
        string patternImgName = @"[iI][0-9]{2}", patternImgDist, patternImgLvl,
            patternDataMaskSize = @"size: ",
            patternDataSigma = @"Sigma: ",
            patternDataSigmaColor = @"color: ",
            patternDataSigmaSpace = @"space: [0-9,-]*",
            patternDataMaskWeight = @"weight: ",
            patternDataMse = @"MSE: [0-9,-]*",
            patternDataMsd = @"MSD: [0-9,-]*",
            patternDataMed = @"MED: [0-9,-]*",
            patternDataMarziliano = @"Marziliano: [0-9,-]*",
            patternDataTime = @"Time: [0-9,-]*";

        private void prepareref_Click(object sender, EventArgs e)
        {
            var lines = File.ReadLines(@"C:\obrazy\ocenaref.txt");

            for (int distortion = 1; distortion <= 23; distortion++)
            {
                while (rejectedDistortions.Contains(distortion))
                {
                    distortion++;
                }
                string name;

                if (distortion < 10)
                {
                    patternImgDist = patternImgName + "0" + distortion.ToString();
                    name = "0" + distortion.ToString();
                }
                else
                {
                    patternImgDist = patternImgName + distortion.ToString();
                    name = distortion.ToString();
                }
                for (int level = 1; level <= 5; level = level + 4)
                {
                    string temp = name;
                    msetofile = 0;
                    msdtofile = 0;
                    medtofile = 0;
                    martofile = 0;
                    timetofile = 0;
                    linesdetected = 0;

                    patternImgLvl = patternImgDist + "_" + level.ToString();
                    temp += "_" + level.ToString();
                    foreach (var line in lines)
                    {
                        if (Regex.IsMatch(line, patternImgLvl))
                        {
                            linesdetected++;
                            Match mseCluttered = Regex.Match(line, patternDataMse);
                            string mse = Regex.Replace(mseCluttered.Value, "[a-zA-Z :]", "");
                            double mseValue;
                            Double.TryParse(mse, out mseValue);
                            msetofile += mseValue;

                            Match msdCluttered = Regex.Match(line, patternDataMsd);
                            string msd = Regex.Replace(msdCluttered.Value, "[a-zA-Z :]", "");
                            double msdValue;
                            Double.TryParse(msd, out msdValue);
                            msdtofile += msdValue;

                            Match medCluttered = Regex.Match(line, patternDataMed);
                            string med = Regex.Replace(medCluttered.Value, "[a-zA-Z :]", "");
                            double medValue;
                            Double.TryParse(med, out medValue);
                            medtofile += medValue;

                            Match marCluttered = Regex.Match(line, patternDataMarziliano);
                            string mar = Regex.Replace(marCluttered.Value, "[a-zA-Z :]", "");
                            double marValue;
                            Double.TryParse(mar, out marValue);
                            martofile += marValue;
                        }
                    }
                    msetofile = msetofile / linesdetected;
                    msdtofile = msdtofile / linesdetected;
                    medtofile = medtofile / linesdetected;
                    martofile = martofile / linesdetected;

                    string newmsetofile = msetofile.ToString().Replace(',', '.');
                    string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                    string newmedtofile = medtofile.ToString().Replace(',', '.');
                    string newmartofile = martofile.ToString().Replace(',', '.');
                    AddToRefFile(temp, newmsetofile, newmsdtofile, newmedtofile, newmartofile);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var lines = File.ReadLines(@"C:\Users\Łukasz\Desktop\Sem 10\magisterka\przetworzone oceny\reference.txt");
            string pattern = "([0-9]{2})_";

            foreach (var line in lines)
            {
                string lvl = "";
                if (Regex.IsMatch(line, "_1"))
                {
                    lvl = "1";
                }
                else if (Regex.IsMatch(line, "_5"))
                {
                    lvl = "5";
                }

                Match patternvalueCluttered = Regex.Match(line, pattern);
                string distnum = Regex.Replace(patternvalueCluttered.Value, "[a-zA-Z ;_]", "");
                double distnumValue;
                Double.TryParse(distnum, out distnumValue);

                Match mseCluttered = Regex.Match(line, @"MSE: [0-9,-.]*");
                string mse = Regex.Replace(mseCluttered.Value, "[a-zA-Z :]", "");

                Match msdCluttered = Regex.Match(line, @"MSD: [0-9,-.]*");
                string msd = Regex.Replace(msdCluttered.Value, "[a-zA-Z :]", "");

                Match medCluttered = Regex.Match(line, @"MED: [0-9,-.]*");
                string med = Regex.Replace(medCluttered.Value, "[a-zA-Z :]", "");

                Match marCluttered = Regex.Match(line, @"Marziliano: [0-9,-.]*");
                string mar = Regex.Replace(marCluttered.Value, "[a-zA-Z :]", "");

                TextWriter text = new StreamWriter(@"C:\Users\Łukasz\Desktop\Sem 10\magisterka\przetworzone oceny\ref"+distnumValue+".txt", true);
                text.Write(mse + " title \'MSE lvl"+lvl+" odniesienie\', \\\r\n" + msd + " title \'MSD lvl" + lvl + " odniesienie\', \\\r\n" + med +
                    " title \'MED lvl" + lvl + " odniesienie\', \\\r\n" + mar + " title \'Marziliano lvl" + lvl + " odniesienie\', \\\r\n");
                text.Close();
            }         
        }

        private void delete_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\Users\Łukasz\Documents\Zwykresy\Bilateral");

            foreach (FileInfo file in di.GetFiles())
            {
                if (!Regex.IsMatch(file.Name, @"2.p"))
                {
                    file.Delete();
                }
            }

            di = new DirectoryInfo(@"C:\Users\Łukasz\Documents\Zwykresy\Gauss");

            foreach (FileInfo file in di.GetFiles())
            {
                if (!Regex.IsMatch(file.Name, @"1.p") && !Regex.IsMatch(file.Name, @"6.p"))
                {
                    file.Delete();
                }
            }

            di = new DirectoryInfo(@"C:\Users\Łukasz\Documents\Zwykresy\Wyostrzanie");

            foreach (FileInfo file in di.GetFiles())
            {
                if (!Regex.IsMatch(file.Name, @"1.p") && !Regex.IsMatch(file.Name, @"6.p"))
                {
                    file.Delete();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] allfiles = Directory.GetFiles("C://Users//Łukasz//Desktop//Sem 10//magisterka//zbiory ocen do wykresów//");
            foreach (var path in allfiles)
            {
                var lines = File.ReadLines(path);
                Match distnumCluttered = Regex.Match(path, "I [0-9]*;");
                string distnum = Regex.Replace(distnumCluttered.Value, "[a-zA-Z ;]", "");
                double distnumValue;
                Double.TryParse(distnum, out distnumValue);

                if (Regex.IsMatch(path, "Avg"))
                {
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Avg" + distnumValue.ToString() + "Lvl" + i.ToString();
                        CreateFile(name);


                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Median"))
                {
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Median" + distnumValue.ToString() + "Lvl" + i.ToString();
                        CreateFile(name);


                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Kuwahara"))
                {
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Kuwahara" + distnumValue.ToString() + "Lvl" + i.ToString();
                        CreateFile(name);


                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Gauss"))
                {
                    Match whatsigma = Regex.Match(path, " [0-9]*.txt");
                    string sigma = Regex.Replace(whatsigma.Value, "[a-zA-Z :.]", "");
                    double sigmavalue;
                    Double.TryParse(sigma, out sigmavalue);
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Gauss" + distnumValue.ToString() + "Lvl" + i.ToString() + " Sigma" + sigmavalue.ToString();
                        CreateFile(name);

                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Unsharp"))
                {
                    Match whatweight = Regex.Match(path, " [0-9,]*.txt");
                    string weight = Regex.Replace(whatweight.Value, "[a-zA-Z :.]", "");
                    double weightvalue;
                    Double.TryParse(weight, out weightvalue);
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Unsharp" + distnumValue.ToString() + "Lvl" + i.ToString() + " Weight" + weightvalue.ToString();
                        CreateFile(name);

                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Bilateral"))
                {
                    Match whatcolor = Regex.Match(path, " [0-9]*.txt");
                    string color = Regex.Replace(whatcolor.Value, "[a-zA-Z :.]", "");
                    double colorvalue;
                    Double.TryParse(color, out colorvalue);
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Bilateral" + distnumValue.ToString() + "Lvl" + i.ToString() + " Color" + colorvalue.ToString();
                        CreateFile(name);

                        for (int j = 2; j <= 62; j = j + 10)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataSigmaSpace + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
            }
        }

        void AddToRefFile(string name, string mse, string msd, string med, string mar)
        {
            TextWriter text = new StreamWriter(@"C:\Users\Łukasz\Desktop\Sem 10\magisterka\przetworzone oceny\reference.txt", true);
            text.Write(name + " MSE: " + mse + " MSD: " + msd + " MED: " + med + " Marziliano: " + mar + "\r\n");
            text.Close();
        }

        private void createSubset_Click(object sender, EventArgs e)
        {
            var lines = File.ReadLines("C://Users//Łukasz//Desktop//Sem 10//magisterka//ocena.txt");            

            for (int distortion = 1; distortion <= 23; distortion++)
            {
                while (rejectedDistortions.Contains(distortion))
                {
                    distortion++;
                }

                if (distortion < 10)
                {
                    patternImgDist = patternImgName + "0" + distortion.ToString();
                }
                else
                {
                    patternImgDist = patternImgName + distortion.ToString();
                }
                for (int level = 1; level <= 5; level = level + 4)
                {
                    patternImgLvl = patternImgDist + "_" + level.ToString();
                    foreach (var line in lines)
                    {
                        if (Regex.IsMatch(line, patternImgLvl))
                        {
                            if (Regex.IsMatch(line, patternsFilters.ElementAt(0)))//Avg
                            {
                                SaveResults("I "+ distortion.ToString() + ";" +patternsFilters.ElementAt(0), line);
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(1)))//gauss
                            {
                                foreach (var item in gauss)
                                {
                                    string gausssigma = patternDataSigma + item.ToString() + ";";
                                    if (Regex.IsMatch(line, gausssigma))
                                    {
                                        SaveResults("I " + distortion.ToString() + ";" + patternsFilters.ElementAt(1) + " Sigma " + item.ToString(), line);
                                    }
                                } 
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(2)))//Median
                            {
                                SaveResults("I " + distortion.ToString() + ";" + patternsFilters.ElementAt(2), line);
                            }
                           else  if (Regex.IsMatch(line, patternsFilters.ElementAt(3)))//bilateral
                            {
                                foreach (var item in bilateral)
                                {
                                    string bilateralcolor = patternDataSigmaColor + item.ToString() + ";";
                                    if (Regex.IsMatch(line, bilateralcolor))
                                    {
                                        SaveResults("I " + distortion.ToString() + ";" + patternsFilters.ElementAt(3) + " Color " + item.ToString(), line);
                                    }
                                }
                            }
                           else  if (Regex.IsMatch(line, patternsFilters.ElementAt(4)))//kuwahara
                            {
                                SaveResults("I " + distortion.ToString() + ";" + patternsFilters.ElementAt(4), line);
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(5)))//unsharp
                            {
                                foreach (var item in unsharp)
                                {
                                    string unsharpWeight = patternDataMaskWeight + item.ToString() + ";";
                                    if (Regex.IsMatch(line,unsharpWeight))
                                    {
                                        SaveResults("I " + distortion.ToString() + ";" + patternsFilters.ElementAt(5) + " Weight " + item.ToString(), line);
                                    }
                                }
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(6)))//equalize
                            {
                                SaveResults("I " + distortion.ToString() + ";" + patternsFilters.ElementAt(6), line);
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(7)))//stretch
                            {
                                SaveResults("I " + distortion.ToString() + ";" + patternsFilters.ElementAt(7), line);
                            }
                        }
                    }
                }
            }
        }

        void CreateFile(string name)
        {
            if (!File.Exists("C://Users//Łukasz//Desktop//Sem 10//magisterka//przetworzne oceny//" + name + ".txt"))
            {
                File.Create(@"C:\Users\Łukasz\Desktop\Sem 10\magisterka\przetworzone oceny\" + name + ".txt").Close();
            }
            TextWriter text = new StreamWriter(@"C:\Users\Łukasz\Desktop\Sem 10\magisterka\przetworzone oceny\" + name + ".txt", true);
            text.Write("#" + name + "\r\n#First set dist level = 1\r\n#Second set dist level = 5\r\n#Mask_size MSE MSE MED MAR TIME");
            text.Close();
        }

        void AddToFile(string name, int size, string mse, string msd, string med, string mar, string time)
        {
            TextWriter text = new StreamWriter(@"C:\Users\Łukasz\Desktop\Sem 10\magisterka\przetworzone oceny\" + name + ".txt", true);
            text.Write("\r\n" + size.ToString() + " " + mse.ToString() + " " + msd.ToString() + " " + med.ToString() + " " + mar.ToString() + " " + time.ToString());
            text.Close();
        }

        double msetofile = 0;
        double msdtofile = 0;
        double medtofile = 0;
        double martofile = 0;
        double timetofile = 0;
        int linesdetected = 0;

        void Increment(string line)
        {
            linesdetected++;
            Match mseCluttered = Regex.Match(line, patternDataMse);
            string mse = Regex.Replace(mseCluttered.Value, "[a-zA-Z :]", "");
            double mseValue;
            Double.TryParse(mse, out mseValue);
            msetofile += mseValue;

            Match msdCluttered = Regex.Match(line, patternDataMsd);
            string msd = Regex.Replace(msdCluttered.Value, "[a-zA-Z :]", "");
            double msdValue;
            Double.TryParse(msd, out msdValue);
            msdtofile += msdValue;

            Match medCluttered = Regex.Match(line, patternDataMed);
            string med = Regex.Replace(medCluttered.Value, "[a-zA-Z :]", "");
            double medValue;
            Double.TryParse(med, out medValue);
            medtofile += medValue;

            Match marCluttered = Regex.Match(line, patternDataMarziliano);
            string mar = Regex.Replace(marCluttered.Value, "[a-zA-Z :]", "");
            double marValue;
            Double.TryParse(mar, out marValue);
            martofile += marValue;

            Match timeCluttered = Regex.Match(line, patternDataTime);
            string time = Regex.Replace(timeCluttered.Value, "[a-zA-Z :]", "");
            double timeValue;
            Double.TryParse(time, out timeValue);
            timetofile += timeValue;
        }

        private void gnucompile_Click(object sender, EventArgs e)
        {
            string[] allfiles = Directory.GetFiles("C://Users//Łukasz//Desktop//Sem 10//magisterka//zbiory ocen do wykresów//");
            foreach (var path in allfiles)
            {
                var lines = File.ReadLines(path);
                Match distnumCluttered = Regex.Match(path, "I [0-9]*;");
                string distnum = Regex.Replace(distnumCluttered.Value, "[a-zA-Z ;]", "");
                double distnumValue;
                Double.TryParse(distnum, out distnumValue);

                if (Regex.IsMatch(path, "Avg"))
                {
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Avg" + distnumValue.ToString() + "Lvl" + i.ToString();
                        CreateFile(name);

                    
                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Median"))
                {
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Median" + distnumValue.ToString() + "Lvl" + i.ToString();
                        CreateFile(name);

                    
                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Kuwahara"))
                {
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Kuwahara" + distnumValue.ToString() + "Lvl" + i.ToString();
                        CreateFile(name);

                    
                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Gauss"))
                {
                    Match whatsigma = Regex.Match(path, " [0-9]*.txt");
                    string sigma = Regex.Replace(whatsigma.Value, "[a-zA-Z :.]", "");
                    double sigmavalue;
                    Double.TryParse(sigma, out sigmavalue);
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Gauss" + distnumValue.ToString() + "Lvl" + i.ToString() + " Sigma" + sigmavalue.ToString();
                        CreateFile(name);
                    
                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Unsharp"))
                {
                    Match whatweight = Regex.Match(path, " [0-9,]*.txt");
                    string weight = Regex.Replace(whatweight.Value, "[a-zA-Z :.]", "");
                    double weightvalue;
                    Double.TryParse(weight, out weightvalue);
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Unsharp" + distnumValue.ToString() + "Lvl" + i.ToString() + " Weight" + weightvalue.ToString();
                        CreateFile(name);
                    
                        for (int j = 3; j <= 15; j = j + 2)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataMaskSize + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
                else if (Regex.IsMatch(path, "Bilateral"))
                {
                    Match whatcolor = Regex.Match(path, " [0-9]*.txt");
                    string color = Regex.Replace(whatcolor.Value, "[a-zA-Z :.]", "");
                    double colorvalue;
                    Double.TryParse(color, out colorvalue);
                    for (int i = 1; i <= 5; i = i + 4)
                    {
                        string name = "Bilateral" + distnumValue.ToString() + "Lvl" + i.ToString() + " Color" + colorvalue.ToString();
                        CreateFile(name);
                    
                        for (int j = 2; j <= 62; j = j + 10)
                        {
                            msetofile = 0;
                            msdtofile = 0;
                            medtofile = 0;
                            martofile = 0;
                            timetofile = 0;
                            linesdetected = 0;

                            foreach (var line in lines)
                            {
                                if (Regex.IsMatch(line, patternDataSigmaSpace + j.ToString()) && Regex.IsMatch(line, "_" + i.ToString()))
                                {
                                    Increment(line);
                                }
                            }
                            msetofile = msetofile / linesdetected;
                            msdtofile = msdtofile / linesdetected;
                            medtofile = medtofile / linesdetected;
                            martofile = martofile / linesdetected;
                            timetofile = timetofile / linesdetected;

                            string newmsetofile = msetofile.ToString().Replace(',', '.');
                            string newmsdtofile = msdtofile.ToString().Replace(',', '.');
                            string newmedtofile = medtofile.ToString().Replace(',', '.');
                            string newmartofile = martofile.ToString().Replace(',', '.');
                            string newtimetofile = timetofile.ToString().Replace(',', '.');
                            AddToFile(name, j, newmsetofile, newmsdtofile, newmedtofile, newmartofile, newtimetofile);
                        }
                    }
                }
            }
        }
    }
}
