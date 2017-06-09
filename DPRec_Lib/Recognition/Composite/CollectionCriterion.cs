using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;

namespace DPRec_Lib.Recognition.Composite
{
    public class CollectionCriterion : ICriterion
    {
        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            var type = (NamedTypeBase)elementToAnalyze;
            var descendants = context.Types.Where(t => t.Bases.Any(b => (NamedTypeBase)b == type)).ToList();
            if (descendants.Count < 2)
            {
                return false;
            }

            var compositedescendants1 = descendants.Where(
                //Bármely field típusa a nem generikus IEnumerable vagy a generikus IEnumerable és a típusparaméter az ős
                //megengedjük-e a nem generikus IEnumerable-t is? (arraylist)
                t => t.Fields.Any(f => (f.Type.Name.Contains("IEnumarable") && (!f.Type.IsGeneric || (f.Type.IsGeneric && f.Type.GenericParameters.Any(gp => gp.IsSubstituted && (NamedTypeBase)gp.Type == type)))
                                        || 
                                 //vagy bármely ősre igaz ugyanez
                                        f.Type.Bases.Any(fb => fb.Name.Contains("IEnumerable") && (!fb.IsGeneric || (fb.IsGeneric && fb.GenericParameters.Any(gp=>gp.IsSubstituted && (NamedTypeBase)gp.Type==type))) 
                                        )))).ToList();
            var compositedescendants2 = descendants.Where(t => t.Fields.Any(f => f.Type.IsArray && BaseExtensions.CompareForArrayTypeCompatibility((NamedTypeBase)f.Type, type))).ToList();
            var compositedescendants = compositedescendants1.Union(compositedescendants2);
            var leafdescendants = descendants.Except(compositedescendants);
            if (compositedescendants.Count() < 1 || leafdescendants.Count() < 1)
            {
                return false;
            }



            if (!context.CallGraph.Nodes.Any(n => n.Callee != null && n.Caller != null && (NamedTypeBase)n.Callee.Parent == type && compositedescendants.Any(t => t == (NamedTypeBase)n.Caller.Parent) && n.Caller.IsOverride && BaseExtensions.CheckForOverrideCompatiblity(n.Caller, n.Callee)))
            {
                return false;
            }

            return true;

        }
    }
}
