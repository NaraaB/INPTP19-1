using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPTPZ1
{
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
}
