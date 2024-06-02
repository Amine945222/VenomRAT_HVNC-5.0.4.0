using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Obfuscation.Protection.CtrlFlow
{
    public class Block
    {
        public Block()
        {
            Instructions = new List<Instruction>();
        }

        public List<Instruction> Instructions { get; set; }

        public int Number { get; set; }
    }
}