﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TED
{
    [DebuggerDisplay("{Name}")]
    public class Simulation
    {
        public static Simulation? Current;

        public readonly string Name;

        public readonly List<TablePredicate> Tables = new List<TablePredicate>();
        public void AddTable(TablePredicate t) => Tables.Add(t);

        public static void AddToCurrentSimulation(TablePredicate t)
        {
            Current?.AddTable(t);
        }

        public Simulation(string name)
        {
            Name = name;
            Current = this;
        }

        #if PROFILER
        /// <summary>
        /// Average combined execution time for all the rules in the simulation
        /// </summary>
        public float RuleExecutionTime => Tables.Select(t => t.RuleExecutionTime).Sum();
        #endif

        public override string ToString() => Name;

        public void Update()
        {
            RecomputeAll();
            AppendAllInputs();
        }

        /// <summary>
        /// Forcibly recompute all the predicates
        /// </summary>
        public void RecomputeAll()
        {
            foreach (var p in Tables)
                if (p.IsIntensional)
                    p.MustRecompute = true;
            foreach (var p in Tables)
                p.EnsureUpToDate();
        }

        
        public void AppendAllInputs()
        {
            foreach (var p in Tables)
                p.AppendInputs();
        }
    }
}
