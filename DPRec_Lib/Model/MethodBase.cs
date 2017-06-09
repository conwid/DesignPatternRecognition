using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Runtime.Serialization;

namespace DPRec_Lib.Model
{
    [Serializable]
    public class MethodBase : IMethod
    {

        private bool isabstract;
        private bool isoverride;
        private bool isstatic;
        private bool isctor;
        private bool isvirtual;

        private List<INamedType> actulaReturnTypes = new List<INamedType>();
        private INamedType declaredReturnType;
        private string name;

        private INamedType parent;


        private bool isgeneric;

        private List<IGenericTypeParameter> genericparameters = new List<IGenericTypeParameter>();


        private IVisibilityModifier visibility;

        private List<IMethodParameter> parameters = new List<IMethodParameter>();


        
        public List<INamedType> ActualReturnTypes
        {
            get
            {
                return actulaReturnTypes;
            }

            set
            {
                this.actulaReturnTypes = value;
            }
        }
        
        public INamedType DeclaredReturnType
        {
            get
            {
                return this.declaredReturnType;
            }

            set
            {
                this.declaredReturnType = value;
            }
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

        public bool IsCtor
        {
            get
            {
                return this.isctor;
            }

            set
            {
                this.isctor = value;
            }
        }

        public bool IsOverride
        {
            get
            {
                return this.isoverride;
            }

            set
            {
                this.isoverride = value;
            }
        }

        public bool IsStatic
        {
            get
            {
                return this.isstatic;
            }

            set
            {
                this.isstatic = value;
            }
        }

        public bool IsVirtual
        {
            get
            {
                return this.isvirtual;
            }

            set
            {
                this.isvirtual = value;
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

        public List<IMethodParameter> Parameters
        {
            get
            {
                return this.parameters;
            }

            set
            {
                this.parameters = value;
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
                this.visibility = value;
            }
        }

        public INamedType Parent
        {
            get
            {
                return parent;
            }

            set
            {
                this.parent = value;
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.Parent.ToString());
            sb.Append(".");
            sb.Append(this.Name);
            if (this.isgeneric)
            {
                sb.Append("<");
                sb.Append(string.Join(",", this.GenericParameters.Select(gp => gp.ToString())));
                sb.Append(">");
            }
            return sb.ToString();
        }


        public override bool Equals(object obj)
        {
            MethodBase other = obj as MethodBase;
            if (obj==null)
            {
                return false;
            }
            if ((NamedTypeBase)this.Parent!=(NamedTypeBase)other.Parent)
            {
                return false;
            }
            if (this.name!=other.name)
            {
                return false;
            }
            if (this.isgeneric!=other.isgeneric)
            {
                return false;
            }            
            if (this.Parameters.Count != other.Parameters.Count)
            {
                return false;
            }
            if (this.Parameters.Except(other.Parameters).Count() != 0)
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
                    if (this.genericparameters[i] != other.genericparameters[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            var a= this.name.GetHashCode() ^ this.Parent.GetHashCode()  ^ this.isgeneric.GetHashCode();
            foreach (var item in this.parameters)
            {
                a = a ^ item.GetHashCode();
            }
            if (this.isgeneric)
            {
                foreach (var item in this.genericparameters)
                {
                    a = a ^ item.GetHashCode();
                }
            }
            return a;
        }

        public static bool operator ==(MethodBase p1, MethodBase p2)
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

        public static bool operator !=(MethodBase p1, MethodBase p2)
        {
            return !(p1 == p2);
        }
    }
}
