using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.AI.Tools {
    public sealed class DateDifferenceTool : IModelTool<string> {
        public string Name => "calculate_date_difference";
        public string Description => "Calculates the difference between two dates in days.";

        public IEnumerable<ModelToolParameter> GetParameters () {
            yield return new ModelToolParameter(
                "start",
                typeof(string),
                "Start date in ISO format."
            );

            yield return new ModelToolParameter(
                "end",
                typeof(string),
                "End date in ISO format."
            );
        }

        public async Task<object?> ExecuteAsync (
            Dictionary<string, object?> args,
            CancellationToken ct ) {
            if ( !args.TryGetValue("start", out var s) || s is not string startStr )
                throw new ArgumentException("Missing 'start' parameter.");

            if ( !args.TryGetValue("end", out var e) || e is not string endStr )
                throw new ArgumentException("Missing 'end' parameter.");

            var start = DateTime.Parse(startStr);
            var end = DateTime.Parse(endStr);

            var diff = (end - start).TotalDays;

            return diff;
        }
    }
}
