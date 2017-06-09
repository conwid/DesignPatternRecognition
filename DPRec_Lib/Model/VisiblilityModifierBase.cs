using System;

namespace DPRec_Lib.Model
{
    [Serializable]
    public sealed class VisibilityModifierBase : IVisibilityModifier
    {
        [Serializable]
        private enum VisiblilityModifierBaseEnum
        {
            Private,
            Protected,
            Public,
            Assembly                                                    
        }

        private VisiblilityModifierBaseEnum visibility;

        private VisibilityModifierBase() { }

        private static readonly Lazy<VisibilityModifierBase> @private = new Lazy<VisibilityModifierBase>(() => new VisibilityModifierBase(VisiblilityModifierBaseEnum.Private));
        private static readonly Lazy<VisibilityModifierBase> @protected = new Lazy<VisibilityModifierBase>(() => new VisibilityModifierBase(VisiblilityModifierBaseEnum.Protected));
        private static readonly Lazy<VisibilityModifierBase> @assembly = new Lazy<VisibilityModifierBase>(() => new VisibilityModifierBase(VisiblilityModifierBaseEnum.Assembly));
        private static readonly Lazy<VisibilityModifierBase> @public = new Lazy<VisibilityModifierBase>(() => new VisibilityModifierBase(VisiblilityModifierBaseEnum.Public));
        private static readonly Lazy<VisibilityModifierBase> @default = new Lazy<VisibilityModifierBase>(() => new VisibilityModifierBase(VisiblilityModifierBaseEnum.Private));

        public static VisibilityModifierBase Private
        {
            get
            {
                return @private.Value;
            }
        }



        public static VisibilityModifierBase Protected
        {
            get
            {
                return @protected.Value;
            }
        }


        public static VisibilityModifierBase Default
        {
            get
            {
                return @default.Value;
            }
        }


        public static VisibilityModifierBase Public
        {
            get
            {
                return @public.Value;
            }
        }

        public static VisibilityModifierBase Internal
        {
            get
            {
                return @assembly.Value;
            }
        }
       

        private VisibilityModifierBase(VisiblilityModifierBaseEnum visibility)
        {
            this.visibility = visibility;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            return this.visibility == ((VisibilityModifierBase)obj).visibility;
        }

        public override int GetHashCode()
        {
            return visibility.GetHashCode();
        }

        public static bool operator ==(VisibilityModifierBase a, VisibilityModifierBase b)
        {
            if (object.ReferenceEquals(a,b))
            {
                return true;
            }
            if ((object)a==null || (object)b==null)
            {
                return false;
            }
            return a.visibility == b.visibility;
        }

        public static bool operator !=(VisibilityModifierBase a, VisibilityModifierBase b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return this.visibility.ToString();
        }
    }
}
