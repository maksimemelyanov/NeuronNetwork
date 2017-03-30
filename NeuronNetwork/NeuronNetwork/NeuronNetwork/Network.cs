using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    /// <summary>
    /// Нейронная сеть
    /// </summary>
    [Serializable]
    public class NeuronNetwork
    {
        /// <summary>
        /// Список распознаваемых категорий
        /// </summary>
        public List<string> Categories { get; protected set; }
        //Func<double, double> StartActivation = new Func<double, double>(x=>x);
        //Func<double, double> Activation = new Func<double, double>(x => x);
        //Func<double, double> dStartActivation = new Func<double, double>(x => x);
        //Func<double, double> dActivation = new Func<double, double>(x => x);

        /// <summary>
        /// Функция активации для начальных нейронов
        /// </summary>
        private Func<double, double> StartActivation = ((double x) => x);

        /// <summary>
        /// Функция активации для нейронов
        /// </summary>
        private Func<double, double> Activation = ((double x) => (1 / (1 + Math.Exp(-x))));

        /// <summary>
        /// Производная функции активации для начальных нейронов
        /// </summary>
        private Func<double, double> dStartActivation = ((double x) => 1);

        /// <summary>
        /// Производная функции активации для нейронов
        /// </summary>
        private Func<double, double> dActivation = ((double x) => ((1 / (1 + Math.Exp(-x))) * (1 - (1 / (1 + Math.Exp(-x))))));


        // Func<double, double> StartActivation = delegate(double x)
        //{ return x; };
        //Func<double, double> Activation = delegate(double x)
        //{ return (1 / (1 + Math.Exp(-x))); };
        //Func<double, double> dStartActivation = delegate(double x)
        //{ return 1; };
        //Func<double, double> dActivation = delegate(double x)
        //{ return ((1 / (1 + Math.Exp(-x))) * (1 - (1 / (1 + Math.Exp(-x))))); };

        /// <summary>
        /// Список слоев сети
        /// </summary>
        public List<Layer> Layers { get; protected set; }

        /// <summary>
        /// Список начальных нейронов
        /// </summary>
        public List<Neuron> Input { get; protected set; }

        /// <summary>
        /// Список выходных нейронов
        /// </summary>
        public List<Neuron> Output { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public NeuronNetwork()
        {

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="layers">Список из количества нейронов на каждом из слоев</param>
        /// <param name="categories">Список распознаваемых категорий</param>
        public NeuronNetwork(List<int> layers, List<string> categories)
        {
            Categories = categories;
            Random r = new Random();
            Input = new List<Neuron>(layers[0]);
            Output = new List<Neuron>(layers[layers.Count - 1]);
            Layers = new List<Layer>(layers.Count);
            int l_count = layers.Count;
            //Создание входного слоя
            Layer l = new Layer("0", layers[0], StartActivation, dStartActivation, isStart: true);
            int n_count = l.Neurons.Count;
            Layers.Add(l);
            {
                foreach (var n in l.Neurons)
                {
                    Input.Add((Neuron)n);
                }
            }

            //Создание скрытых слоев
            for (int i = 1; i < l_count - 1; i++)
            {
                l = new Layer(i.ToString(), layers[i], Activation, dActivation);
                n_count = l.Neurons.Count;
                for (int j = 0; j < n_count; j++)
                {
                    List<Neuron> pr_Neurons = Layers[i - 1].Neurons;
                    foreach (var n in pr_Neurons)
                    {
                        l.Weights.Add(new Tuple<Neuron, Neuron, double>(n, l.Neurons[j], (r.NextDouble() - 0.5)));
                    }
                }
                Layers.Add(l);

            }

            //Создание выходного слоя
            l = new Layer((l_count - 1).ToString(), layers[l_count - 1], Activation, dActivation, isEnd: true);
            n_count = l.Neurons.Count;
            for (int j = 0; j < n_count; j++)
            {
                List<Neuron> pr_Neurons = Layers[l_count - 2].Neurons;
                foreach (var n in pr_Neurons)
                {
                    l.Weights.Add(new Tuple<Neuron, Neuron, double>(n, l.Neurons[j], (r.NextDouble() - 0.5)));
                }
            }
            Layers.Add(l);
            {
                foreach (var n in l.Neurons)
                {
                    Output.Add((Neuron)n);
                }
            }
        }

        /// <summary>
        /// Расчет выхода сети
        /// </summary>
        public void Calculate()
        {
            //Вычисляем по слоям
            for (int i = 1; i < Layers.Count; i++)
            {
                Layers[i].Calculate(Layers[i - 1]);
            }
        }

        /// <summary>
        /// Получение результат
        /// </summary>
        /// <returns>Вектор принадлежности к определенному классу</returns>
        public List<double> GetResult()
        {
            Layer last = Layers.Last();
            int count = last.Neurons.Count;
            List<double> result = new List<double>(count);
            double max = 0;
            for (int i = 0; i < count; i++)
            {
                if (last.Neurons[i].Output > max)
                    max = last.Neurons[i].Output;

            }
            for (int i = 0; i < count; i++)
            {
                result.Add(last.Neurons[i].Output == max ? 1 : 0); //Максимальное значение выхода соответствует классу
            }
            return result;

        }

        /// <summary>
        /// Установка начальных нейронов
        /// </summary>
        /// <param name="values">Устанавливаемые значения</param>
        public void SetStart(List<double> values)
        {
            int size = values.Count;
            for (int i = 0; i < size; i++)
            {
                Layers[0].Neurons[i].SetValue(values[i]);
            }
        }

        /// <summary>
        /// Обучение на конкретном примере (метод обратного распространения ошибки)
        /// </summary>
        /// <param name="input">Входной вектор</param>
        /// <param name="result">Выходной вектор</param>
        public void Studying(List<double> input, List<double> result)
        {
            SetStart(input); //Установка начальных значений
            Calculate(); //Расчет выхода сети
            double norma = 0.55; //Скорость обучения
            int last_layer = Layers.Count - 1;
            int count = Layers[last_layer].Neurons.Count; //Выходной слой
            List<double> res = GetResult();
            for (int i = 0; i < count; i++)
            {
                Layers[last_layer].Neurons[i].SetError((result[i] - res[i])); //Расчет ошибки нейрона на выходном слое
            }

            for (int l = last_layer - 1; l > 0; l--)
            {
                //Расчет ошибок нейронов на предыдущих слоях (послойно)
                count = Layers[l].Neurons.Count;
                for (int i = 0; i < count; i++)
                {
                    double d = 0;

                    //Выбираем связь со следующим слоем
                    List<Tuple<Neuron, Neuron, double>> weights =
                        Layers[l + 1].Weights.Where(x => x.Item1 == Layers[l].Neurons[i]).ToList();
                    foreach (var w in weights)
                    {
                        //Наращиваем ошибку
                        d += w.Item2.Error * w.Item3;
                    }
                    Layers[l].Neurons[i].SetError(d);
                }
            }

            //Изменение весов
            for (int i = 1; i < last_layer + 1; i++)
            {
                int w_count = Layers[i].Weights.Count;
                for (int j = 0; j < w_count; j++)
                {
                    //Находим старую связь, заитраем ее, и добавляем новую с измененным весом
                    Tuple<Neuron, Neuron, double> old_weight = Layers[i].Weights[0];
                    Tuple<Neuron, Neuron, double> new_weight = new Tuple<Neuron, Neuron, double>(old_weight.Item1, old_weight.Item2, old_weight.Item3 + old_weight.Item1.Output * old_weight.Item2.Error *
                                                  norma * old_weight.Item2.dActivate(old_weight.Item1.Output));
                    Layers[i].Weights.Add(new_weight);
                    Layers[i].Weights.Remove(old_weight);
                    w_count -= 1;
                    j = -1;
                }
            }
        }

        /// <summary>
        /// Обучение на наборе
        /// </summary>
        /// <param name="inputs">Набор пар "пример - эталонный результат"</param>
        /// <param name="epochs">Количество эпох обучения</param>
        public void Studying(List<Tuple<List<double>, List<double>>> inputs, int epochs)
        {
            int count = inputs.Count;

            for (int t = 0; t < epochs; t++)
            {
                for (int i = 0; (i < count); i++)
                {
                    Studying(inputs[i].Item1, inputs[i].Item2);
                }
            }
        }


    }
}
