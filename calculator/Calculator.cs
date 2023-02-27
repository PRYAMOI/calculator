using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator
{
    public class Calculator
    {
        enum Operator
        {
            UNKNOWN = 0, PLUS, MINUS, MULTIPLY, DIVIDE, OPEN_PARENTHESE, CLOSE_PARENTHESE
        }

        int GetPriority(Operator op)
        {
            switch (op)
            {
                case Operator.PLUS: return 1;
                case Operator.MINUS: return 1;
                case Operator.MULTIPLY: return 2;
                case Operator.DIVIDE: return 2;
                case Operator.OPEN_PARENTHESE: return 0;
                case Operator.CLOSE_PARENTHESE: return 3;
            }

            return 0;
        }

        Operator StringToOperator(string op)
        {
            op = op.Replace(" ", "");
            if (op == "+") return Operator.PLUS;
            if (op == "-") return Operator.MINUS;
            if (op == "*") return Operator.MULTIPLY;
            if (op == "/") return Operator.DIVIDE;
            if (op == "(") return Operator.OPEN_PARENTHESE;
            if (op == ")") return Operator.CLOSE_PARENTHESE;

            return Operator.UNKNOWN;
        }

        public bool TryParse(string value, out double result)
        {
            // Try parsing in the current culture
            if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                // Then try in US english
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                // Then in neutral language
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return false;
            }

            return true;
        }

        // Splits given expression and returns tokens as array of strings
        string[] GetTokens(string expression)
        {
            List<string> tokens = new List<string>();

            string currentToken = "";
            bool lastOperator = true;
            bool nextOperandIsNegative = false;

            for (int i = 0; i < expression.Length; i++)
            {
                char currentChar = expression[i];
                if (Char.IsDigit(currentChar) || currentChar == '.' || currentChar == ',')
                {
                    currentToken += currentChar;
                    lastOperator = false;
                }
                else
                {
                    if (currentToken != "")
                    {
                        if (nextOperandIsNegative)
                        {
                            tokens.Add("-" + currentToken);
                            nextOperandIsNegative = false;
                        }
                        else
                            tokens.Add(currentToken);
                    }

                    if (currentChar == '-' && lastOperator)
                        nextOperandIsNegative = true;

                    if (!Char.IsWhiteSpace(currentChar) && currentChar != ')')
                        lastOperator = true;

                    if (!Char.IsWhiteSpace(currentChar) && !nextOperandIsNegative)
                        tokens.Add(currentChar.ToString());

                    currentToken = "";
                }
            }

            if (currentToken != "") // Add last constructed token
            {
                if (nextOperandIsNegative)
                    tokens.Add("-" + currentToken);
                else
                    tokens.Add(currentToken);
            }

            return tokens.ToArray();
        }

        public double Calculate(string expression)
        {
            Stack<Operator> operators = new Stack<Operator>();
            Stack<double> operands = new Stack<double>();

            string[] tokens = GetTokens(expression);

            for (int i = 0; i < tokens.Length; i++)
            {
                if (TryParse(tokens[i], out double num)) // Next token is a number
                {
                    operands.Push(num);
                }
                else // Next token is an operator
                {
                    Operator currentOperator = StringToOperator(tokens[i]);
                    if (currentOperator == Operator.OPEN_PARENTHESE)
                    {
                        // Do nothing
                    }
                    else if (currentOperator == Operator.CLOSE_PARENTHESE)
                    {
                        // Calculate until we reach open paranthese
                        while (operators.Peek() != Operator.OPEN_PARENTHESE)
                        {
                            double second = operands.Pop();
                            double first = operands.Pop();
                            Operator stackOperator = operators.Pop();
                            switch (stackOperator)
                            {
                                case Operator.PLUS: operands.Push(first + second); break;
                                case Operator.MINUS: operands.Push(first - second); break;
                                case Operator.MULTIPLY: operands.Push(first * second); break;
                                case Operator.DIVIDE: operands.Push(first / second); break;
                                default: Console.WriteLine("Error: Unknown operator"); break;
                            }
                        }
                        operators.Pop();
                    }
                    else
                    {
                        // Calculate operators with higher priority
                        while (operators.Count != 0 && GetPriority(currentOperator) <= GetPriority(operators.Peek()))
                        {
                            double second = operands.Pop();
                            double first = operands.Pop();
                            Operator stackOperator = operators.Pop();
                            switch (stackOperator)
                            {
                                case Operator.PLUS: operands.Push(first + second); break;
                                case Operator.MINUS: operands.Push(first - second); break;
                                case Operator.MULTIPLY: operands.Push(first * second); break;
                                case Operator.DIVIDE: operands.Push(first / second); break;
                                default: Console.WriteLine("Error: Unknown operator"); break;
                            }
                        }
                    }

                    if (currentOperator != Operator.CLOSE_PARENTHESE)
                        operators.Push(currentOperator);
                }
            }

            while (operators.Count != 0)
            {
                Operator currentOperator = operators.Pop();
                double second = operands.Pop();
                double first = operands.Pop();
                switch (currentOperator)
                {
                    case Operator.PLUS: operands.Push(first + second); break;
                    case Operator.MINUS: operands.Push(first - second); break;
                    case Operator.MULTIPLY: operands.Push(first * second); break;
                    case Operator.DIVIDE: operands.Push(first / second); break;
                    default: Console.WriteLine("Error: Unknown operator"); break;
                }
            }

            double result = operands.Pop();

            return result;
        }

        public Calculator()
        {

        }
    }
}
