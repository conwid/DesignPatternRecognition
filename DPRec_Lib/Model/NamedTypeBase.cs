using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using DPRec_Lib.CSharpImplementation;

namespace DPRec_Lib.Model
{
    [Serializable]
    public class NamedTypeBase : INamedType
    {

        private List<INamedType> bases = new List<INamedType>();
        private bool isabstract;
        private bool istatic;
        private List<IMethod> methods = new List<IMethod>();
        private string name;
        private IVisibilityModifier visibility;
        private List<IField> fields = new List<IField>();

        private string @namespace;

        private bool isArray;

        private bool isgeneric;
        private List<IGenericTypeParameter> genericparameters = new List<IGenericTypeParameter>();     
               

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.@namespace);
            sb.Append(".");
            sb.Append(this.name);
            if (this.isgeneric)
            {                
                sb.Append("<");
                sb.Append(string.Join(",", this.GenericParameters.Select(gp => gp.ToString())));
                sb.Append(">");                
            }
            if (this.isArray)
            {
                sb.Append("[]");
            }
            return sb.ToString();
        }       

        public bool IsAbstract
        {
            get
            {
                return this.isabstract;
            }

            set
            {
                this.isabstract = value;
            }
        }

        public bool IsStatic
        {
            get
            {
                return this.istatic;
            }

            set
            {
                this.istatic = value;
            }
        }



        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public IVisibilityModifier Visibility
        {
            get
            {
                return this.visibility;
            }

            set
            {
                this.visibility = (VisibilityModifierBase)value;
            }
        }

        public List<INamedType> Bases
        {
            get
            {
                return this.bases;
            }

            set
            {
                this.bases = value;
            }
        }

        public List<IField> Fields
        {
            get
            {
                return this.fields;
            }

            set
            {
                this.fields = value;
            }
        }

        public List<IMethod> Methods
        {
            get
            {
                return this.methods;
            }

            set
            {
                this.methods = value;
            }
        }

        public List<IGenericTypeParameter> GenericParameters
        {
            get
            {
                return this.genericparameters;
            }

            set
            {
                this.genericparameters = value;
            }
        }

        public bool IsGeneric
        {
            get
            {
                return this.isgeneric;
            }

            set
            {
                this.isgeneric = value;
            }
        }
       

        public bool IsArray
        {
            get
            {
                return this.isArray;
            }

            set
            {
                this.isArray = value;
            }
        }

        public string Namespace
        {
            get
            {
                return this.@namespace;
            }

            set
            {
                this.@namespace = value;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as NamedTypeBase;            
            if (obj == null)
            {
                return false;
            }           
            if (this.name != other.name)
            {
                return false;
            }
            if (this.@namespace != other.@namespace)
            {
                return false;
            }
            if (this.isArray!=other.isArray)
            {
                return false;
            }
            if (this.isgeneric != other.isgeneric)
            {
                return false;
            }
            if (this.isgeneric)
            {
                if (this.GenericParameters.Count != other.GenericParameters.Count)
                {
                    return false;
                }
                for (int i = 0; i < this.genericparameters.Count; i++)
                {
                    if ((GenericTypeParameterBase)this.genericparameters[i] != (GenericTypeParameterBase)other.genericparameters[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            var hc = this.Name.GetHashCode() ^ this.IsGeneric.GetHashCode() ^ this.isArray.GetHashCode()^this.@namespace.GetHashCode();
            foreach (var item in this.GenericParameters)
            {
                hc = hc ^ item.GetHashCode();
            }
            return hc;
        }

        public static bool operator ==(NamedTypeBase p1, NamedTypeBase p2)
        {
            if (object.ReferenceEquals(p1, p2))
            {
                return true;
            }
            if ((object)p1 == null || (object)p2 == null)
            {
                return false;
            }
            return p1.Equals(p2);
        }

        public static bool operator !=(NamedTypeBase p1, NamedTypeBase p2)
        {
            return !(p1 == p2);
        }
    }
}
