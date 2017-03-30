using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{

    [Serializable]
    public class Neuron
    {
        public string Name { get; protected set; }
        public Func<double, double> Activate { get; protected set; }
        public Func<double, double> dActivate { get; protected set; }
        public double Output { get; protected set; }
        public double Error { get; protected set; }
        public Neuron()
        {

        }
        public Neuron(string name, Func<double, double> activate, Func<double, double> dactivate)
        {
            Name = name;
            Activate = activate;
            dActivate = dactivate;
        }

        public void SetValue(double value)
        {
            Output = Activate(value);
        }
        public void SetError(double value)
        {
            Error = value;
        }
    }


    

  




}
