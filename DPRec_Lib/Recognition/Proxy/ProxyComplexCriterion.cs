using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;
using DPRec_Lib.Helpers;

namespace DPRec_Lib.Recognition.Proxy
{


    public class ProxyComplexCriterion : ICriterion
    {
        private List<NamedTypeBase> GetSiblings(NamedTypeBase type, List<NamedTypeBase> types)
        {
            return types
                .Where(t => t.Bases.Where(o=>o.Name!="Object").Select(x => (NamedTypeBase)x)
                             .Intersect(type.Bases.Where(o=>o.Name!="Object").Select(x2 => (NamedTypeBase)x2), new TypeComparer()).Count() > 1)
                             .Except(new List<NamedTypeBase>() { type }, new TypeComparer()).ToList();
        }

        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            var type = (NamedTypeBase)elementToAnalyze;
            var siblings = GetSiblings(type, context.Types);

            if (siblings.Count<1) { return false; }


            var candidates = siblings.Where(s => type.Fields.Any(f => (NamedTypeBase)f.Type == s)).ToList();

            if (context.CallGraph.Nodes.Any(n => n.Callee != null && n.Caller != null && n.Callee.IsOverride && n.Caller.IsOverride && BaseExtensions.CheckForOverrideCompatiblity(n.Callee, n.Caller)))
            {
                return true;
            }
            return false;
        }
    }
}
