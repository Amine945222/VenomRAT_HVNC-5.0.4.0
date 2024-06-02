using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscation.Protection.Arithmetic.Utils;

namespace Obfuscation.Protection.Arithmetic.Functions
{
    public class Div : Function
    {
        public virtual ArithmeticTypes ArithmeticTypes => ArithmeticTypes.Div;

        public override ArithmeticVt Arithmetic(Instruction instruction, ModuleDef module)
        {
            if (!ArithmeticUtils.CheckArithmetic(instruction)) return null!;
            var arithmeticEmulator = new ArithmeticEmulator(instruction.GetLdcI4Value(), ArithmeticUtils.GetY(instruction.GetLdcI4Value()), ArithmeticTypes);
            return new ArithmeticVt(new Value(arithmeticEmulator.GetValue(), arithmeticEmulator.GetY()), new Token(OpCodes.Div), ArithmeticTypes);
        }
    }
}