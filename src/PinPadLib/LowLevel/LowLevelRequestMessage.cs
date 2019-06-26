using System;
using System.Collections.Generic;
using System.Linq;

namespace PinPadLib.LowLevel
{
    public class LowLevelRequestMessage
    {
        public LowLevelRequestMessage(CommandName command, params string[] parameters)
        {
            if (command == default)
                throw new ArgumentException("Invalid command", nameof(command));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.Any(x => x == null))
                throw new ArgumentException("Parameters must not be null", nameof(parameters));

            Command = command;
            Parameters = parameters;
        }
        public CommandName Command { get; }
        public IEnumerable<string> Parameters { get; }

        public LowLevelRequestMessage With(CommandName? command = null, IEnumerable<string> parameters = null)
        {
            command = command ?? Command;
            parameters = parameters ?? Parameters;

            return new LowLevelRequestMessage(command.Value, parameters.ToArray());
        }
    }
}
