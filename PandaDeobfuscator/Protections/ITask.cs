using dnlib.DotNet;

namespace PandaDeobfuscator.Protections
{
    interface ITask
    {
        void Execute(ModuleDefMD moduleDefMD);
    }
}
