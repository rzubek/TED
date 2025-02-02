﻿namespace TED
{
    /// <summary>
    /// Implements negation of a goal
    /// </summary>
    internal sealed class MaximalPrimitive<T1> : AnyPredicate
    {
        public static MaximalPrimitive<T1> Maximal = new MaximalPrimitive<T1>(1);
        public static MaximalPrimitive<T1> Minimal = new MaximalPrimitive<T1>(-1);

        private readonly int Multiplier;

        public MaximalPrimitive(int multiplier) : base("Maximal")
        {
            Multiplier = multiplier;
        }

        public Goal this[Var<T1> arg, Var<float> utility, AnyGoal g] => new Goal(this, arg, utility, new Constant<AnyGoal>(g));

        public class Goal : AnyGoal
        {
            private readonly MaximalPrimitive<T1> predicate;
            private readonly Var<T1> Arg;
            private readonly Var<float> Utility;
            private readonly AnyGoal Generator;

            public Goal(MaximalPrimitive<T1> predicate, Var<T1> arg, Var<float> utility, Constant<AnyGoal> g) : base(new AnyTerm[] { arg, utility, g })
            {
                Arg = arg;
                Utility = utility;
                this.predicate = predicate;
                Generator = g.Value;
            }

            public override AnyPredicate Predicate => predicate;

            internal override AnyGoal RenameArguments(Substitution s)
                => new Goal(predicate, (Var<T1>)s.SubstituteVariable(Arg), (Var<float>)s.SubstituteVariable(Utility), new Constant<AnyGoal>(Generator.RenameArguments(s)));

            internal override AnyCall MakeCall(GoalAnalyzer ga)
            {
                if (ga.IsInstantiated(Arg))
                    throw new InstantiationException(Maximal, Arg);
                if (ga.IsInstantiated(Utility))
                    throw new InstantiationException(Maximal, Utility);

                var call = Generator.MakeCall(ga);

                if (!ga.IsInstantiated(Arg))
                    throw new InstantiationException($"Goal {Generator} does not bind the variable {Arg} in call {this}");
                if (!ga.IsInstantiated(Utility))
                    throw new InstantiationException($"Goal {Generator} does not bind the variable {Utility} in call {this}");

                return new Call(predicate, ga.Emit(Arg), ga.Emit(Utility), call);
            }
        }

        private class Call : AnyCall
        {
            private readonly MaximalPrimitive<T1> predicate;
            private readonly MatchOperation<T1> Arg;
            private readonly MatchOperation<float> Utility;
            private readonly AnyCall Goal;
            private bool restart;

            public Call(MaximalPrimitive<T1> predicate, MatchOperation<T1> arg, MatchOperation<float> utility, AnyCall call) : base(Maximal)
            {
                Arg = arg;
                Utility = utility;
                Goal = call;
                this.predicate = predicate;
            }

            public override IPattern ArgumentPattern => new Pattern<T1, float, AnyCall>(Arg, Utility,
                MatchOperation<AnyCall>.Constant(Goal));

            public override void Reset()
            {
                restart = true;
            }

            public override bool NextSolution()
            {
                if (!restart) return false;
                restart = false;
                Goal.Reset();
                var gotOne = false;
                var argCell = Arg.ValueCell;
                var utilityCell = Utility.ValueCell;
                var bestArg = argCell.Value;
                var bestUtil = utilityCell.Value;
                while (Goal.NextSolution())
                {
                    var u = utilityCell.Value * predicate.Multiplier;
                    if (!gotOne || u > bestUtil)
                    {
                        bestArg = argCell.Value;
                        bestUtil = u;
                    }

                    gotOne = true;
                }

                if (gotOne)
                {
                    argCell.Value = bestArg;
                    utilityCell.Value = bestUtil * predicate.Multiplier;
                }

                return gotOne;
            }
        }
    }

    /// <summary>
    /// Implements negation of a goal
    /// </summary>
    public sealed class MaximalPrimitive<T1, T2> : AnyPredicate
    {
        public static MaximalPrimitive<T1, T2> Maximal = new MaximalPrimitive<T1, T2>(1);
        public static MaximalPrimitive<T1, T2> Minimal = new MaximalPrimitive<T1, T2>(-1);

        private readonly int Multiplier;

        public MaximalPrimitive(int multiplier) : base("Maximal")
        {
            Multiplier = multiplier;
        }

        public Goal this[Var<T1> arg1, Var<T2> arg2, Var<float> utility, AnyGoal g] => new Goal(this, arg1, arg2, utility, new Constant<AnyGoal>(g));

        public class Goal : AnyGoal
        {
            private readonly MaximalPrimitive<T1, T2> predicate;
            private readonly Var<T1> Arg1;
            private readonly Var<T2> Arg2;
            private readonly Var<float> Utility;
            private readonly AnyGoal Generator;

            public Goal(MaximalPrimitive<T1, T2> predicate, Var<T1> arg1, Var<T2> arg2, Var<float> utility, Constant<AnyGoal> g) : base(new AnyTerm[] { arg1, utility, g })
            {
                Arg1 = arg1;
                Arg2 = arg2;
                Utility = utility;
                this.predicate = predicate;
                Generator = g.Value;
            }

            public override AnyPredicate Predicate => predicate;

            internal override AnyGoal RenameArguments(Substitution s)
                => new Goal(predicate, (Var<T1>)s.SubstituteVariable(Arg1), (Var<T2>)s.SubstituteVariable(Arg2), (Var<float>)s.SubstituteVariable(Utility), new Constant<AnyGoal>(Generator.RenameArguments(s)));

            internal override AnyCall MakeCall(GoalAnalyzer ga)
            {
                if (ga.IsInstantiated(Arg1))
                    throw new InstantiationException(Maximal, Arg1);
                if (ga.IsInstantiated(Arg2))
                    throw new InstantiationException(Maximal, Arg2);
                if (ga.IsInstantiated(Utility))
                    throw new InstantiationException(Maximal, Utility);

                var call = Generator.MakeCall(ga);

                if (!ga.IsInstantiated(Arg1))
                    throw new InstantiationException($"Goal {Generator} does not bind the variable {Arg1} in call {this}");
                if (!ga.IsInstantiated(Arg2))
                    throw new InstantiationException($"Goal {Generator} does not bind the variable {Arg2} in call {this}");
                if (!ga.IsInstantiated(Utility))
                    throw new InstantiationException($"Goal {Generator} does not bind the variable {Utility} in call {this}");

                return new Call(predicate, ga.Emit(Arg1), ga.Emit(Arg2), ga.Emit(Utility), call);
            }
        }

        private class Call : AnyCall
        {
            private readonly MaximalPrimitive<T1, T2> predicate;
            private readonly MatchOperation<T1> Arg1;
            private readonly MatchOperation<T2> Arg2;
            private readonly MatchOperation<float> Utility;
            private readonly AnyCall Goal;
            private bool restart;

            public Call(MaximalPrimitive<T1, T2> predicate, MatchOperation<T1> arg1, MatchOperation<T2> arg2, MatchOperation<float> utility, AnyCall call) : base(Maximal)
            {
                Arg1 = arg1;
                Arg2 = arg2;
                Utility = utility;
                Goal = call;
                this.predicate = predicate;
            }

            public override IPattern ArgumentPattern => new Pattern<T1, T2, float, AnyCall>(Arg1, Arg2, Utility,
                MatchOperation<AnyCall>.Constant(Goal));

            public override void Reset()
            {
                restart = true;
            }

            public override bool NextSolution()
            {
                if (!restart) return false;
                restart = false;
                Goal.Reset();
                var gotOne = false;
                var arg1Cell = Arg1.ValueCell;
                var arg2Cell = Arg2.ValueCell;
                var utilityCell = Utility.ValueCell;
                var bestArg1 = arg1Cell.Value;
                var bestArg2 = arg2Cell.Value;
                var bestUtil = utilityCell.Value;
                while (Goal.NextSolution())
                {
                    var u = utilityCell.Value * predicate.Multiplier;
                    if (!gotOne || u > bestUtil)
                    {
                        bestArg1 = arg1Cell.Value;
                        bestArg2 = arg2Cell.Value;
                        bestUtil = u;
                    }

                    gotOne = true;
                }

                if (gotOne)
                {
                    arg1Cell.Value = bestArg1;
                    arg2Cell.Value = bestArg2;
                    utilityCell.Value = bestUtil * predicate.Multiplier;
                }

                return gotOne;
            }
        }
    }
}
