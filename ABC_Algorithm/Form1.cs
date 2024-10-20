using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ABC_Algorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Başlatma Butonu
        private void baslat_Click(object sender, EventArgs e)
        {
            chart1.Series["Convergence"].Points.Clear();
            int colonySize, iteration, limit, foodNumber, employeeBee;
            colonySize = int.Parse(textBox1.Text);
            iteration = int.Parse(textBox2.Text);
            limit = int.Parse(textBox3.Text);
            employeeBee = colonySize / 2;
            foodNumber = employeeBee;
            Random random = new Random();
            double lowestX1 = 0, lowestX2 = 0, lowestResult = 0, lowestFitness = 0;
            List<double> convergenceValues = new List<double>();

            for (int i = 0; i < iteration; i++)
            {
                List<double> x1Numbers = GenerateRandomNumbers(foodNumber, -2, 2, 4, random);
                List<double> x2Numbers = GenerateRandomNumbers(foodNumber, -2, 2, 4, random);
                List<double> results = CalculateFunction(x1Numbers, x2Numbers);
                List<double> fitness = CalculateFitness(results);
                EmployeeBee(foodNumber, x1Numbers, x2Numbers, results, fitness);
                List<double> probabilities = Probabilities(foodNumber, fitness);
                OnlookerBee(foodNumber, x1Numbers, x2Numbers, results, fitness, probabilities);          
                ScoutBee(limit, foodNumber, x1Numbers, x2Numbers, results, fitness);
                int lowestValueIndex = results.IndexOf(results.Min());
                if (fitness[lowestValueIndex] > lowestFitness)
                {
                    lowestX1 = x1Numbers[lowestValueIndex];
                    lowestX2 = x2Numbers[lowestValueIndex];
                    lowestResult = results[lowestValueIndex];
                    lowestFitness = fitness[lowestValueIndex];
                    convergenceValues.Add(lowestResult);
                }
            }
            WriteValueToLabel(label5, lowestX1.ToString());
            WriteValueToLabel(label6, lowestX2.ToString());
            WriteValueToLabel(label7, lowestResult.ToString());
            WriteValueToLabel(label8, lowestFitness.ToString());
            for (int i = 0; i < convergenceValues.Count; i++)
            {
                chart1.Series["Convergence"].Points.AddY(convergenceValues[i]);
            }
            chart1.Series["Convergence"].ChartType = SeriesChartType.Line; // Çizgi grafiği
            chart1.ChartAreas[0].AxisX.Title = "Iteration"; // X eksen başlığı
            chart1.ChartAreas[0].AxisY.Title = "Convergence Value"; // Y eksen başlığı
            chart1.ChartAreas[0].AxisX.Minimum = 1; // Minimum iterasyon sayısını 1 olarak ayarlayın
            chart1.ChartAreas[0].AxisX.Maximum = iteration;
            chart1.ChartAreas[0].AxisY.Minimum = 1;
            chart1.ChartAreas[0].AxisY.Maximum = 10;
            chart1.ChartAreas[0].AxisY.Interval = 1;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
        }

        //Rastgele x1 ve x2 oluşturma
        private static List<double> GenerateRandomNumbers(int count, double minValue, double maxValue, int decimalPlaces, Random random)
        {
            List<double> numbers = new List<double>();

            for (int i = 0; i < count; i++)
            {
                double randomValue = Math.Round(minValue + (maxValue - minValue) * random.NextDouble(), decimalPlaces);
                numbers.Add(randomValue);
            }
            return numbers;
        }

        //f(x1,x2) değerlerini bulma
        private List<double> CalculateFunction(List<double> x1, List<double> x2)
        {
            List<double> results = new List<double>();

            for (int i = 0; i < x1.Count; i++)
            {
                double result = Function(x1[i], x2[i]);
                results.Add(result);
            }

            return results;
        }

        //f fonksiyonu
        private static double Function(double x1, double x2)
        {
            double part1 = 1 + Math.Pow((x1 + x2 + 1), 2) * (19 - 14 * x1 + 3 * Math.Pow(x1, 2) - 14 * x2 + 6 * x1 * x2 + 3 * Math.Pow(x2, 2));
            double part2 = 30 + Math.Pow((2 * x1 - 3 * x2), 2) * (18 - 32 * x1 + 12 * Math.Pow(x1, 2) + 48 * x2 - 36 * x1 * x2 + 27 * Math.Pow(x2, 2));
            return Math.Round(part1 * part2, 4);
        }

        //Fitness Hesaplama
        private static List<double> CalculateFitness(List<double> results)
        {
            List<double> fitnessValues = new List<double>();

            foreach (var result in results)
            {
                double fitness;
                if (result >= 0)
                {
                    fitness = 1.0 / (1.0 + result);
                }
                else
                {
                    fitness = 1.0 + Math.Abs(result);
                }
                fitnessValues.Add(Math.Round(fitness, 4));
            }

            return fitnessValues;
        }

        //İşçi Arı Fazı

        public static void EmployeeBee(int foodNumber, List<double> x1Numbers, List<double> x2Numbers, List<double> results, List<double> fitness)
        {
            Random random = new Random();

            for (int i = 0; i < foodNumber; i++)
            {
                int k = random.Next(0, foodNumber); // 0 ile foodNumber arasında bir integer k
                int j = random.Next(0, 2); // 0 ya da 1 olacak bir integer j
                double fi = random.NextDouble() * 2 - 1; // -1 ile 1 arasında ondalıklı kısım maksimum 4 basamak olan bir double fi
                fi = Math.Round(fi, 4);

                double x, x2, V;

                if (j == 0)
                {
                    x = x1Numbers[i];
                    x2 = x1Numbers[k];
                    V = x + fi * (x - x2);
                    double newFunctionValue = Function(Math.Round(V, 4), x2Numbers[i]);
                    double newFitnessValue = CalculateFitness(new List<double> { newFunctionValue })[0];
                    if (newFitnessValue > fitness[i])
                    {
                        x1Numbers[i] = Math.Round(V, 4);
                        results[i] = Math.Round(newFunctionValue, 4);
                        fitness[i] = Math.Round(newFitnessValue, 4);
                    }
                }
                else
                {
                    x = x2Numbers[i];
                    x2 = x2Numbers[k];
                    V = x + fi * (x - x2);
                    double newFunctionValue = Function(x1Numbers[i], Math.Round(V, 4));
                    double newFitnessValue = CalculateFitness(new List<double> { newFunctionValue })[0];
                    if (newFitnessValue > fitness[i])
                    {
                        x2Numbers[i] = Math.Round(V, 4);
                        results[i] = Math.Round(newFunctionValue, 4);
                        fitness[i] = Math.Round(newFitnessValue, 4);
                    }
                }
            }
        }

        //Olasılık Hesaplama

        public static List<double> Probabilities(int foodNumber, List<double> fitness)
        {
            List<double> probabilities = new List<double>();
            double sumFitness = 0.0;
     
            for (int i = 0; i < foodNumber; i++)
            {
                sumFitness += fitness[i];
            }

            for (int i = 0; i < foodNumber; i++)
            {
                double probabilitiesValue = fitness[i] / sumFitness;
                probabilities.Add(Math.Round(probabilitiesValue, 4));
            }

            return probabilities;
        }

        //Rulet Tekerleği ile i Değeri Seçme

        public static int RouletteWheelSelection(List<double> probabilities)
        {
            Random random = new Random();
            double randomValue = random.NextDouble();
            double cumulativeProbability = 0.0;

            for (int i = 0; i < probabilities.Count; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomValue <= cumulativeProbability)
                {
                    return i;
                }
            }

            return probabilities.Count - 1; // Güvenlik amaçlı, bu noktaya nadiren ulaşılır.
        }

        //Gözcü Arı Fazı

        public static void OnlookerBee(int foodNumber, List<double> x1Numbers, List<double> x2Numbers, List<double> results, List<double> fitness, List<double> probabilities)
        {
            Random random = new Random();

            for (int t = 0; t < foodNumber; t++)
            {
                // Rulet tekerleği ile i değerini seçin
                int i = RouletteWheelSelection(probabilities);
                int k = random.Next(0, foodNumber); // 0 ile foodNumber arasında bir integer k
                int j = random.Next(0, 2); // 0 ya da 1 olacak bir integer j
                double fi = random.NextDouble() * 2 - 1; // -1 ile 1 arasında ondalıklı kısım maksimum 4 basamak olan bir double fi
                fi = Math.Round(fi, 4);

                double x, x2, V;

                if (j == 0)
                {
                    x = x1Numbers[i];
                    x2 = x1Numbers[k];
                    V = x + fi * (x - x2);
                    double newFunctionValue = Function(V, x2Numbers[i]);
                    double newFitnessValue = CalculateFitness(new List<double> { newFunctionValue })[0];
                    if (newFitnessValue > fitness[i])
                    {
                        x1Numbers[i] = V;
                        results[i] = newFunctionValue;
                        fitness[i] = newFitnessValue;
                    }
                }
                else
                {
                    x = x2Numbers[i];
                    x2 = x2Numbers[k];
                    V = x + fi * (x - x2);
                    double newFunctionValue = Function(x1Numbers[i], V);
                    double newFitnessValue = CalculateFitness(new List<double> { newFunctionValue })[0];
                    if (newFitnessValue > fitness[i])
                    {
                        x2Numbers[i] = V;
                        results[i] = newFunctionValue;
                        fitness[i] = newFitnessValue;
                    }
                }
            }
        }

        //Kaşif Arı Fazı

        public static void ScoutBee(int limit, int foodNumber, List<double> x1Numbers, List<double> x2Numbers, List<double> results, List<double> fitness)
        {
           for (int i = 0; i < limit; i++)
            {
                EmployeeBee(foodNumber, x1Numbers, x2Numbers, results, fitness);
            }
        }

        //Ekrana Yazdırma

        private void WriteValueToLabel(Label label, string value)
        {
            label.Text = value;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
      
    }
}
