using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    /// <summary>
    /// Слой нейронов
    /// </summary>
    [Serializable]
    public class Layer
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; protected set; }
        
        /// <summary>
        /// Нейроны данного слоя
        /// </summary>
        public List<Neuron> Neurons { get; protected set; }
        
        /// <summary>
        /// Значение веса связи между нейронами (предыдущий - текущий - вес)
        /// </summary>
        public List<Tuple<Neuron, Neuron, double>> Weights { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Layer()
        {

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Имя слоя</param>
        /// <param name="count">Количество нейронов</param>
        /// <param name="activate">Функция активации для нейронов</param>
        /// <param name="dactivate">Производная функции активации для нейронов</param>
        /// <param name="isStart">Является ли слой начальным</param>
        /// <param name="isEnd">Является ли слой конечным</param>
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

        /// <summary>
        /// Расчет выходов нейронов текущего слоя
        /// </summary>
        /// <param name="pr_layer">Предыдущий слой</param>
        public void Calculate(Layer pr_layer)
        {
            //Для каждого нейрона из слоя
            foreach (var n in Neurons)
            {
                // выбираем связи с нейронами предыдущего слоя
                List<Tuple<Neuron, Neuron, double>> pr_neurons = Weights.Where(x => x.Item2 == n).ToList();
                double sum = 0;

                //Суммируем
                foreach (var pr_n in pr_neurons)
                {
                    sum += pr_n.Item1.Output * pr_n.Item3;
                }

                //Считаем выход нейрона
                n.SetValue(sum);
            }
        }
    }
}
