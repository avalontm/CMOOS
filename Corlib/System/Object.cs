using Internal.Runtime;
using Internal.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System
{
    public unsafe class Object
    {
        EEType* m_pEEType;
        internal EEType* EEType
        {
            get
            {
                // NOTE:  if managed code can be run when the GC has objects marked, then this method is 
                //        unsafe.  But, generically, we don't expect managed code such as this to be allowed
                //        to run while the GC is running.
                return m_pEEType;
            }
        }

        internal EETypePtr EETypePtr
        {
            get
            {
                return new EETypePtr(new IntPtr(m_pEEType));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private class RawData
        {
            public byte Data;
        }

        internal ref byte GetRawData()
        {
            return ref Unsafe.As<RawData>(this).Data;
        }

        internal uint GetRawDataSize()
        {
            return m_pEEType->BaseSize - (uint)sizeof(ObjHeader) - (uint)sizeof(EEType*);
        }

        // Creates a new instance of an Object.
        public Object() { }

        // Allow an object to free resources before the object is reclaimed by the GC.
        // This method's virtual slot number is hardcoded in runtimes. Do not add any virtual methods ahead of this.
        ~Object() { }
        
        public unsafe virtual bool Equals(object b)
        {
            object a = this;

            if (a== null || b == null)
            {
                Debug.WriteLine($"[Equals] NULL");
                return false;
            }

            switch (a.m_pEEType->ElementType)
            {
                case EETypeElementType.Byte:
                    return ((Byte)a == (Byte)b);
                case EETypeElementType.SByte:
                    return ((SByte)a == (SByte)b);
                case EETypeElementType.Int16:
                    return ((Int16)a == (Int16)b);
                case EETypeElementType.UInt16:
                    return ((UInt16)a == (UInt16)b);
                case EETypeElementType.Int32:
                    return ((Int32)a == (Int32)b);
                case EETypeElementType.UInt32:
                    return ((UInt32)a == (UInt32)b);
                case EETypeElementType.Int64:
                    return ((Int64)a == (Int64)b);
                case EETypeElementType.UInt64:
                    return ((UInt64)a == (UInt64)b);
                case EETypeElementType.IntPtr:
                    return ((IntPtr)a == (IntPtr)b);
                case EETypeElementType.UIntPtr:
                    return ((UIntPtr)a == (UIntPtr)b);
                case EETypeElementType.Char:
                    return ((Char)a == (Char)b);
                case EETypeElementType.Class:
                      return (a == b);
                    
            }

            return false;
        }
        
        
        public unsafe static bool ReferenceEquals(object a, object b)
        {
            return a.Equals(b);
        }
        
        /*
        public unsafe static bool operator ==(object a, object b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(object a, object b)
        {
            return !a.Equals(b);
        }
        */

        public virtual int GetHashCode()
        {
            return (int)this.EEType->HashCode;
        }

        public virtual string ToString()
        {
            return $"Type: {(int)EEType->ElementType}";// GetType().ToString();
        }


        public virtual void Dispose()
        {
            var obj = this;
            free(Unsafe.As<object, IntPtr>(ref obj));
        }

        public static implicit operator bool(object obj)=> obj != null;
        
        public static T FromHandle<T>(IntPtr handle) where T : class
        {
            return Unsafe.As<IntPtr, T>(ref handle);
        }

        public IntPtr GetHandle()
        {
            object _this = this;
            return Unsafe.As<object, IntPtr>(ref _this);
        }


        [DllImport("*")]
        static extern ulong free(nint ptr);

    }
}
