using Microsoft.Win32;
using NeuronUI.Data;
using NeuronUI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NeuronUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public bool InputsLoaded { get; set; }
        public bool OutputsLoaded { get; set; }

        public List<List<double>> Inputs { get; set; }
        public List<double> Outputs { get; set; }

        public NeuronViewModel NeuronViewModel { get; set; }

        public int MaxStepts { get; set; }

        private void LoadInputsButton_Click(object sender, RoutedEventArgs e)
        {
            var data = LoadCsvDataFromFile();
            if (data is null)
            {
                return;
            }

            InputsState.Text = $"Entradas cargadas: Sí";
            InputsLoaded = true;
            OnLoadData(data, FileDataType.Input);
        }

        private void LoadOutputsButton_Click(object sender, RoutedEventArgs e)
        {
            var data = LoadCsvDataFromFile();
            if (data is null)
            {
                return;
            }

            OutputsState.Text = $"Salidas cargadas: Sí";
            OutputsLoaded = true;
            OnLoadData(data, FileDataType.Output);
        }

        private void OnLoadData(List<string[]> data, FileDataType dataType)
        {
            switch (dataType)
            {
                case FileDataType.Input:
                    {
                        ParseInputs(data);

                        InputsCount.Text = $"Entradas: {Inputs[0].Count}";
                        PatternsCount.Text = $"Patrones: {Inputs.Count}";
                        break;
                    }
                case FileDataType.Output:
                    {
                        ParseOutputs(data);
                        break;
                    }

                default:
                    throw new ArgumentException("Data type not provided");
            }

            SetUpNeuronButton.IsEnabled = InputsLoaded && OutputsLoaded;
        }

        private void ParseOutputs(List<string[]> data)
        {
            Outputs = new();

            foreach (var item in data)
            {
                foreach (var inp in item)
                {
                    if (double.TryParse(inp, out double result))
                    {
                        Outputs.Add(result);
                    }
                }
            }
        }

        private void ParseInputs(List<string[]> data)
        {
            Inputs = new();

            foreach (var item in data)
            {
                List<double> inputs = new();
                foreach (var inp in item)
                {
                    if (double.TryParse(inp, out double result))
                    {
                        inputs.Add(result);
                    }
                }

                if (inputs.Any())
                {
                    Inputs.Add(inputs);
                }

            }
        }

        private void SetUpNeuronButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation.GetHasError(MaxIterations) && !Validation.GetHasError(TrainingRate))
            {
                if (!string.IsNullOrEmpty(MaxIterations.Text) && !string.IsNullOrEmpty(TrainingRate.Text))
                {
                    MaxStepts = int.Parse(MaxIterations.Text);
                    double traininRate = double.Parse(TrainingRate.Text);

                    NeuronSetUpInputModel neuron = new()
                    {
                        InputsNumber = Inputs[0].Count,
                        TrainingRate = traininRate
                    };

                    var dataContext = (NeuronViewModel)DataContext;
                    dataContext.SetUpNeuron.Execute(neuron);

                    StartTraining.Visibility = Visibility.Visible;
                }
            }
        }

        private static List<string[]> LoadCsvDataFromFile()
        {
            string fileName = SelectFile();
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var data = CsvDataLoader.LoadCsv(fileName);
            return data;
        }

        private static string SelectFile()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.DefaultExt = "*.csv";
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv|TXT Files (*.txt)|*.txt";

            bool? result = openFileDialog.ShowDialog();

            return result.HasValue && result.Value ? openFileDialog.FileName : string.Empty;
        }

        private void StartTraining_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (NeuronViewModel)DataContext;
            NeuronTrainingInputModel neuronTraining = new()
            {
                MaxStepts = MaxStepts,
                Inputs = Inputs,
                Outputs = Outputs
            };

            dataContext.StartTraining.Execute(neuronTraining);
        }
    }
}