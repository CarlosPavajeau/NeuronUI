using System;
using System.Collections.Generic;

namespace NeuronUI.Models
{
    public class Neuron
    {
        public Neuron(int inputsNumber, double trainingRate)
        {
            Weights = new List<double>(inputsNumber);
            TrainingRate = trainingRate;

            Errors = new List<double>();

            Init();
        }

        public List<double> Weights { get; }
        public double Sill { get; set; }
        public double TrainingRate { get; }

        public List<double> Errors { get; }

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
            Errors.Add(Math.Abs(error));

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
            double acc = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                acc += inputs[i] * Weights[i];
            }

            return acc + Sill;
        }
    }
}
