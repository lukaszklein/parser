﻿using System;
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

        private void load_Click(object sender, EventArgs e)
        {
            var lines = File.ReadLines("C://Users//Łukasz//Desktop//Sem 10//magisterka//ocena.txt");
            List<int> rejectedDistortions = new List<int> { 2, 3, 5, 8, 9, 11, 13, 15, 16, 18, 19, 21, 24 };
            List<string> patternsFilters = new List<string> { "Avg", "Gauss", "Median", "Bilateral", "Kuwahara", "Unsharp", "Eq", "Str" };
            string patternImgName,
                patternDataMaskSize = @"size: [0-9,-]*",
                patternDataSigma = @"Sigma: [0-9,-]*",
                patternDataSigmaColor = @"color: [0-9,-]*",
                patternDataSigmaSpace = @"space: [0-9,-]*",
                patternDataMaskWeight = @"weight: [0-9,-]*",
                patternDataMse = @"MSE: [0-9,-]*",
                patternDataMsd = @"MSD: [0-9,-]*",
                patternDataMed = @"MED: [0-9,-]*",
                patternDataMarziliano = @"Marziliano: [0-9,-]*",
                patternDataTime = @"Time: [0-9,-]*";

            int numoftests = 2;
           
            double[] avgMse = new double[7];//[maskSize]
            double[] avgMsd = new double[7];
            double[] avgMed = new double[7];
            double[] avgMar = new double[7];
            double[] avgTime = new double[7];

            double[,] gaussMse = new double[7,6];//[maskSize][sigma]
            double[,] gaussMsd = new double[7,6];
            double[,] gaussMed = new double[7,6];
            double[,] gaussMar = new double[7,6];
            double[,] gaussTime = new double[7,6];

            double[] medianMse = new double[7];//[masksize]
            double[] medianMsd = new double[7];
            double[] medianMed = new double[7];
            double[] medianMar = new double[7];
            double[] medianTime = new double[7];

            double[,] bilateralMse = new double[6,6];//[sigmacolor][sigmaspace]
            double[,] bilateralMsd = new double[6,6];
            double[,] bilateralMed = new double[6,6];
            double[,] bilateralMar = new double[6,6];
            double[,] bilateralTime = new double[6,6];

            double[] kuwaharaMse = new double[7];//[masksize]
            double[] kuwaharaMsd = new double[7];
            double[] kuwaharaMed = new double[7];
            double[] kuwaharaMar = new double[7];
            double[] kuwaharaTime = new double[7];

            double[,] unsharpMse = new double[7,7];//[masksize][maskweight]
            double[,] unsharpMsd = new double[7,7];
            double[,] unsharpMed = new double[7,7];
            double[,] unsharpMar = new double[7,7];
            double[,] unsharpTime = new double[7,7];

            double equalizeMse = 0;
            double equalizeMsd = 0;
            double equalizeMed = 0;
            double equalizeMar = 0;
            double equalizeTime = 0;

            double stretchMse = 0;
            double stretchMsd = 0;
            double stretchMed = 0;
            double stretchMar = 0;
            double stretchTime = 0;

            double timev = 0;

            //iteracja po rodzajach zakłóceń
             for (int distortion = 1; distortion <= 1; distortion++)
             {
                Title title = chart1.Titles.Add("Ocena filtru uśredniającego, zakłócenie nr " + distortion.ToString());
                // int i = 1;
                while (rejectedDistortions.Contains(distortion))
                {
                    distortion++;
                }

                if (distortion < 10)
                {
                    patternImgName = "[iI][0-9]{2}0" + distortion.ToString();
                }
                else
                {
                    patternImgName = "[iI][0-9]{2}" + distortion.ToString();
                }
                for (int level = 1; level <= 5; level = level + 4)
                {
                    patternImgName += "_" + level.ToString();

                    foreach (var line in lines)
                    {
                        Match temp = Regex.Match(line, patternImgName);
                        
                        if (Regex.IsMatch(line, patternImgName))
                        {
                            Match mseCluttered = Regex.Match(line, patternDataMse);
                            string mse = Regex.Replace(mseCluttered.Value, "[a-zA-Z :]", "");
                            double mseValue;
                            Double.TryParse(mse, out mseValue);

                            Match msdCluttered = Regex.Match(line, patternDataMsd);
                            string msd = Regex.Replace(msdCluttered.Value, "[a-zA-Z :]", "");
                            double msdValue;
                            Double.TryParse(msd, out msdValue);

                            Match medCluttered = Regex.Match(line, patternDataMed);
                            string med = Regex.Replace(medCluttered.Value, "[a-zA-Z :]", "");
                            double medValue;
                            Double.TryParse(med, out medValue);

                            Match marCluttered = Regex.Match(line, patternDataMarziliano);
                            string mar = Regex.Replace(marCluttered.Value, "[a-zA-Z :]", "");
                            double marValue;
                            Double.TryParse(mar, out marValue);

                            Match timeCluttered = Regex.Match(line, patternDataTime);
                            string time = Regex.Replace(timeCluttered.Value, "[a-zA-Z :]", "");
                            double timeValue;
                            Double.TryParse(time, out timeValue);
                            timev += timeValue;

                            if (Regex.IsMatch(line, patternsFilters.ElementAt(0)))//Avg
                            {
                                MessageBox.Show(temp.Value);
                                Match maskSizeCluttered = Regex.Match(line, patternDataMaskSize);
                                string maskSize = Regex.Replace(maskSizeCluttered.Value, "[a-zA-Z :]", "");
                                int maskValue;
                                Int32.TryParse(maskSize, out maskValue);
                                int index = (maskValue - 1) / 2 - 1;
                                MessageBox.Show("dla distortion: " + distortion.ToString() + " level: " + level.ToString() + " mask size: " + maskValue.ToString());
                                avgMse[index] += mseValue;
                                MessageBox.Show(mseValue.ToString());
                                avgMsd[index] += msdValue;
                                MessageBox.Show(msdValue.ToString());
                                avgMed[index] += medValue;
                                MessageBox.Show(medValue.ToString());
                                avgMar[index] += marValue;
                                MessageBox.Show(marValue.ToString());
                                avgTime[index] += timeValue;
                                MessageBox.Show(timeValue.ToString());
                                MessageBox.Show("koniec");
                            }
                            /*else if (Regex.IsMatch(line, patternsFilters.ElementAt(1)))//Gauss
                            {
                                Match maskSizeCluttered = Regex.Match(line, patternDataMaskSize);
                                string maskSize = Regex.Replace(maskSizeCluttered.Value, "[a-zA-Z :]", "");
                                int maskValue;
                                Int32.TryParse(maskSize, out maskValue);
                                int index = (maskValue - 1) / 2 - 1;

                                Match sigmaCluttered = Regex.Match(line, patternDataSigma);
                                string sigma = Regex.Replace(sigmaCluttered.Value, "[a-zA-Z :]", "");
                                int sigmaValue;
                                Int32.TryParse(sigma, out sigmaValue);
                                int indexsigma = (sigmaValue - 1) / 30;

                                gaussMse[index, indexsigma] += mseValue;
                                gaussMsd[index, indexsigma] += msdValue;
                                gaussMed[index, indexsigma] += medValue;
                                gaussMar[index, indexsigma] += marValue;
                                gaussTime[index, indexsigma] += timeValue;
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(2)))//Median
                            {
                                Match maskSizeCluttered = Regex.Match(line, patternDataMaskSize);
                                string maskSize = Regex.Replace(maskSizeCluttered.Value, "[a-zA-Z :]", "");
                                int maskValue;
                                Int32.TryParse(maskSize, out maskValue);
                                int index = (maskValue - 1) / 2 - 1;

                                medianMse[index] += mseValue;
                                medianMsd[index] += msdValue;
                                medianMed[index] += medValue;
                                medianMar[index] += marValue;
                                medianTime[index] += timeValue;
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(3)))//Bilateral
                            {
                                Match sigmaColorCluttered = Regex.Match(line, patternDataSigmaColor);
                                string sigmaColor = Regex.Replace(sigmaColorCluttered.Value, "[a-zA-Z :]", "");
                                int sigmaColorValue;
                                Int32.TryParse(sigmaColor, out sigmaColorValue);
                                int indexColor = (sigmaColorValue - 2) / 30;

                                Match sigmaSpaceCluttered = Regex.Match(line, patternDataSigmaSpace);
                                string sigmaSpace = Regex.Replace(sigmaSpaceCluttered.Value, "[a-zA-Z :]", "");
                                int sigmaSpaceValue;
                                Int32.TryParse(sigmaColor, out sigmaSpaceValue);
                                int indexSpace = (sigmaSpaceValue - 2) / 30;

                                bilateralMse[indexColor, indexSpace] += mseValue;
                                bilateralMsd[indexColor, indexSpace] += msdValue;
                                bilateralMed[indexColor, indexSpace] += medValue;
                                bilateralMar[indexColor, indexSpace] += marValue;
                                bilateralTime[indexColor, indexSpace] += timeValue;
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(4)))//Avg
                            {
                                Match maskSizeCluttered = Regex.Match(line, patternDataMaskSize);
                                string maskSize = Regex.Replace(maskSizeCluttered.Value, "[a-zA-Z :]", "");
                                int maskValue;
                                Int32.TryParse(maskSize, out maskValue);
                                int index = (maskValue - 1) / 2 - 1;

                                kuwaharaMse[index] += mseValue;
                                kuwaharaMsd[index] += msdValue;
                                kuwaharaMed[index] += medValue;
                                kuwaharaMar[index] += marValue;
                                kuwaharaTime[index] += timeValue;
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(5)))//Unsharp
                            {
                                Match maskSizeCluttered = Regex.Match(line, patternDataMaskSize);
                                string maskSize = Regex.Replace(maskSizeCluttered.Value, "[a-zA-Z :]", "");
                                int maskValue;
                                Int32.TryParse(maskSize, out maskValue);
                                int index = (maskValue - 1) / 2 - 1;

                                Match maskWeightCluttered = Regex.Match(line, patternDataMaskWeight);
                                string maskWeight = Regex.Replace(maskWeightCluttered.Value, "[a-zA-Z :]", "");
                                double maskWeightValue;
                                Double.TryParse(maskWeight, out maskWeightValue);
                                double indexMaskWeight = (maskWeightValue - 0.1) / 0.5;

                                unsharpMse[index, (int)indexMaskWeight] += mseValue;
                                unsharpMsd[index, (int)indexMaskWeight] += msdValue;
                                unsharpMed[index, (int)indexMaskWeight] += medValue;
                                unsharpMar[index, (int)indexMaskWeight] += marValue;
                                unsharpTime[index, (int)indexMaskWeight] += timeValue;
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(6)))//Equalize
                            {
                                equalizeMse += mseValue;
                                equalizeMsd += msdValue;
                                equalizeMed += medValue;
                                equalizeMar += marValue;
                                equalizeTime += timeValue;
                            }
                            else if (Regex.IsMatch(line, patternsFilters.ElementAt(7)))//Stretch
                            {
                                stretchMse += mseValue;
                                stretchMsd += msdValue;
                                stretchMed += medValue;
                                stretchMar += marValue;
                                stretchTime += timeValue;
                            }*/
                        }
                    }
                    for (int j = 0; j < 7; j++)
                    {
                        MessageBox.Show("indeks: " + j.ToString());
                        avgMse[j] = avgMse[j] / numoftests;
                        MessageBox.Show(avgMse[j].ToString());
                        avgMsd[j] = avgMsd[j] / numoftests;
                        MessageBox.Show(avgMsd[j].ToString());
                        avgMed[j] = avgMed[j] / numoftests;
                        MessageBox.Show(avgMed[j].ToString());
                        avgMar[j] = avgMar[j] / numoftests;
                        MessageBox.Show(avgMar[j].ToString());
                    }

                    chart1.ChartAreas[0].AxisX.Title = "Rozmiar maski";
                    chart1.ChartAreas[0].AxisY.Title = "Wartość oceny";
                    for (int j = 0; j < 7; j++)//j - indeks, ((j + 1) * 2) + 1 - maska
                    {

                        chart1.Series["MSE lvl" + level.ToString()].Points.AddXY(((j + 1) * 2) + 1, avgMse[j]);
                        chart1.Series["MSD lvl" + level.ToString()].Points.AddXY(((j + 1) * 2) + 1, avgMsd[j]);
                        chart1.Series["MED lvl" + level.ToString()].Points.AddXY(((j + 1) * 2) + 1, avgMed[j]);
                        chart1.Series["Marziliano lvl" + level.ToString()].Points.AddXY(((j + 1) * 2) + 1, avgMar[j]);
                    }
                }
                chart1.SaveImage("C://Users//Łukasz//Desktop//Sem 10//magisterka//wykresy//Avg_" + distortion.ToString() + ".bmp", ChartImageFormat.Bmp);
            }
            timev = (timev / 1000) / 60;
            MessageBox.Show(timev.ToString());

        }
    }
}