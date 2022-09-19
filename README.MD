# A Simple Vector-Based LED User Control
By Steve Marsh On 2013-04-05

## Original Source
https://www.codeproject.com/articles/114122/a-simple-vector-based-led-user-control

![image](https://user-images.githubusercontent.com/26984068/191117540-cf64c502-8d2a-4e4e-b7c8-de4d22329fc8.png)


## Introduction
The LEDBulb is a .NET user control for Windows Forms that emulates an LED light. Its purpose is to provide a sleek looking representation of an LED light that is sizable, has a transparent background and can be set to different colors.

```c#
LedBulb bulb = new LedBulb();
bulb.Size = new Size(25, 25);
bulb.Color = Color.LawnGreen;
bulb.On = true;
this.Controls.Add(bulb);
```

## Sizeable
I recently needed to add a custom user control to a Window Forms project of mine that would represent an LED bulb commonly seen on household electronics. After a quick search, most of the existing controls I found were ugly and used static images, which means they wouldn’t scale well. This control uses the System.Drawing.Drawing2D namespace to draw a vector image so that it not only looks clean but will also scale to any size without affecting the image quality.

![image](https://user-images.githubusercontent.com/26984068/191117565-2dc38b37-884e-4f1c-a0b0-c7ee5ce8527e.png)


Rendering the control starts with drawing a solid circle with our background color. Then we draw a radial gradient over it with our highlight color, transitioning to transparent. To draw the reflection, we draw another radial gradient, from white to transparent, but in a smaller rectangle shifted up and left. To prevent the white gradient from showing up outside the bulb, we set the clip parameter to the bounds of our original circle. 

```c#
// Fill in the background circle 
g.FillEllipse(new SolidBrush(darkColor), drawRectangle);

// Draw the glow gradient
GraphicsPath path = new GraphicsPath();
path.AddEllipse(drawRectangle);
PathGradientBrush pathBrush = new PathGradientBrush(path);
pathBrush.CenterColor = lightColor;
pathBrush.SurroundColors = new Color[] { Color.FromArgb(0, lightColor) };
g.FillEllipse(pathBrush, drawRectangle);

// Set the clip boundary to the edge of the ellipse
GraphicsPath gp = new GraphicsPath();
gp.AddEllipse(drawRectangle);
g.SetClip(gp);

// Draw the white reflection gradient
GraphicsPath path1 = new GraphicsPath();
path1.AddEllipse(whiteRectangle); // a smaller rectangle set to the top left
PathGradientBrush pathBrush1 = new PathGradientBrush(path);
pathBrush1.CenterColor = Color.FromArgb(180, 255, 255, 255);
pathBrush1.SurroundColors = new Color[] { Color.FromArgb(0, 255, 255, 255) };
g.FillEllipse(pathBrush1, whiteRect);
```

## Transparent Background 
It is important that this control has a transparent background. To do this, we need to add a command to the constructor. 

```c#
SetStyle(ControlStyles.DoubleBuffer
    | ControlStyles.AllPaintingInWmPaint
    | ControlStyles.ResizeRedraw
    | ControlStyles.UserPaint
    | ControlStyles.SupportsTransparentBackColor, true
);
```

## Customizable Colors
It is important that this control has a transparent background. To do this, we need to add a command to the constructor. 

```c#
this.DarkColor = ControlPaint.Dark(_this.Color);
this.DarkDarkColor = ControlPaint.DarkDark(_this.Color);
```

![image](https://user-images.githubusercontent.com/26984068/191117626-a8f406b9-c9fc-4972-85f4-d55c4e63e82f.png)

![image](https://user-images.githubusercontent.com/26984068/191117637-4295be6e-32a0-4bad-a53a-a50cb6eb5dcc.png)


## Blink
The control has a blink method that will make the control start blinking. You pass the number of milliseconds between blinks. To turn the blinking off just call the blink function again passing 0 as the argument.

```c#
led.Blink(2000); // Slow blink
led.Blink(500);  // Fast blink
led.Blink(0);    // Turn off blinking
```

![image](https://user-images.githubusercontent.com/26984068/191117684-075c84b9-4072-4608-b5ed-7d81e6b6c521.png)


## Points of Interest 
This control uses double-buffering by drawing its image to an off-screen bitmap first, then sending the final rendering to the client. Double buffering creates a smoother drawing of the control and avoids flicker when moving or re-sizing. The code to accomplish this is below.

```c#
protected override void OnPaint(PaintEventArgs e){
    // Create an offscreen graphics object for double buffering
    Bitmap offScreenBmp = new Bitmap(this.ClientRectangle.Width, 
                this.ClientRectangle.Height);
    System.Drawing.Graphics g = Graphics.FromImage(offScreenBmp);
    g.SmoothingMode = SmoothingMode.HighQuality;

    // Render the control to the off-screen bitmap
    drawControl(g);

    // Draw the image to the screen
    e.Graphics.DrawImageUnscaled(offScreenBmp, 0, 0);    
}
```
