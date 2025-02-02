﻿using System;
using System.Diagnostics;

namespace TED
{
    /// <summary>
    /// Represents a variable in a Goal (abstract syntax tree of a call to a predicate)
    /// This is not the run-time representation of a variable.  That is held in a ValueCell.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{DebugName}")]
    public class Var<T> : Term<T>
    {
        /// <summary>
        /// Make a new variable
        /// </summary>
        /// <param name="name">Human-readable name of the variable</param>
        public Var(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the variable, for debugging purposes
        /// </summary>
        public readonly string Name;
        
        /// <summary>
        /// Yes, this is a variable
        /// </summary>
        public override bool IsVariable => true;
        
        /// <summary>
        /// Make a TED variable of the specified type and name
        /// </summary>
        /// <param name="s"></param>
        public static explicit operator Var<T>(string s) => new Var<T>(s);

        internal override Term<T> ApplySubstitution(Substitution s) => s.SubstituteVariable(this);

        internal override Func<T> MakeEvaluator(GoalAnalyzer ga)
        {
            var ma = ga.Emit(this);
            if (!ma.IsInstantiated)
                throw new InstantiationException(
                    $"Variable {this} used in a functional expression before it is bound to a value.");
            var cell = ma.ValueCell;
            return () => cell.Value;
        }

        public override string ToString() => Name;

        public string DebugName => ToString();

        /// <summary>
        /// Make a different variable with the same name and type
        /// </summary>
        public AnyTerm Clone() => new Var<T>(Name);
    }
}
