using System;
using System.Windows.Forms;

namespace Bulb {
	public partial class ExampleForm : Form {

		private int _blink = 0;
		
		public ExampleForm() {
			InitializeComponent();
		}

		// Turn the bulb On or Off
		private void ledBulb_Click(object sender, EventArgs e) {
			((LedBulb)sender).On = !((LedBulb)sender).On;
		}

		private void ledBulb7_Click(object sender, EventArgs e) {
			if (_blink == 0) _blink = 500;
			else _blink = 0;
			((LedBulb)sender).Blink(_blink);
		}
	}
}
