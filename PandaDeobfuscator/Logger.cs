using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandaDeobfuscator
{
    class Logger
    {
        public static void Push(string message, TypeLine typeLine = TypeLine.NewLine)
        {
            switch (typeLine)
            {
                case TypeLine.Default:
                    Console.Write($"[{DateTime.Now}]: {message}");
                    break;
                case TypeLine.NewLine:
                    Console.WriteLine($"[{DateTime.Now}]: {message}");
                    break;
            }
        }
        public enum TypeLine
        {
            Default,
            NewLine
        }
    }
}
