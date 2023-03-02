using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator
{

    [TestFixture]
    class Tests
    {
        Calculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new Calculator();
        }

        [TestCase("1 + 123 - (4 * 5)", 104)]
        [TestCase("5 + (2 - 4)", 3)]
        [TestCase("5 + 9", 14)]
        [TestCase("(3 + 4)", 7)]
        [TestCase("13 + 4 - 4 / (3 + 1)", 16)]
        [TestCase("1 + ((1 + 1) + 1) + (1 + 1) + 1", 7)]
        [TestCase("1000 - 7 * 3 - 1 - 1 - 8 - 343 - (2 - 1 * (5 * 4))", 644)]
        [TestCase("1000 - 7 - 7 - 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7 -7- 7- 7- 7- 7- 7 -7", 790)]
        [TestCase("((2+3) * 4)", 20)]
        [TestCase("(((10 / 5)+3) * 4)", 20)]
        public void ShouldReturnSolvedExpression(string testString, double expected)
        {
            Calculator calc = new Calculator();
            double res = calc.Calculate(testString);
            res.Should().Be(expected);
        }

    }
}
