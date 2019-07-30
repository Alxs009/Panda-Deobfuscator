using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using PandaDeobfuscator.Protections;
using PandaDeobfuscator.Protections.Melt;
using PandaDeobfuscator.Protections.De4dot;
using PandaDeobfuscator.Protections.ReferenceProxy;
using PandaDeobfuscator.Protections.StringDecoder;

namespace PandaDeobfuscator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Panda Deobfuscator - coded by Alxs009";
            ModuleDefMD moduleDefMD = null;
            Logger.Push("Input assembly: ", Logger.TypeLine.Default);
            string assembly = Console.ReadLine();
            try
            {
                moduleDefMD = ModuleDefMD.Load(assembly);
                Logger.Push($"Loaded assembly: {moduleDefMD.FullName}");
            }
            catch { Logger.Push("Failed load..."); return; }
            if (!moduleDefMD.GlobalType.NestedTypes.Any(x => x.Name.Contains("Panda")))
            {
                Logger.Push("Other obfuscator");
                goto delay;
            }
            var iTask = new List<ITask> { new De4dotCleaner(), new StringMelt(), new RefProxy(), new StringDecoder(), new De4dotCleaner() };
            foreach (var task in iTask)
                task.Execute(moduleDefMD);
            SaveModule(moduleDefMD);
        delay:
            Logger.Push("Done...");
            Console.ReadKey(true);
        }
        private static void SaveModule(ModuleDefMD moduleDefMD)
        {
            string fileName = Path.GetDirectoryName(moduleDefMD.Location) + "\\" + Path.GetFileNameWithoutExtension(moduleDefMD.Location) + "_deobfuscated" + Path.GetExtension(moduleDefMD.Location);
            moduleDefMD.Write(fileName, new ModuleWriterOptions { Logger = DummyLogger.NoThrowInstance /* No show error message */});
            Logger.Push($"Saved at: {fileName}");
        }
    }
}
