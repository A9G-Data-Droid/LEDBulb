using Bulb;
using System;
using System.Windows.Forms;

namespace BulbExample
{
    public partial class ExampleForm : Form
    {

        private int _blink = 0;

        public ExampleForm()
        {
            InitializeComponent();
        }

        // Turn the bulb On or Off
        private void LedBulb_Click(object sender, EventArgs e)
        {
            ((LedBulb)sender).On = !((LedBulb)sender).On;
        }

        private void LedBulb7_Click(object sender, EventArgs e)
        {
            if (_blink == 0)
                _blink = 500;
            else
                _blink = 0;

            ((LedBulb)sender).Blink(_blink);
        }
    }
}
