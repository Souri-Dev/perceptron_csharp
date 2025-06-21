using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Activity3
{

    public partial class Form1 : Form
    {
        private Button[,] pixelGrid = new Button[5, 3];
        private Perceptron[] perceptrons;
        private Dictionary<int, int[]> digitPatterns;

        public Form1()
        {
            InitializeComponent();
            InitPixelGrid();
            InitPerceptrons();
            TrainPerceptrons();
        }

        private void InitPixelGrid()
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var btn = new Button
                    {
                        Width = 40,
                        Height = 40,
                        Top = row * 42,
                        Left = col * 42,
                        BackColor = Color.White,
                        Tag = 0
                    };
                    btn.Click += (s, e) =>
                    {
                        btn.BackColor = (int)btn.Tag == 0 ? Color.Black : Color.White;
                        btn.Tag = (int)btn.Tag == 0 ? 1 : 0;
                    };
                    Controls.Add(btn);
                    pixelGrid[row, col] = btn;
                }
            }

            var predictBtn = new Button
            {
                Text = "Predict",
                Top = 220,
                Left = 0,
                Width = 126
            };
            predictBtn.Click += PredictBtn_Click;
            Controls.Add(predictBtn);
        }

        private void InitPerceptrons()
        {
            perceptrons = new Perceptron[10];
            for (int i = 0; i < 10; i++)
                perceptrons[i] = new Perceptron(15);

            digitPatterns = new Dictionary<int, int[]> {
                { 0, new[]{1,1,1,1,0,1,1,0,1,1,0,1,1,1,1} },
                { 1, new[]{0,1,0,1,1,0,0,1,0,0,1,0,1,1,1} },
                { 2, new[]{1,1,1,0,0,1,1,1,1,1,0,0,1,1,1} },
                { 3, new[]{1,1,1,0,0,1,0,1,1,0,0,1,1,1,1} },
                { 4, new[]{1,0,1,1,0,1,1,1,1,0,0,1,0,0,1} },
                { 5, new[]{1,1,1,1,0,0,1,1,1,0,0,1,1,1,1} },
                { 6, new[]{1,1,1,1,0,0,1,1,1,1,0,1,1,1,1} },
                { 7, new[]{1,1,1,0,0,1,0,1,0,0,1,0,0,1,0} },
                { 8, new[]{1,1,1,1,0,1,1,1,1,1,0,1,1,1,1} },
                { 9, new[]{1,1,1,1,0,1,1,1,1,0,0,1,1,1,1} }
            };
        }

        private void TrainPerceptrons()
        {
            for (int epoch = 0; epoch < 100; epoch++)
            {
                foreach (var pair in digitPatterns)
                {
                    int digit = pair.Key;
                    int[] pattern = pair.Value;
                    for (int i = 0; i < 10; i++)
                    {
                        int target = i == digit ? 1 : 0;
                        perceptrons[i].Train(pattern, target);
                    }
                }
            }
        }

        private void PredictBtn_Click(object sender, EventArgs e)
        {
            int[] input = new int[15];
            for (int row = 0; row < 5; row++)
                for (int col = 0; col < 3; col++)
                    input[row * 3 + col] = (int)pixelGrid[row, col].Tag;

            int best = -1;
            double bestScore = double.MinValue;
            for (int i = 0; i < 10; i++)
            {
                double score = perceptrons[i].Score(input);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = i;
                }
            }

            MessageBox.Show($"Predicted Digit: {best}", "Result");
        }
    }

    public class Perceptron
    {
        private double[] Weights;
        private double Bias;
        private double LearningRate = 0.1;

        public Perceptron(int inputSize)
        {
            Weights = new double[inputSize];
            Bias = 0;
        }

        public int Predict(int[] input) => Score(input) >= 0 ? 1 : 0;

        public double Score(int[] input)
        {
            double sum = Bias;
            for (int i = 0; i < input.Length; i++)
                sum += input[i] * Weights[i];
            return sum;
        }

        public void Train(int[] input, int target)
        {
            int output = Predict(input);
            int error = target - output;
            for (int i = 0; i < Weights.Length; i++)
                Weights[i] += LearningRate * error * input[i];
            Bias += LearningRate * error;
        }
    }
}
