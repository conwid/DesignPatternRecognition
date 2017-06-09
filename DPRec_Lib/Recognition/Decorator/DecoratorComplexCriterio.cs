using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;
using DPRec_Lib.Helpers;

namespace DPRec_Lib.Recognition.Decorator
{
    public class DecoratorComplexCriterion : ICriterion
    {

        private List<NamedTypeBase> GetSiblings(NamedTypeBase type, List<NamedTypeBase> types)
        {
            return types
                .Where(t => t.Bases.Where(o => o.Name != "Object").Select(x => (NamedTypeBase)x)
                             .Intersect(type.Bases.Where(o => o.Name != "Object").Select(x2 => (NamedTypeBase)x2), new TypeComparer()).Count() > 1)
                             .Except(new List<NamedTypeBase>() { type }, new TypeComparer()).ToList();
        }

        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            var type = (NamedTypeBase)elementToAnalyze;
            var siblings = GetSiblings(type, context.Types);
            return type.Methods.Count(m => m.IsCtor && m.Parameters.Any(p => p.Type != null && siblings.Any(s=>s==(NamedTypeBase)p.Type))) > 0;
        }
    }
}
