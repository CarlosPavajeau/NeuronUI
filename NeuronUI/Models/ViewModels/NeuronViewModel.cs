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

        private string _iterationError;

        public NeuronViewModel()
        {
            Inicializate = new AsyncCommand(Init, CanInicializate);
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

        public string IterationErrors
        {
            get => _iterationError;
            set
            {
                OnPropertyChanged(ref _iterationError, value);
            }
        }

        public Neuron Neuron
        {
            get => _neuron;
            set
            {
                _neuron = value;
                OnPropertyChanged(nameof(Neuron));

                SetViewData();
            }
        }

        public ICommand Inicializate { get; set; }

        public void SetViewData()
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

            IterationErrors = $"{_neuron.Errors.Sum() / 4}";
        }

        private bool CanInicializate()
        {
            return !string.IsNullOrWhiteSpace(_maxSteps);
        }

        private Task Init(object parameter)
        {
            if (parameter is NeuronInputModel neuroInput)
            {
                Neuron = new Neuron(neuroInput.InputsNumber, neuroInput.TrainingRate);
            }

            return Task.CompletedTask;
        }
    }
}
