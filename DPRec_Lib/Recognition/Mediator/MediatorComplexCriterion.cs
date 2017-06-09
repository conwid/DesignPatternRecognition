using System;
using DPRec_Lib.Model;
using DPRec_Lib.Recognition.Model;
using System.Linq;
using DPRec_Lib.Helpers;

namespace DPRec_Lib.Recognition.Mediator
{
    public class MediatorComplexCriterion : ICriterion
    {
        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            var type = (NamedTypeBase)elementToAnalyze;
            var descendants = context.Types.Where(t => t.Bases.Any(b => (NamedTypeBase)b == type)).ToList();
            foreach (var d in descendants)
            {
                var fieldTypes = d.Fields.Select(f => f.Type).Cast<NamedTypeBase>().Distinct(new TypeComparer()).ToList();
                for (int i = 0; i < fieldTypes.Count; i++)
                {
                    if (fieldTypes[i] == null)
                    {
                        continue;
                    }
                    for (int j = i + 1; j < fieldTypes.Count; j++)
                    {
                        if (fieldTypes[j] == null)
                        {
                            continue;
                        }
                        if (fieldTypes[i].IsArray || fieldTypes[j].IsArray)
                        { continue; }
                        var ancestor = AreSiblings(fieldTypes[i], fieldTypes[j]);
                        if (ancestor != null)
                        {
                            if (ancestor.Fields.Any(f => (NamedTypeBase)f.Type == type))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private NamedTypeBase AreSiblings(NamedTypeBase namedTypeBase1, NamedTypeBase namedTypeBase2)
        {
            foreach (var baseType in namedTypeBase1.Bases)
            {
                var ancestor = namedTypeBase2.Bases.FirstOrDefault(b => (NamedTypeBase)b == (NamedTypeBase)baseType);
                if (ancestor != null && (ancestor.Name != "Object" || ancestor.Name != "object"))
                {
                    return (NamedTypeBase)ancestor;
                }
            }
            return null;
        }
    }
}