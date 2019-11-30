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

    class Polynome
    {
        public List<ComplexNumber> Coefficient { get; set; }

        public Polynome()
        {
            Coefficient = new List<ComplexNumber>();
        }

        public Polynome(List<ComplexNumber> coefs)
        {
            Coefficient = new List<ComplexNumber>();
            foreach (ComplexNumber element in coefs)
            {
                Coefficient.Add(element);
            }
        }

        public Polynome Derivate()
        {
            Polynome derivation = new Polynome();
            for (int i = 1; i < Coefficient.Count; i++)
            {
                derivation.Coefficient.Add(Coefficient[i].Multiply(new ComplexNumber() { RealPart = i }));
            }

            return derivation;
        }

        public ComplexNumber Evaluate(ComplexNumber complexNumber)
        {
            ComplexNumber evaluatedComplex = ComplexNumber.Zero;
            for (int i = 0; i < Coefficient.Count; i++)
            {
                ComplexNumber coefficient = Coefficient[i];
                ComplexNumber givenComplexNumber = complexNumber;
                int power = i;

                if (i > 0)
                {
                    for (int j = 0; j < power - 1; j++)
                        givenComplexNumber = givenComplexNumber.Multiply(complexNumber);

                    coefficient = coefficient.Multiply(givenComplexNumber);
                }

                evaluatedComplex = evaluatedComplex.Add(coefficient);
            }

            return evaluatedComplex;
        }

        public override string ToString()
        {
            string polynomeByString = "";
            for (int i = 0; i < Coefficient.Count; i++)
            {
                polynomeByString += Coefficient[i];
                if (i > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        polynomeByString += "x";
                    }
                }
                polynomeByString += " + ";
            }
            return polynomeByString.Substring(0, polynomeByString.Length - 3);
        }
    }

    class ComplexNumber
    {
        public double RealPart { get; set; }
        public double ImaginaryPart { get; set; }

        public readonly static ComplexNumber Zero = new ComplexNumber()
        {
            RealPart = 0,
            ImaginaryPart = 0
        };

        public bool isComplexZero()
        {
            ComplexNumber a = this;
            if (a.RealPart == 0 && a.ImaginaryPart == 0)
                return true;
            else
                return false;
        }

        public ComplexNumber Multiply(ComplexNumber b)
        {
            ComplexNumber a = this;
            return new ComplexNumber()
            {
                RealPart = a.RealPart * b.RealPart - a.ImaginaryPart * b.ImaginaryPart,
                ImaginaryPart = a.RealPart * b.ImaginaryPart + a.ImaginaryPart * b.RealPart
            };
        }

        public ComplexNumber Add(ComplexNumber b)
        {
            ComplexNumber a = this;
            return new ComplexNumber()
            {
                RealPart = a.RealPart + b.RealPart,
                ImaginaryPart = a.ImaginaryPart + b.ImaginaryPart
            };
        }
        public ComplexNumber Subtract(ComplexNumber b)
        {
            ComplexNumber a = this;
            return new ComplexNumber()
            {
                RealPart = a.RealPart - b.RealPart,
                ImaginaryPart = a.ImaginaryPart - b.ImaginaryPart
            };
        }

        internal ComplexNumber Divide(ComplexNumber b)
        {
            ComplexNumber numerator = this.Multiply(new ComplexNumber() { RealPart = b.RealPart, ImaginaryPart = -b.ImaginaryPart });
            double denominator = b.RealPart * b.RealPart + b.ImaginaryPart * b.ImaginaryPart;

            return new ComplexNumber()
            {
                RealPart = numerator.RealPart / denominator,
                ImaginaryPart = numerator.ImaginaryPart / denominator
            };
        }

        public override string ToString()
        {
            return $"({RealPart} + {ImaginaryPart}i)";
        }
    }
}
