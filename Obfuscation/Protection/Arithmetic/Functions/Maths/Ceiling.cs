﻿using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscation.Protection.Arithmetic.Utils;

namespace Obfuscation.Protection.Arithmetic.Functions.Maths
{
    public class Ceiling : Function
    {
        public virtual ArithmeticTypes ArithmeticTypes => ArithmeticTypes.Ceiling;

        public override ArithmeticVt Arithmetic(Instruction instruction, ModuleDef module)
        {
            if (!ArithmeticUtils.CheckArithmetic(instruction)) return null!;
            var arithmeticTypes = new List<ArithmeticTypes> { ArithmeticTypes.Add, ArithmeticTypes.Sub };
            var arithmeticEmulator = new ArithmeticEmulator(instruction.GetLdcI4Value(), ArithmeticUtils.GetY(instruction.GetLdcI4Value()), ArithmeticTypes);
            return new ArithmeticVt(new Value(arithmeticEmulator.GetValue(arithmeticTypes), arithmeticEmulator.GetY()), new Token(ArithmeticUtils.GetOpCode(arithmeticEmulator.GetType), module.Import(ArithmeticUtils.GetMethod(ArithmeticTypes))), ArithmeticTypes);
        }
    }
}