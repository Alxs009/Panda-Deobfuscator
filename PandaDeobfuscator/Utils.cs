using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandaDeobfuscator
{
    class Utils
    {
        public static void NopInstructions(params Instruction[] instructions)
        {
            foreach (var instruction in instructions)
                instruction.OpCode = OpCodes.Nop;
        }
    }
}
