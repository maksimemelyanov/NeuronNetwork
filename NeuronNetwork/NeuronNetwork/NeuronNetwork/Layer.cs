using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    [Serializable]
    public class Layer
    {
        public string Name { get; protected set; }
        public List<Neuron> Neurons { get; protected set; }
        public List<Tuple<Neuron, Neuron, double>> Weights { get; set; }

        public Layer()
        {

        }
        public Layer(string name, int count, Func<double, double> activate, Func<double, double> dactivate, bool isStart = false, bool isEnd = false)
        {
            Name = name;
            Neurons = new List<Neuron>();
            Weights = new List<Tuple<Neuron, Neuron, double>>();
            if (isStart)
            {
                for (int i = 0; i < count; i++)
                {
                    Neuron n = new Neuron(name + "_" + i, activate, dactivate);
                    Neurons.Add(n);
                }
            }
            else if (isEnd)
            {
                for (int i = 0; i < count; i++)
                {
                    Neuron n = new Neuron(name + "_" + i, activate, dactivate);
                    Neurons.Add(n);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Neuron n = new Neuron(name + "_" + i, activate, dactivate);
                    Neurons.Add(n);
                }
            }

        }

        public void Calculate(Layer pr_layer)
        {
            foreach (var n in Neurons)
            {
                List<Tuple<Neuron, Neuron, double>> pr_neurons = Weights.Where(x => x.Item2 == n).ToList();
                double sum = 0;
                foreach (var pr_n in pr_neurons)
                {
                    sum += pr_n.Item1.Output * pr_n.Item3;
                }
                n.SetValue(sum);
            }
        }
    }
}
