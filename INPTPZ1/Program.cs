using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;


namespace INPTPZ1
{
    /// <summary>
    /// This program should produce Newton fractals.
    /// See more at: https://en.wikipedia.org/wiki/Newton_fractal
    /// </summary>
    class Program
    {
        private static int width, height;
        private static double xmin, ymin, xmax, ymax, xstep, ystep;
        private static Bitmap bmpCanvas;
        private static Color[] colors;
        private static Polynome poly, derivationPoly;
        private static List<ComplexNumber> roots;
        private static int iteration, idOfRoot;
        private static ComplexNumber chosenComplex;

        private static void Initialize()
        {
            bmpCanvas = new Bitmap(width, height);
            xmin = ymin = -1.5;
            xmax = ymax = 1.5;
            xstep = (xmax - xmin) / width;
            ystep = (ymax - ymin) / height;

            colors = new Color[]
            {
                Color.Magenta, Color.Aqua, Color.Lime
            };

            roots = new List<ComplexNumber>();
            poly = new Polynome(new List<ComplexNumber>
            {
                new ComplexNumber {RealPart = 1},
                ComplexNumber.Zero,
                ComplexNumber.Zero,
                new ComplexNumber {RealPart = 1},
            });
            derivationPoly = poly.Derivate();
            Console.WriteLine("f(x) = " + poly);
            Console.WriteLine("f'(x) = " + derivationPoly);
        }

        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                if (!Int32.TryParse(args[0], out width))
                {
                    Console.WriteLine("Error occured while parsing arg[0] to Width value");
                    return;
                }
                if (!Int32.TryParse(args[1], out height))
                {
                    Console.WriteLine("Error occured while parsing arg[1] to Height value");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Please enter the width and height of the canvas");
                Console.WriteLine("Width = ");
                if (!Int32.TryParse(Console.ReadLine(), out width))
                {
                    Console.WriteLine("Error occured while parsing input to Width value");
                    return;
                }
                Console.WriteLine("Height = ");
                if (!Int32.TryParse(Console.ReadLine(), out height))
                {
                    Console.WriteLine("Error occured while parsing input to Height value");
                    return;
                }
            }
            if (width > 0 && height > 0)
            {
                Initialize();
                for (int xCoordinate = 0; xCoordinate < width; xCoordinate++)
                {
                    for (int yCoordinate = 0; yCoordinate < height; yCoordinate++)
                    {
                        chosenComplex = new ComplexNumber()
                        {
                            RealPart = xmin + yCoordinate * xstep,
                            ImaginaryPart = ymin + xCoordinate * ystep
                        };
                        if (chosenComplex.isComplexZero())
                        {
                            chosenComplex.RealPart = 0.0001;
                            chosenComplex.ImaginaryPart = 0.0001f;
                        }
                        FindSolutionByNewton();

                        FindIdOfRoot();

                        ColorizePixel(idOfRoot, iteration, yCoordinate, xCoordinate);
                    }
                }
                bmpCanvas.Save("../../../INPTPZ1_output.png");
            }
            else
            {
                Console.WriteLine("Invalid input for Width and/or Height!");
                return;
            }
            Console.ReadKey();
            return;
        }
        private static void FindSolutionByNewton()
        {
            iteration = 0;
            for (int q = 0; q < 25; q++)
            {
                var diff = poly.Evaluate(chosenComplex).Divide(derivationPoly.Evaluate(chosenComplex));
                chosenComplex = chosenComplex.Subtract(diff);
                if (Math.Pow(diff.RealPart, 2) + Math.Pow(diff.ImaginaryPart, 2) >= 0.5)
                {
                    q--;
                }
                iteration++;
            }
        }
        private static void FindIdOfRoot()
        {
            idOfRoot = 0;
            bool isRootIdKnown = false;
            for (int w = 0; w < roots.Count; w++)
            {
                if (Math.Pow(chosenComplex.RealPart - roots[w].RealPart, 2) + Math.Pow(chosenComplex.ImaginaryPart - roots[w].ImaginaryPart, 2) <= 0.001) //tolerance?
                {
                    isRootIdKnown = true;
                    idOfRoot = w;
                }
            }
            if (!isRootIdKnown)
            {
                roots.Add(chosenComplex);
                idOfRoot = roots.Count;
            }
        }
        private static void ColorizePixel(int currentIdOfRoot, int currentIteration, int x, int y)
        {
            Color pxlColor = colors[currentIdOfRoot % colors.Length];
            int red = Math.Min(Math.Max(0, pxlColor.R - currentIteration * 3), 255);
            int green = Math.Min(Math.Max(0, pxlColor.G - currentIteration * 3), 255);
            int blue = Math.Min(Math.Max(0, pxlColor.B - currentIteration * 3), 255);
            pxlColor = Color.FromArgb(red, green, blue);
            bmpCanvas.SetPixel(x, y, pxlColor);
        }
    }
}
