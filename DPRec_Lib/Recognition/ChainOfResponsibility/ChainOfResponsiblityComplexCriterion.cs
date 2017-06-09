using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;

namespace DPRec_Lib.Recognition.ChainOfResponsibility
{
    public class ChainOfResponsiblityComplexCriterion : ICriterion
    {
        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {            
            var candidates = new List<NamedTypeBase>();
            var type = (NamedTypeBase)elementToAnalyze;
            {
                if (context.Types.Count(t => t.Bases != null && t.Bases.Any(b => (NamedTypeBase)b == type) && t.Methods != null && t.Methods.Count > 0) > 1)
                {
                    if ((type.Fields != null && type.Fields.Any(f => (NamedTypeBase)f.Type == type)))
                    {
                        candidates.Add(type);
                    }
                }
            }

            foreach (var candidate in candidates)
            {
                var list = context.Types.Where(t => t.Bases != null && t.Bases.Any(b => (NamedTypeBase)b == candidate) && t.Methods != null && t.Methods.Count > 0).ToList();
                list.Add(candidate);
                var allmethods = list.SelectMany(l => l.Methods);
                if (allmethods.Count() < 1) { return false; }
                var candidatemethods = allmethods.Where(m => !m.IsStatic && (VisibilityModifierBase)m.Visibility == VisibilityModifierBase.Public && list.All(l => l.Methods.Any(m2 => m2.Name == m.Name && m2.IsGeneric == m.IsGeneric))).Where(m => (NamedTypeBase)m.Parent == candidate).ToList();
                if (candidatemethods.Count() < 1)
                {
                    return false;
                }

                foreach (var candidateMethod in candidatemethods.Cast<MethodBase>())
                {
                    var candidateNodes = context.CallGraph.Nodes.Where(n => n.Caller != null && n.Callee != null && n.Callee.Name == candidateMethod.Name && n.Caller.Name == candidateMethod.Name && n.Caller.Parent != n.Callee.Parent && (NamedTypeBase)n.Callee.Parent == candidate);
                    var sameSourceCount = candidateNodes.GroupBy(n => n.Caller.Parent.Name).Count();

                    if (sameSourceCount != candidateNodes.Count())
                    {
                        continue;
                    }
                    if (candidateNodes.Count() == list.Count - 2)
                    {
                        return true;
                    }
                }

            }
            return false;
        }
    }
}
