using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscation.Protection.Arithmetic
{
    public abstract class Function
    {
        public abstract ArithmeticVt Arithmetic(Instruction instruction, ModuleDef module);
    }
}