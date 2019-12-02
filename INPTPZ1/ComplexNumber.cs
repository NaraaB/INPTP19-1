using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPTPZ1
{
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
