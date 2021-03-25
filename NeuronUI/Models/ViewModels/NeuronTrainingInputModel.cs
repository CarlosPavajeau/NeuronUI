using System.Collections.Generic;

namespace NeuronUI.Models.ViewModels
{
    public class NeuronTrainingInputModel
    {
        public List<List<double>> Inputs { get; set; }
        public List<double> Outputs { get; set; }

        public int MaxStepts { get; set; }
    }
}
