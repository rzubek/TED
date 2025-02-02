﻿using System;
using System.Reflection;

namespace TED
{
    /// <summary>
    /// Wrapper for C# comparison operators (<, >, etc.) for some type.
    /// </summary>
    internal sealed class ComparisonPrimitive<T> : PrimitivePredicate<T,T>
    {
        internal static readonly ComparisonPrimitive<T> LessThan = new ComparisonPrimitive<T>("<", "op_LessThan");
        internal static readonly ComparisonPrimitive<T> LessThanEq = new ComparisonPrimitive<T>("<=", "op_LessThanOrEqual");
        internal static readonly ComparisonPrimitive<T> GreaterThan = new ComparisonPrimitive<T>(">", "op_GreaterThan");
        internal static readonly ComparisonPrimitive<T> GreaterThanEq = new ComparisonPrimitive<T>(">=", "op_GreaterThanOrEqual");

        private readonly Func<T, T, bool> comparison;

        private ComparisonPrimitive(string name, string operatorName) : base(name)
        {
            var type = typeof(T);
            var methodInfo = type.GetOperatorMethodInfo(operatorName, typeof(bool), type, type);
            if (methodInfo == null)
                throw new ArgumentException($"There is no {name} overload defined for comparing two {type.Name}s");
            comparison = (Func<T, T, bool>)methodInfo.CreateDelegate(typeof(Func<T, T, bool>));
        }

        public override AnyCall MakeCall(Goal g, GoalAnalyzer tc)
        {
            var i1 = tc.Emit(g.Arg1);
            var i2 = tc.Emit(g.Arg2);
            if (!i1.IsInstantiated)
                throw new InstantiationException(this, g.Arg1);
            if (!i2.IsInstantiated)
                throw new InstantiationException(this, g.Arg2);
            return new Call(this, i1.ValueCell, i2.ValueCell, comparison);
        }

        private class Call : AnyCall
        {
            private readonly ValueCell<T> arg1;
            private readonly ValueCell<T> arg2;
            private readonly Func<T, T, bool> test;
            private bool ready;

            public override IPattern ArgumentPattern =>
                new Pattern<T, T>(MatchOperation<T>.Read(arg1), MatchOperation<T>.Read(arg2));

            public Call(ComparisonPrimitive<T> predicate, ValueCell<T> arg1, ValueCell<T> arg2, Func<T, T, bool> test) : base(predicate)
            {
                this.arg1 = arg1;
                this.arg2 = arg2;
                this.test = test;
            }

            public override void Reset()
            {
                ready = true;
            }

            public override bool NextSolution()
            {
                if (!ready) return false;
                ready = false;
                return test(arg1.Value, arg2.Value);
            }
        }
    }

}
