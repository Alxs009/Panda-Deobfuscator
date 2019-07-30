using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace PandaDeobfuscator.Protections.StringDecoder
{
    sealed class StringDecoder : ITask
    {
        public void Execute(ModuleDefMD moduleDefMD)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Push($"{nameof(StringDecoder)} working...");
            Console.ForegroundColor = ConsoleColor.Gray;

            int countFixed = 0;
            foreach (var typeDef in moduleDefMD.GetTypes())
            {
                foreach (var methodDef in typeDef.Methods.Where(x => x.HasBody && x.Body.Instructions.Count > 3))
                {
                    var instructions = methodDef.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        /*
                           Call	class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
                           Ldstr	"Base64"
                           Call	uint8[] [mscorlib]System.Convert::FromBase64String(string)
                           Callvirt	instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
                         */
                        if (instructions[i].OpCode == OpCodes.Call
                            && instructions[i].Operand.ToString().Contains("Encoding::get_UTF8")
                            && instructions[i + 1].OpCode == OpCodes.Ldstr
                            && instructions[i + 2].OpCode == OpCodes.Call
                            && instructions[i + 3].OpCode == OpCodes.Callvirt)
                        {
                            string decryptedStr = Encoding.UTF8.GetString(Convert.FromBase64String(instructions[i + 1].Operand.ToString()));
                            instructions[i].OpCode = OpCodes.Ldstr;
                            instructions[i].Operand = decryptedStr;
                            Utils.NopInstructions(instructions[i + 1], instructions[i + 2], instructions[i + 3]);
                            countFixed++;
                        }
                    }
                }
            }
            Logger.Push($"Count decrypted strings: {countFixed}");
        }
    }
}
