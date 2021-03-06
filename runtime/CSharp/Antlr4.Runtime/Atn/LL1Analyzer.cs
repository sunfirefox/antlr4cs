/*
 * [The "BSD license"]
 *  Copyright (c) 2013 Terence Parr
 *  Copyright (c) 2013 Sam Harwell
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *  1. Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 *  2. Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *  3. The name of the author may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 *  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 *  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 *  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 *  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 *  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 *  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 *  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 *  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 *  THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System.Collections.Generic;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Sharpen;

namespace Antlr4.Runtime.Atn
{
    public class LL1Analyzer
    {
        /// <summary>
        /// Special value added to the lookahead sets to indicate that we hit
        /// a predicate during analysis if
        /// <code>seeThruPreds==false</code>
        /// .
        /// </summary>
        public const int HitPred = TokenConstants.InvalidType;

        [NotNull]
        public readonly ATN atn;

        public LL1Analyzer(ATN atn)
        {
            this.atn = atn;
        }

        /// <summary>
        /// Calculates the SLL(1) expected lookahead set for each outgoing transition
        /// of an
        /// <see cref="ATNState">ATNState</see>
        /// . The returned array has one element for each
        /// outgoing transition in
        /// <code>s</code>
        /// . If the closure from transition
        /// <em>i</em> leads to a semantic predicate before matching a symbol, the
        /// element at index <em>i</em> of the result will be
        /// <code>null</code>
        /// .
        /// </summary>
        /// <param name="s">the ATN state</param>
        /// <returns>
        /// the expected symbols for each outgoing transition of
        /// <code>s</code>
        /// .
        /// </returns>
        [return: Nullable]
        public virtual IntervalSet[] GetDecisionLookahead(ATNState s)
        {
            //		System.out.println("LOOK("+s.stateNumber+")");
            if (s == null)
            {
                return null;
            }
            IntervalSet[] look = new IntervalSet[s.NumberOfTransitions];
            for (int alt = 0; alt < s.NumberOfTransitions; alt++)
            {
                look[alt] = new IntervalSet();
                HashSet<ATNConfig> lookBusy = new HashSet<ATNConfig>();
                bool seeThruPreds = false;
                // fail to get lookahead upon pred
                Look(s.Transition(alt).target, null, PredictionContext.EmptyFull, look[alt], lookBusy
                    , new BitSet(), seeThruPreds, false);
                // Wipe out lookahead for this alternative if we found nothing
                // or we had a predicate when we !seeThruPreds
                if (look[alt].Size() == 0 || look[alt].Contains(HitPred))
                {
                    look[alt] = null;
                }
            }
            return look;
        }

        /// <summary>
        /// Compute set of tokens that can follow
        /// <code>s</code>
        /// in the ATN in the
        /// specified
        /// <code>ctx</code>
        /// .
        /// <p/>
        /// If
        /// <code>ctx</code>
        /// is
        /// <code>null</code>
        /// and the end of the rule containing
        /// <code>s</code>
        /// is reached,
        /// <see cref="TokenConstants.Epsilon"/>
        /// is added to the result set.
        /// If
        /// <code>ctx</code>
        /// is not
        /// <code>null</code>
        /// and the end of the outermost rule is
        /// reached,
        /// <see cref="Antlr4.Runtime.TokenConstants.Eof">Antlr4.Runtime.TokenConstants.Eof</see>
        /// is added to the result set.
        /// </summary>
        /// <param name="s">the ATN state</param>
        /// <param name="ctx">
        /// the complete parser context, or
        /// <code>null</code>
        /// if the context
        /// should be ignored
        /// </param>
        /// <returns>
        /// The set of tokens that can follow
        /// <code>s</code>
        /// in the ATN in the
        /// specified
        /// <code>ctx</code>
        /// .
        /// </returns>
        [return: NotNull]
        public virtual IntervalSet Look(ATNState s, PredictionContext ctx)
        {
            return Look(s, null, ctx);
        }

        /// <summary>
        /// Compute set of tokens that can follow
        /// <code>s</code>
        /// in the ATN in the
        /// specified
        /// <code>ctx</code>
        /// .
        /// <p/>
        /// If
        /// <code>ctx</code>
        /// is
        /// <code>null</code>
        /// and the end of the rule containing
        /// <code>s</code>
        /// is reached,
        /// <see cref="TokenConstants.Epsilon"/>
        /// is added to the result set.
        /// If
        /// <see cref="IntStreamConstants.Eof"/>
        /// is not
        /// <code>null</code>
        /// and the end of the outermost rule is
        /// reached,
        /// <see cref="TokenConstants.Eof"/>
        /// is added to the result set.
        /// </summary>
        /// <param name="s">the ATN state</param>
        /// <param name="stopState">
        /// the ATN state to stop at. This can be a
        /// <see cref="BlockEndState">BlockEndState</see>
        /// to detect epsilon paths through a closure.
        /// </param>
        /// <param name="ctx">
        /// the complete parser context, or
        /// <code>null</code>
        /// if the context
        /// should be ignored
        /// </param>
        /// <returns>
        /// The set of tokens that can follow
        /// <code>s</code>
        /// in the ATN in the
        /// specified
        /// <code>ctx</code>
        /// .
        /// </returns>
        [return: NotNull]
        public virtual IntervalSet Look(ATNState s, ATNState stopState, PredictionContext
             ctx)
        {
            IntervalSet r = new IntervalSet();
            bool seeThruPreds = true;
            // ignore preds; get all lookahead
            Look(s, stopState, ctx, r, new HashSet<ATNConfig>(), new BitSet(), seeThruPreds, 
                true);
            return r;
        }

        /// <summary>
        /// Compute set of tokens that can follow
        /// <code>s</code>
        /// in the ATN in the
        /// specified
        /// <code>ctx</code>
        /// .
        /// <p/>
        /// If
        /// <code>ctx</code>
        /// is
        /// <see cref="PredictionContext.EmptyLocal">PredictionContext.EmptyLocal</see>
        /// and
        /// <code>stopState</code>
        /// or the end of the rule containing
        /// <code>s</code>
        /// is reached,
        /// <see cref="TokenConstants.Epsilon"/>
        /// is added to the result set. If
        /// <code>ctx</code>
        /// is not
        /// <see cref="PredictionContext.EmptyLocal">PredictionContext.EmptyLocal</see>
        /// and
        /// <code>addEOF</code>
        /// is
        /// <code>true</code>
        /// and
        /// <code>stopState</code>
        /// or the end of the outermost rule is reached,
        /// <see cref="TokenConstants.Eof"/>
        /// is added to the result set.
        /// </summary>
        /// <param name="s">the ATN state.</param>
        /// <param name="stopState">
        /// the ATN state to stop at. This can be a
        /// <see cref="BlockEndState">BlockEndState</see>
        /// to detect epsilon paths through a closure.
        /// </param>
        /// <param name="ctx">
        /// The outer context, or
        /// <see cref="PredictionContext.EmptyLocal">PredictionContext.EmptyLocal</see>
        /// if
        /// the outer context should not be used.
        /// </param>
        /// <param name="look">The result lookahead set.</param>
        /// <param name="lookBusy">
        /// A set used for preventing epsilon closures in the ATN
        /// from causing a stack overflow. Outside code should pass
        /// <code>new HashSet&lt;ATNConfig&gt;</code>
        /// for this argument.
        /// </param>
        /// <param name="calledRuleStack">
        /// A set used for preventing left recursion in the
        /// ATN from causing a stack overflow. Outside code should pass
        /// <code>new BitSet()</code>
        /// for this argument.
        /// </param>
        /// <param name="seeThruPreds">
        /// 
        /// <code>true</code>
        /// to true semantic predicates as
        /// implicitly
        /// <code>true</code>
        /// and "see through them", otherwise
        /// <code>false</code>
        /// to treat semantic predicates as opaque and add
        /// <see cref="HitPred">HitPred</see>
        /// to the
        /// result if one is encountered.
        /// </param>
        /// <param name="addEOF">
        /// Add
        /// <see cref="TokenConstants.Eof"/>
        /// to the result if the end of the
        /// outermost context is reached. This parameter has no effect if
        /// <code>ctx</code>
        /// is
        /// <see cref="PredictionContext.EmptyLocal">PredictionContext.EmptyLocal</see>
        /// .
        /// </param>
        protected internal virtual void Look(ATNState s, ATNState stopState, PredictionContext
             ctx, IntervalSet look, HashSet<ATNConfig> lookBusy, BitSet calledRuleStack, 
            bool seeThruPreds, bool addEOF)
        {
            //		System.out.println("_LOOK("+s.stateNumber+", ctx="+ctx);
            ATNConfig c = ATNConfig.Create(s, 0, ctx);
            if (!lookBusy.Add(c))
            {
                return;
            }
            if (s == stopState)
            {
                if (PredictionContext.IsEmptyLocal(ctx))
                {
                    look.Add(TokenConstants.Epsilon);
                    return;
                }
                else
                {
                    if (ctx.IsEmpty && addEOF)
                    {
                        look.Add(TokenConstants.Eof);
                        return;
                    }
                }
            }
            if (s is RuleStopState)
            {
                if (PredictionContext.IsEmptyLocal(ctx))
                {
                    look.Add(TokenConstants.Epsilon);
                    return;
                }
                else
                {
                    if (ctx.IsEmpty && addEOF)
                    {
                        look.Add(TokenConstants.Eof);
                        return;
                    }
                }
                for (int i = 0; i < ctx.Size; i++)
                {
                    if (ctx.GetReturnState(i) != PredictionContext.EmptyFullStateKey)
                    {
                        ATNState returnState = atn.states[ctx.GetReturnState(i)];
                        //					System.out.println("popping back to "+retState);
                        for (int j = 0; j < ctx.Size; j++)
                        {
                            bool removed = calledRuleStack.Get(returnState.ruleIndex);
                            try
                            {
                                calledRuleStack.Clear(returnState.ruleIndex);
                                Look(returnState, stopState, ctx.GetParent(j), look, lookBusy, calledRuleStack, seeThruPreds
                                    , addEOF);
                            }
                            finally
                            {
                                if (removed)
                                {
                                    calledRuleStack.Set(returnState.ruleIndex);
                                }
                            }
                        }
                        return;
                    }
                }
            }
            int n = s.NumberOfTransitions;
            for (int i_1 = 0; i_1 < n; i_1++)
            {
                Transition t = s.Transition(i_1);
                if (t.GetType() == typeof(RuleTransition))
                {
                    if (calledRuleStack.Get(((RuleTransition)t).target.ruleIndex))
                    {
                        continue;
                    }
                    PredictionContext newContext = ctx.GetChild(((RuleTransition)t).followState.stateNumber
                        );
                    try
                    {
                        calledRuleStack.Set(((RuleTransition)t).target.ruleIndex);
                        Look(t.target, stopState, newContext, look, lookBusy, calledRuleStack, seeThruPreds
                            , addEOF);
                    }
                    finally
                    {
                        calledRuleStack.Clear(((RuleTransition)t).target.ruleIndex);
                    }
                }
                else
                {
                    if (t is AbstractPredicateTransition)
                    {
                        if (seeThruPreds)
                        {
                            Look(t.target, stopState, ctx, look, lookBusy, calledRuleStack, seeThruPreds, addEOF
                                );
                        }
                        else
                        {
                            look.Add(HitPred);
                        }
                    }
                    else
                    {
                        if (t.IsEpsilon)
                        {
                            Look(t.target, stopState, ctx, look, lookBusy, calledRuleStack, seeThruPreds, addEOF
                                );
                        }
                        else
                        {
                            if (t.GetType() == typeof(WildcardTransition))
                            {
                                look.AddAll(IntervalSet.Of(TokenConstants.MinUserTokenType, atn.maxTokenType));
                            }
                            else
                            {
                                //				System.out.println("adding "+ t);
                                IntervalSet set = t.Label;
                                if (set != null)
                                {
                                    if (t is NotSetTransition)
                                    {
                                        set = set.Complement(IntervalSet.Of(TokenConstants.MinUserTokenType, atn.maxTokenType
                                            ));
                                    }
                                    look.AddAll(set);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
