using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    /// <summary>
    /// Нейрон
    /// </summary>
    [Serializable]
    public class Neuron
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Функция активации
        /// </summary>
        public Func<double, double> Activate { get; protected set; }

        /// <summary>
        /// Производная функции активации
        /// </summary>
        public Func<double, double> dActivate { get; protected set; }

        /// <summary>
        /// Выход нейрона
        /// </summary>
        public double Output { get; protected set; }

        /// <summary>
        /// Ошибка нейрона
        /// </summary>
        public double Error { get; protected set; }
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public Neuron()
        {

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="activate">Функция активации</param>
        /// <param name="dactivate">Производная функции активации</param>
        public Neuron(string name, Func<double, double> activate, Func<double, double> dactivate)
        {
            Name = name;
            Activate = activate;
            dActivate = dactivate;
        }

        /// <summary>
        /// Установка выхода нейрона
        /// </summary>
        /// <param name="value">Значение сумматора</param>
        public void SetValue(double value)
        {
            Output = Activate(value);
        }

        /// <summary>
        /// Установка ошибки
        /// </summary>
        /// <param name="value">Значение ошибки</param>
        public void SetError(double value)
        {
            Error = value;
        }
    }


    

  




}
