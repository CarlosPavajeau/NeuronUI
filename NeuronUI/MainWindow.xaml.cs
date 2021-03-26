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

            NeuronViewModel = (NeuronViewModel)DataContext;
        }

        public bool InputsLoaded { get; set; }
        public bool OutputsLoaded { get; set; }

        public List<List<double>> TrainingInputs { get; set; }
        public List<double> Outputs { get; set; }
        public List<List<double>> SimulationInputs { get; set; }

        public NeuronViewModel NeuronViewModel { get; }

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
            OnLoadData(data, FileDataType.TrainingInputs);
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
            OnLoadData(data, FileDataType.Outputs);
        }

        private void OnLoadData(List<string[]> data, FileDataType dataType)
        {
            switch (dataType)
            {
                case FileDataType.TrainingInputs:
                    {
                        ParseInputs(data);

                        InputsCount.Text = $"Entradas: {TrainingInputs[0].Count}";
                        PatternsCount.Text = $"Patrones: {TrainingInputs.Count}";
                        break;
                    }
                case FileDataType.Outputs:
                    {
                        ParseOutputs(data);
                        break;
                    }
                case FileDataType.SimulationInputs:
                    {
                        ParseSimulationInputs(data);

                        if (TrainingInputs[0].Count == SimulationInputs[0].Count)
                        {
                            SimulateButton.IsEnabled = true;
                            SimulateInputsError.Visibility = Visibility.Collapsed;
                            ClearSimulationOutputs();
                        }
                        else
                        {
                            SimulateInputsError.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                default:
                    throw new ArgumentException("Data type not provided");
            }

            SetUpNeuronButton.IsEnabled = InputsLoaded && OutputsLoaded;
            StartTraining.Visibility = Visibility.Hidden;
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
            TrainingInputs = new();

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
                    TrainingInputs.Add(inputs);
                }

            }
        }

        private void ParseSimulationInputs(List<string[]> data)
        {
            SimulationInputs = new();

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
                    SimulationInputs.Add(inputs);
                }

            }
        }

        private void SetUpNeuronButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation.GetHasError(MaxIterations) &&
                !Validation.GetHasError(TrainingRate) &&
                !Validation.GetHasError(ErrorTolerance))
            {
                if (!string.IsNullOrEmpty(MaxIterations.Text) &&
                    !string.IsNullOrEmpty(TrainingRate.Text) &&
                    !string.IsNullOrEmpty(ErrorTolerance.Text))
                {
                    MaxStepts = int.Parse(MaxIterations.Text);
                    double traininRate = double.Parse(TrainingRate.Text);

                    NeuronSetUpInputModel neuron = new()
                    {
                        InputsNumber = TrainingInputs[0].Count,
                        TrainingRate = traininRate
                    };

                    NeuronViewModel.SetUpNeuron.Execute(neuron);

                    StartTraining.Visibility = Visibility.Visible;
                    ClearSimulationOutputs();
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
            double errorTolerance = double.Parse(NeuronViewModel.ErrorTolerance);
            NeuronTrainingInputModel neuronTraining = new()
            {
                MaxStepts = MaxStepts,
                Inputs = TrainingInputs,
                Outputs = Outputs,
                ErrorTolerance = errorTolerance
            };

            NeuronViewModel.StartTraining.Execute(neuronTraining);

            SimulationButtons.Visibility = Visibility.Visible;
            LoadSimulationDataButton.IsEnabled = true;
            ClearSimulationOutputs();
        }

        private void ClearSimulationOutputs()
        {
            SimulationOutputs.Children.Clear();
        }

        private void LoadSimulationDataButton_Click(object sender, RoutedEventArgs e)
        {
            var data = LoadCsvDataFromFile();
            if (data is null)
            {
                return;
            }

            OnLoadData(data, FileDataType.SimulationInputs);
        }

        private void SimulateButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSimulationOutputs();

            foreach (var input in SimulationInputs)
            {
                double result = NeuronViewModel.Neuron.Output(input.ToArray());

                TextBlock simulationOutput = new()
                {
                    FontSize = 17,
                };

                for (int i = 0; i < input.Count; i++)
                {
                    simulationOutput.Text += $"Entrada {i + 1}: {input[i]}";

                    if (i < input.Count - 1)
                    {
                        simulationOutput.Text += ", ";
                    }
                }

                simulationOutput.Text += $" -> Salida: {result}";

                SimulationOutputs.Children.Add(simulationOutput);
            }
        }
    }
}