using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class ColorSensor
    {
        public readonly static float MMRadius = 5;
        public static float PixelsRadius = 0;
        public PointF Position { get; set; }
        public bool PointingDown { get; }
        public ColorSensor(PointF position, bool pointingDown)
        {
            if(PixelsRadius <= 0)
            {
                PixelsRadius = FieldAndRobotInfo.ToPixels(new PointF(MMRadius, 0)).X;
            }
            Position = position;
            PointingDown = pointingDown;
        }
        public Color GetColor(Robot robot, Bitmap field)
        {
            var colorPoint = Extensions.RotatePointAroundPoint(robot.Origin, Position, -robot.Rotation).Add(robot.Location).Subtract(robot.Origin);
            PointF pixelColorPoint = FieldAndRobotInfo.ToPixels(colorPoint);
            Point intPixelColorPoint = new Point((int)pixelColorPoint.X, (int)pixelColorPoint.Y);
            int pixleRadius = (int)PixelsRadius;
            double r = 0;
            double g = 0;
            double b = 0;
            long amount = 0;
            for(int x = intPixelColorPoint.X - pixleRadius; x < intPixelColorPoint.X + pixleRadius; x++)
            {
                for (int y = intPixelColorPoint.Y - pixleRadius; y < intPixelColorPoint.Y + pixleRadius; y++)
                {
                    if(x >= 0 && y>=0 && x < field.Size.Width && y < field.Size.Height)
                    {
                        Point p = new Point(x, y);
                        if(Extensions.Distance(pixelColorPoint, p) <= PixelsRadius)
                        {
                            Color c = field.GetPixel(x, y);
                            r += c.R;
                            g += c.G;
                            b += c.B;
                            amount++;
                        }
                    }
                }
            }
            if (amount > 0)
            {
                r /= amount;
                g /= amount;
                b /= amount;
            }
            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }
    }
}
