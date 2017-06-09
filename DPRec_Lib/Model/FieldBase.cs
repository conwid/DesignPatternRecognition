using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DPRec_Lib.Model
{
    [Serializable]
    public class FieldBase : IField
    {

        private string name;
        private INamedType type;
        private IVisibilityModifier visiblity;
        private bool isstatic;
        private INamedType parent;
        private bool isinit;
        private bool isinitoncreation;
        private List<IMethod> directlyreturnningmethods;

        private bool autoimplemented;

        public bool IsStatic
        {
            get
            {
                return isstatic;
            }

            set
            {
                isstatic = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                this.name = value;
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

        public IVisibilityModifier Visibility
        {
            get
            {
                return this.visiblity;
            }

            set
            {
                this.visiblity = value;
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

        public bool IsInitializedOnce
        {
            get
            {
                return this.isinit;
            }

            set
            {
                this.isinit = value;
            }
        }

        public bool IsInitializedOnCreation
        {
            get
            {
                return this.isinitoncreation;
            }

            set
            {
                this.isinitoncreation = value;
            }
        }

        public List<IMethod> DirectlyReturningMethods
        {
            get
            {
                return this.directlyreturnningmethods;
            }

            set
            {
                this.directlyreturnningmethods = value;
            }
        }

        public bool AutoImplemented
        {
            get
            {
                return this.autoimplemented;
            }

            set
            {
                this.autoimplemented = value;
            }
        }

        public override string ToString()
        {
            return string.Concat(this.Parent.ToString() + "." + this.Name);
        }


        public override bool Equals(object obj)
        {
            FieldBase other = obj as FieldBase;
            if (obj==null)
            {
                return false;
            }
            if (this.name!=other.name)
            {
                return false;
            }
            if ((NamedTypeBase)this.Parent!=(NamedTypeBase)other.parent)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode() ^ this.parent.GetHashCode();
        }

        public static bool operator ==(FieldBase p1, FieldBase p2)
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

        public static bool operator !=(FieldBase p1, FieldBase p2)
        {
            return !(p1 == p2);
        }
    }
}
