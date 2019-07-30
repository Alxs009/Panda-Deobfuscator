using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace PandaDeobfuscator.Protections.ReferenceProxy
{
    sealed class RefProxy : ITask
    {
        public void Execute(ModuleDefMD moduleDefMD)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Push($"{nameof(RefProxy)} working...");
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
                        switch (instructions[i].OpCode.OperandType)
                        {
                            case OperandType.InlineMethod:
                                if (instructions[i].Operand is MethodDef refMethod && refMethod.Body.Instructions.Where(x => x.OpCode == OpCodes.Newobj).Any() && refMethod.HasReturnType && refMethod.IsStatic && refMethod.IsHideBySig)
                                {
                                    instructions[i].OpCode = OpCodes.Newobj;
                                    instructions[i].Operand = refMethod.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj).Operand;
                                    junkMethod.Add(refMethod);
                                    countFixed++;
                                }
                                else if (instructions[i].Operand is MethodDef setMethod && !setMethod.HasReturnType && setMethod.IsHideBySig && setMethod.Body.Instructions.Count == 4)
                                {
                                    instructions[i].OpCode = OpCodes.Stfld;
                                    instructions[i].Operand = setMethod.Body.Instructions.Single(x => x.OpCode == OpCodes.Stfld).Operand;
                                    junkMethod.Add(setMethod);
                                    countFixed++;
                                }
                                else if (instructions[i].Operand is MethodDef refsMethod && refsMethod.IsStatic && refsMethod.ReturnType == moduleDefMD.ImportAsTypeSig(typeof(int)))
                                {
                                    instructions[i].OpCode = OpCodes.Ldc_I4;
                                    instructions[i].Operand = refsMethod.Body.Instructions.First().GetLdcI4Value();
                                    junkMethod.Add(refsMethod);
                                    countFixed++;
                                }
                                break;
                        }
                    }
                }
                foreach (var methodDef in junkMethod)
                    typeDef.Methods.Remove(methodDef);
                junkMethod.Clear();
            }
            Logger.Push($"{nameof(RefProxy)} fixed: {countFixed}");
        }
    }
}
