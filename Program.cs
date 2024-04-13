namespace RPN;

internal class Program
{
	public static void Main(string[] args)
	{
		RPNMachine machine = new();
		int baseTop = Console.CursorTop;
		int oldTop = 0;
		try
		{
			while (true)
			{
				var operands = machine.Operands.ToArray();
				for (int i = operands.Length - 1; i >= 0; i--)
				{
					double operand = operands[i];
					if (i < operands.Length - 1) Console.Write("|");
					Console.Write(operand.ToString());
				}

				Console.Write(">");
				Console.CursorVisible = true;
				string? input = Console.ReadLine();
				Console.CursorVisible = false;
				if (input is null)
				{
					Console.WriteLine();
					Console.WriteLine();
					throw new EndProgramException();
				}
				else
				{
					int tmpTop = Console.CursorTop;
					for (; Console.CursorTop < oldTop; Console.CursorTop++)
					{
						ClearLine();
					}
					Console.CursorTop = tmpTop;
					Console.ForegroundColor = ConsoleColor.DarkGray;
					double result = machine.Eval(input);
					Console.ResetColor();
					Console.WriteLine(result);
				}

				oldTop = Console.CursorTop;
				Console.CursorTop = baseTop;
				ClearLine();
			}
		}
		catch (EndProgramException)
		{
			Console.Clear();
			Console.ResetColor();
			Console.CursorVisible = true;
		}
	}

	private static void ClearLine()
	{
		for (int i = 0; i < Console.BufferWidth; i++) Console.Write(" ");
		Console.CursorLeft = 0;
	}
}

class EndProgramException : Exception { }


public class RPNMachine
{
	public Stack<double> Operands { get; private init; } = new();

	private readonly string[] Operators = ["*", "+", "-", "/", "^"];

	public double Eval(string expression)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(expression))
			{
				string[] tokens = expression.Split();
				foreach (string token in tokens)
				{
					switch (token)
					{
						case "break":
						case "exit":
						case "quit":
						case "bye":
							throw new EndProgramException();
						case "clear":
							Operands.Clear();
							return double.NaN;
					}

					if (string.IsNullOrWhiteSpace(token)) continue;
					if (Operators.Contains(token))
					{
						double op1, op2;
						Operands.TryPop(out op2);
						Operands.TryPop(out op1);
						Console.Write($"Pusing {op1} {token} {op2} = ");
						switch (token)
						{
							case "*":
								Operands.Push(op1 * op2);
								break;
							case "+":
								Operands.Push(op1 + op2);
								break;
							case "-":
								Operands.Push(op1 - op2);
								break;
							case "/":
								Operands.Push(op1 / op2);
								break;
							case "^":
								Operands.Push(Math.Pow(op1, op2));
								break;
						}

						Console.WriteLine(Operands.Peek());
					}
					else
					{
						double d;
						switch (token.ToLower())
						{
							case "+pi":
							case "pi":
								d = Math.PI;
								break;
							case "-pi":
								d = -Math.PI;
								break;
							case "+e":
							case "e":
								d = Math.E;
								break;
							case "-e":
								d = -Math.E;
								break;
							case "+ans":
							case "ans":
								Operands.TryPeek(out d);
								break;
							case "-ans":
								Operands.TryPeek(out d);
								d = -d;
								break;
							case "+infinity":
							case "infinity":
								d = double.PositiveInfinity;
								break;
							case "-infinity":
								d = double.NegativeInfinity;
								break;
							default:
								d = double.Parse(token);
								break;
						}
						Console.WriteLine($"Pushing {d}");
						Operands.Push(d);
					}
				}
			}
		}
		catch (Exception e)
		{
			if (e.GetType() == typeof(EndProgramException)) throw;
			Console.WriteLine("Error: " + e.Message);
		}
		return Operands.TryPeek(out double outDouble) ? outDouble : double.NaN;
	}
}