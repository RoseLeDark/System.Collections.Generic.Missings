using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.AI.Tools {
    public sealed class CalculatorTool : IModelTool<string> {
        public string Name => "calculate";
        public string Description => "Performs basic arithmetic operations.";

        public IEnumerable<ModelToolParameter> GetParameters () {
            yield return new ModelToolParameter(
                "a",
                typeof(double),
                "First number."
            );

            yield return new ModelToolParameter(
                "b",
                typeof(double),
                "Second number."
            );

            yield return new ModelToolParameter(
                "op",
                typeof(string),
                "Operation: add, sub, mul, div."
            );
        }

        public async Task<object?> ExecuteAsync (
            Dictionary<string, object?> args,
            CancellationToken ct ) {
            double a = Convert.ToDouble(args["a"]);
            double b = Convert.ToDouble(args["b"]);
            string op = args["op"]?.ToString() ?? "add";

            return op switch
            {
                "add" => a + b,
                "sub" => a - b,
                "mul" => a * b,
                "div" => b == 0 ? throw new DivideByZeroException() : a / b,
                _ => throw new ArgumentException($"Unknown operation '{op}'.")
            };
        }
    }
}
