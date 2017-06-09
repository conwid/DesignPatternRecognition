using System;

namespace DPRec_Lib.Model
{
    [Serializable]
    public class GenericTypeParameterBase : IGenericTypeParameter
    {

        private bool issubstituted;
        private int position;
        private INamedType type;
        public bool IsSubstituted
        {
            get
            {
                return this.issubstituted;
            }

            set
            {
                this.issubstituted = value;
            }
        }

        public int Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
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
            if (this.issubstituted)
            {
                return this.type.ToString();
            }
            else
            {
                return string.Format("T{0}", this.position.ToString());
            }
        }

        public override bool Equals(object obj)
        {
            GenericTypeParameterBase other = obj as GenericTypeParameterBase;
            if (other==null)
            {
                return false;
            }
            if (this.position!=other.position)
            {
                return false;
            }
            if (!this.issubstituted && !other.issubstituted)
            {
                return true;
            }
            else if (this.issubstituted && other.issubstituted && (NamedTypeBase)this.type==(NamedTypeBase)other.type)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var a= this.position ^ this.issubstituted.GetHashCode();
            if (this.issubstituted)
            {
                a = a ^ this.type.GetHashCode();
            }
            return a;
        }

        public static bool operator ==(GenericTypeParameterBase p1, GenericTypeParameterBase p2)
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

        public static bool operator !=(GenericTypeParameterBase p1, GenericTypeParameterBase p2)
        {
            return !(p1 == p2);
        }
    }
}
