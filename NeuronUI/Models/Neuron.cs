using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuronUI.Models
{
    public class Neuron
    {
        public Neuron(int inputsNumber, double trainingRate)
        {
            Weights = new List<double>(inputsNumber);
            TrainingRate = trainingRate;

            Init();
        }

        public Neuron()
        {
            Weights = new List<double>();
        }

        public List<double> Weights { get; set; }
        public double Sill { get; set; }

        private double TrainingRate { get; }

        private void Init()
        {
            Random random = new();
            for (int i = 0; i < Weights.Capacity; i++)
            {
                Weights.Add((random.NextDouble() * 2) - 1.0f);
            }

            Sill = (random.NextDouble() * 2) - 1.0f;
        }

        public void Learn(double[] inputs, double expectedOutput)
        {
            double output = Output(inputs);
            double error = expectedOutput - output;

            for (int i = 0; i < Weights.Count; i++)
            {
                Weights[i] += TrainingRate * error * inputs[i];
            }

            Sill += TrainingRate * error;
        }

        public double Output(double[] inputs)
        {
            return Predict(NextInput(inputs));
        }

        private static double Predict(double input)
        {
            return input > 0 ? 1.0 : 0.0;
        }

        private double NextInput(double[] inputs)
        {
            double acc = inputs.Select((t, i) => t * Weights[i]).Sum();
            return acc + Sill;
        }
    }
}
