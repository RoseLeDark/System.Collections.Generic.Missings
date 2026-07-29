using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.AI.Tools {
    public sealed class DateTimeTool : IModelTool<string> {
        public string Name => "get_current_datetime";
        public string Description => "Returns the current date and time.";

        public IEnumerable<ModelToolParameter> GetParameters () {
            yield return new ModelToolParameter(
                "useUtc",
                typeof(bool),
                "If true, returns UTC time; otherwise local time."
            );
        }

        public async Task<object?> ExecuteAsync (
            Dictionary<string, object?> args,
            CancellationToken ct ) {
            bool useUtc = args.TryGetValue("useUtc", out var v) && v is bool b && b;

            return useUtc
                ? DateTime.UtcNow.ToString("o")
                : DateTime.Now.ToString("o");
        }
    }
}
