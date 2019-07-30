using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace PandaDeobfuscator.Protections.Melt
{
    sealed class StringMelt : ITask
    {
        public void Execute(ModuleDefMD moduleDefMD)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Push($"{nameof(StringMelt)} working...");
            Console.ForegroundColor = ConsoleColor.Gray;

            int countFixed = 0;
            var junkMethod = new List<MethodDef>();
            foreach (var typeDef in moduleDefMD.GetTypes())
            {
                foreach (var methodDef in typeDef.Methods.Where(x => x.HasBody))
                {
                    var instructions = methodDef.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode == OpCodes.Call
                            && instructions[i].Operand is MethodDef refMethod && refMethod.ReturnType == moduleDefMD.ImportAsTypeSig(typeof(string)) && refMethod.IsStatic && refMethod.Body.Instructions.Count == 2)
                        {
                            instructions[i].OpCode = OpCodes.Ldstr;
                            instructions[i].Operand = refMethod.Body.Instructions.First().Operand.ToString();
                            junkMethod.Add(refMethod);
                            countFixed++;
                        }
                    }
                }
                foreach (var methodDef in junkMethod)
                    typeDef.Methods.Remove(methodDef);
                junkMethod.Clear();
            }
            Logger.Push($"{nameof(StringMelt)} fixed: {countFixed}");
        }
    }
}
