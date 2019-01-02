using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WROSimulatorV2.Properties;

namespace WROSimulatorV2
{
    public static class TreeNodeImageGenerator
    {
        public static Dictionary<TreeNodeImageInfo, int> StateImageIndexDictionary;
        public static List<Bitmap> Init()
        {
            Bitmap breakpointImage = Resources.breakpoint;
            Bitmap runningArrowImage = Resources.runningArrow;
            Bitmap startPointImage = Resources.startPoint;

            List<Bitmap> originalImages = new List<Bitmap>();
            originalImages.Add(breakpointImage);
            originalImages.Add(runningArrowImage);
            originalImages.Add(startPointImage);
            StateImageIndexDictionary = new Dictionary<TreeNodeImageInfo, int>();
            StateImageIndexDictionary.Add(new TreeNodeImageInfo(false, false, false), -1);
            List<Bitmap> images = new List<Bitmap>();
            byte i = 1;
            while (true)
            {
                bool[] bools = GetBinary(i, originalImages.Count);
                TreeNodeImageInfo imageInfo = new TreeNodeImageInfo(bools);
                StateImageIndexDictionary.Add(imageInfo, i - 1);
                images.Add(GetCombinationImage(bools, originalImages));
                bool allTrue = true;
                for (int j = 0; j < bools.Length; j++)
                {
                    if (!bools[j])
                    {
                        allTrue = false;
                        break;
                    }
                }
                if (allTrue)
                {
                    break;
                }
                i++;
            }
            return images;
        }
        static Bitmap GetCombinationImage(bool[] bools, List<Bitmap> originals)
        {
            List<Bitmap> usedImages = new List<Bitmap>();
            for (int i = 0; i < bools.Length; i++)
            {
                if (bools[i])
                {
                    usedImages.Add(originals[i]);
                }
            }
            Bitmap newImage = new Bitmap(usedImages[0]);
            for (int i = 1; i < usedImages.Count; i++)
            {
                for (int x = 0; x < newImage.Width; x++)
                {
                    for (int y = 0; y < newImage.Height; y++)
                    {
                        Color color = usedImages[i].GetPixel(x, y);
                        if (color.A != 0)
                        {
                            color = Color.FromArgb(255, color.R, color.G, color.B);
                            newImage.SetPixel(x, y, color);
                        }
                    }
                }
            }
            return newImage;
        }
        public static bool[] GetBinary(byte b, int amountOfDigits)
        {
            bool[] digits = new bool[amountOfDigits];
            byte digit = 1;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                digits[i] = (byte)(digit & b) == 1;
                b = (byte)(b >> 1);
            }
            return digits;
        }
    }
    public struct TreeNodeImageInfo
    {
        public bool Breakpoint { get; set; }
        public bool Running { get; set; }
        public bool StartPoint { get; set; }
        public TreeNodeImageInfo(bool breakpoint, bool running, bool startPoint)
        {
            Breakpoint = breakpoint;
            Running = running;
            StartPoint = startPoint;
        }
        public TreeNodeImageInfo(bool[] booleans)
        {
            Breakpoint = booleans[0];
            Running = booleans[1];
            StartPoint = booleans[2];
        }
    }
}
