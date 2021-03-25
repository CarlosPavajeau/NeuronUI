using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeuronUI.Models.ViewModels
{
    public class NeuronViewModel : ObservableObject
    {
        private Neuron _neuron;
        private string _sill = string.Empty;
        private string _weights = string.Empty;

        private string _maxSteps = string.Empty;
        private string _trainingRate = string.Empty;

        public NeuronViewModel()
        {
            SetUpNeuron = new AsyncCommand(Init, CanInicializate);
            StartTraining = new AsyncCommand(TrainingNeuron);

            ErrorsSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Error del patrón",
                    Values = new ChartValues<ObservableValue>
                    {
                        new ObservableValue(1)
                    },
                    PointGeometry = DefaultGeometries.Circle,
                }
            };
        }

        public string Weights
        {
            get => _weights;
            set
            {
                _weights = value;
                OnPropertyChanged(nameof(Weights));
            }
        }
        public string Sill
        {
            get => _sill;
            set
            {
                _sill = value;
                OnPropertyChanged(nameof(Sill));
            }
        }

        public string TrainingRate
        {
            get => _trainingRate;
            set
            {
                OnPropertyChanged(ref _trainingRate, value);
            }
        }

        public string MaxSteps
        {
            get => _maxSteps;
            set
            {
                OnPropertyChanged(ref _maxSteps, value);
            }
        }

        public Neuron Neuron
        {
            get => _neuron;
            set
            {
                _neuron = value;
                OnPropertyChanged(nameof(Neuron));

                ErrorsSeries[0].Values.Clear();
                ErrorsSeries[0].Values.Add(new ObservableValue(1));

                RefreshViewData();
            }
        }

        public ICommand SetUpNeuron { get; set; }

        public ICommand StartTraining { get; set; }

        public SeriesCollection ErrorsSeries { get; set; }

        public void RefreshViewData()
        {
            Sill = $"Umbral: {_neuron.Sill}";

            string weightsStr = string.Empty;
            for (int i = 0; i < _neuron.Weights.Count; i++)
            {
                weightsStr += $"{_neuron.Weights[i]}";
                if (i < _neuron.Weights.Count - 1)
                {
                    weightsStr += ", ";
                }
            }

            Weights = $"Pesos: {weightsStr}";
        }

        private bool CanInicializate()
        {
            return !string.IsNullOrWhiteSpace(_maxSteps) && !string.IsNullOrWhiteSpace(_trainingRate);
        }

        private Task Init(object parameter)
        {
            if (parameter is NeuronSetUpInputModel neuroInput)
            {
                Neuron = new Neuron(neuroInput.InputsNumber, neuroInput.TrainingRate);
            }

            return Task.CompletedTask;
        }

        private Task TrainingNeuron(object parameter)
        {
            if (parameter is not NeuronTrainingInputModel neuronTraining)
            {
                return Task.CompletedTask;
            }

            int steps = 0;
            bool sw = false;

            while (!sw && (steps <= neuronTraining.MaxStepts))
            {
                ++steps;
                sw = true;
                List<double> patternErrors = new();

                for (int i = 0; i < neuronTraining.Inputs.Count; i++)
                {
                    var input = neuronTraining.Inputs[i].ToArray();
                    double result = _neuron.Output(input);

                    double linealError = neuronTraining.Outputs[i] - result;
                    double patternError = Math.Abs(linealError);
                    patternErrors.Add(patternError);

                    if (result != neuronTraining.Outputs[i])
                    {
                        _neuron.Learn(input, neuronTraining.Outputs[i]);
                        sw = false;
                        RefreshViewData();
                    }
                }

                ErrorsSeries[0].Values.Add(new ObservableValue(patternErrors.Average()));
            }

            return Task.CompletedTask;
        }
    }
}
