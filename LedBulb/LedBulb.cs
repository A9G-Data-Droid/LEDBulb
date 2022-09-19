using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;



namespace Bulb
{

	/// <summary>
	/// The LEDBulb is a .Net control for Windows Forms that emulates an
	/// LED light with two states On and Off.  The purpose of the control is to 
	/// provide a sleek looking representation of an LED light that is sizable, 
	/// has a transparent background and can be set to different colors.  
	/// </summary>
	public partial class LedBulb : Control
	{

		private Color _color;
		private bool _on = true;
		private readonly Color _reflectionColor = Color.FromArgb(180, 255, 255, 255);
		private readonly Color[] _surroundColor = new Color[] { Color.FromArgb(0, 255, 255, 255) };
		private readonly Timer _timer = new();

		/// <summary>
		/// Gets or Sets the color of the LED light
		/// </summary>
		[DefaultValue(typeof(Color), "153, 255, 54")]
		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				DarkColor = ControlPaint.Dark(_color);
				DarkDarkColor = ControlPaint.DarkDark(_color);
				Invalidate();  // Redraw the control
			}
		}

		/// <summary>
		/// Dark shade of the LED color used for gradient
		/// </summary>
		public Color DarkColor { get; protected set; }

		/// <summary>
		/// Very dark shade of the LED color used for gradient
		/// </summary>
		public Color DarkDarkColor { get; protected set; }

		/// <summary>
		/// Gets or Sets whether the light is turned on
		/// </summary>
		public bool On
		{
			get { return _on; }
			set { _on = value; Invalidate(); }
		}

		/// <summary>
		/// Colors for if the bulb is on or off
		/// </summary>
		private Color OnColor => On ? this.Color : Color.FromArgb(150, DarkColor);
		private Color OffColor => On ? DarkColor : DarkDarkColor;

		// Calculate the dimensions of the bulb
		private int BulbWidth => Width - (Padding.Left + Padding.Right);
		private int BulbHeight => Height - (Padding.Top + Padding.Bottom);

		/// <summary>
		/// Diameter is the lesser of width and height, Subtract 1 pixel so ellipse doesn't get cut off
		/// </summary>
		private int Diameter => Math.Max(Math.Min(BulbWidth, BulbHeight) - 1, 1);



		/// <summary>
		/// Constructor to get a new bulb!
		/// </summary>
		public LedBulb()
		{
			SetStyle(ControlStyles.DoubleBuffer
			| ControlStyles.AllPaintingInWmPaint
			| ControlStyles.ResizeRedraw
			| ControlStyles.UserPaint
			| ControlStyles.SupportsTransparentBackColor, true);

			Color = Color.FromArgb(255, 153, 255, 54);
			_timer.Tick += new EventHandler(
				(sender, e) => { On = !On; }
			);
		}



		/// <summary>
		/// Handles the Paint event for this UserControl
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			// Create an offscreen graphics object for double buffering
			Bitmap offScreenBmp = new(ClientRectangle.Width, ClientRectangle.Height);
			using Graphics g = Graphics.FromImage(offScreenBmp);
			g.SmoothingMode = SmoothingMode.HighQuality;

			// Draw the control
			DrawControl(g);

			// Draw the image to the screen
			e.Graphics.DrawImageUnscaled(offScreenBmp, 0, 0);
		}

		/// <summary>
		/// Causes the Led to start blinking
		/// </summary>
		/// <param name="milliseconds">Number of milliseconds to blink for. 0 stops blinking.</param>
		public void Blink(int milliseconds)
		{
			if (milliseconds > 0)
			{
				On = true;
				_timer.Interval = milliseconds;
				_timer.Enabled = true;
			}
			else
			{
				_timer.Enabled = false;
				On = false;
			}
		}


		/// <summary>
		/// Renders the control to an image
		/// </summary>
		private void DrawControl(Graphics g)
		{
			// Draw the background ellipse
			var canvasRectangle = new Rectangle(Padding.Left, Padding.Top, Diameter, Diameter);
			g.FillEllipse(new SolidBrush(OffColor), canvasRectangle);

			// Draw the glow gradient
			var glowPath = new GraphicsPath();
			glowPath.AddEllipse(canvasRectangle);
			var glowBrush = new PathGradientBrush(glowPath)
			{
				CenterColor = OnColor,
				SurroundColors = new Color[] { Color.FromArgb(0, OnColor) }
			};

			g.FillEllipse(glowBrush, canvasRectangle);

			// Don't paint outside the circle!
			g.Clip = new Region(glowPath);

			// Draw the white reflection gradient
			var offset = Convert.ToInt32(Diameter * .15F);
			var reflectionDiameter = Convert.ToInt32(canvasRectangle.Width * .8F);
			var whiteRect = new Rectangle(canvasRectangle.X - offset, canvasRectangle.Y - offset, reflectionDiameter, reflectionDiameter);
			var reflectionPath = new GraphicsPath();
			reflectionPath.AddEllipse(whiteRect);
			var reflectionBrush = new PathGradientBrush(reflectionPath)
			{
				CenterColor = _reflectionColor,
				SurroundColors = _surroundColor
			};

			g.FillEllipse(reflectionBrush, whiteRect);

			// Draw the border
			g.SetClip(canvasRectangle);
			if (On) g.DrawEllipse(new Pen(Color.FromArgb(85, Color.Black), 1F), canvasRectangle);
		}
	}
}
