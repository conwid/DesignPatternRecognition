using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{
    [Serializable]
    public class MethodParameterBase : IMethodParameter
    {
        private bool isTypeParameter;
        private INamedType type;
        private string name;
        private int ordinal;

        public override bool Equals(object obj)
        {
            MethodParameterBase other = obj as MethodParameterBase;
            if (other ==null)
            {
                return false;
            }

            if (this.ordinal!=other.ordinal)
            {
                return false;
            }

            if (!this.isTypeParameter==other.isTypeParameter)
            {
                return false;
            }

            if (this.isTypeParameter)
            {
                return true;
            }

            return ((NamedTypeBase)this.type == (NamedTypeBase)other.type);
        }

        public override int GetHashCode()
        {
            var a = this.ordinal ^ this.isTypeParameter.GetHashCode();
            if (this.isTypeParameter)
            {
                return a;
            }
            return a ^ this.type.GetHashCode();
        }

        public static bool operator ==(MethodParameterBase mp1, MethodParameterBase mp2)
        {
            if (object.ReferenceEquals(mp1,mp2))
            {
                return true;
            }
            if ((object)mp1==null || (object)mp2==null)
            {
                return false;
            }
            return mp1.Equals(mp2);
        }

        public static bool operator !=(MethodParameterBase mp1, MethodParameterBase mp2)
        {
            return !(mp1 == mp2);
        }

        public bool IsTypeParameter
        {
            get
            {
                return this.isTypeParameter;
            }

            set
            {
                this.isTypeParameter = value;
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

        public int Ordinal
        {
            get
            {
                return this.ordinal;
            }

            set
            {
                this.ordinal = value;
            }
        }

        public INamedType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.name, this.Type.ToString());
        }
    }
}
