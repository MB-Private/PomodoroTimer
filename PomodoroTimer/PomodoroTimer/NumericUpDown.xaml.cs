using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public event EventHandler ValueChanged;

        private const int MaxValue = 40;
        private const int MinValue = 0;

        private int m_Value = 0;

        public int Value
        {
            get { return m_Value; }
            set
            {
                if (value != m_Value)
                {
                    m_Value = value;
                    ValueTextBox.Text = value.ToString();
                    OnValueChanged();
                }
            }
        }

        public NumericUpDown()
        {
            InitializeComponent();
            ValueTextBox.Text       = m_Value.ToString();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value < MaxValue)
            {
                Value++;
            }
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value > MinValue)
            {
                Value--;
            }
        }

        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueTextBox == null) { return; }

            int     newValue;
            bool    isNewValueOk    = int.TryParse(ValueTextBox.Text, out newValue);

            isNewValueOk            = isNewValueOk && newValue >= MinValue && newValue <= MaxValue;

            if (!isNewValueOk)
            {
                ValueTextBox.Text   = Value.ToString();
                return;
            }

            Value = newValue;
        }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
