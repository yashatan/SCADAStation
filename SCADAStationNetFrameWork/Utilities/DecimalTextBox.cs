using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace SCADAStationNetFrameWork
{
    public class DecimalTextBox:TextBox
    {
        public DecimalTextBox()
        {
            this.PreviewTextInput += DecimalTextBox_PreviewTextInput;
            DataObject.AddPastingHandler(this, DecimalTextBox_Pasting);
        }

        private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(this.Text+e.Text);
        }

        private void DecimalTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex(@"^[0-9]+\.?[0-9]*$"); // Chỉ cho phép số thập phân dương
            return regex.IsMatch(text);
        }
    }
    public class NaturalNumberTextBox : TextBox
    {
        public NaturalNumberTextBox()
        {
            this.PreviewTextInput += NaturalNumberTextBox_PreviewTextInput;
            DataObject.AddPastingHandler(this, NaturalNumberTextBox_Pasting);
        }

        private void NaturalNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(this.Text + e.Text);
        }

        private void NaturalNumberTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex(@"^[0-9]+$"); // Chỉ cho phép số thập phân dương
            return regex.IsMatch(text);
        }
    }
}
