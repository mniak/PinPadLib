using System;
using System.Collections.Generic;
using System.Linq;

namespace PinPadLib.LowLevel
{
    public class LowLevelResponseMessage
    {
        public LowLevelResponseMessage(params string[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.Any(x => x == null))
                throw new ArgumentException("Parameters must not be null", nameof(parameters));

            Parameters = parameters;
        }
        public CommandName Command { get; }
        public IEnumerable<string> Parameters { get; set; }
    }
}
