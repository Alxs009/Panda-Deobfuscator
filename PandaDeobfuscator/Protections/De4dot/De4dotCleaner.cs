using System;
using System.Linq;
using dnlib.DotNet;
using de4dot.blocks.cflow;

namespace PandaDeobfuscator.Protections.De4dot
{
    sealed class De4dotCleaner : ITask
    {
        public void Execute(ModuleDefMD moduleDefMD)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Push($"{nameof(De4dotCleaner)} working...");
            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (var typeDef in moduleDefMD.GetTypes())
            {
                foreach (var methodDef in typeDef.Methods.Where(x => x.HasBody))
                {
                    new CflowDeobfuscator().Deobfuscate(methodDef);
                }
            }
        }
    }
}
