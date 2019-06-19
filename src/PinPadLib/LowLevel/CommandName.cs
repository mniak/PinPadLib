using System;
using System.Collections.Generic;

namespace PinPadLib.LowLevel
{
    public struct CommandName : IEquatable<CommandName>
    {
        internal readonly string acronym;
        private CommandName(string acronym)
        {
            this.acronym = acronym ?? throw new ArgumentNullException(nameof(acronym));
        }

        public static CommandName ChipDirect = new CommandName("CHP");
        public static CommandName ChangeParameter = new CommandName("CNG");
        public static CommandName Open = new CommandName("OPN");
        public static CommandName Close = new CommandName("CLO");
        public static CommandName Display = new CommandName("DSP");
        public static CommandName DisplayEx = new CommandName("DEX");
        public static CommandName GetKey = new CommandName("GKY");
        public static CommandName GetPIN = new CommandName("GPN");
        public static CommandName RemoveCard = new CommandName("RMC");
        public static CommandName GenericCmd = new CommandName("GEN");
        public static CommandName CheckEvent = new CommandName("CKE");
        public static CommandName GetCard = new CommandName("GCR");
        public static CommandName GoOnChip = new CommandName("GOC");
        public static CommandName FinishChip = new CommandName("FNC");
        public static CommandName GetInfo = new CommandName("GIN");
        public static CommandName EncryptBuffer = new CommandName("ENB");
        public static CommandName TableLoadInit = new CommandName("TLI");
        public static CommandName TableLoadRec = new CommandName("TLR");
        public static CommandName TableLoadEnd = new CommandName("TLE");
        public static CommandName GetDUKPT = new CommandName("GDU");
        public static CommandName GetTimeStamp = new CommandName("GTS");
        public static CommandName DefineWKPAN = new CommandName("DWK");

        public bool Equals(CommandName other)
        {
            return other.acronym == this.acronym;
        }
        public override bool Equals(object obj)
        {
            if (obj is CommandName cn)
                return Equals(cn);
            return false;
        }

        public override int GetHashCode()
        {
            return 330845882 + EqualityComparer<string>.Default.GetHashCode(this.acronym);
        }

        public static bool operator ==(CommandName left, CommandName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CommandName left, CommandName right)
        {
            return !(left == right);
        }
    }
}
