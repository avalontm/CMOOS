using Moos.Core.System.Runtime.InteropServices;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
    public unsafe abstract class Type : _Type
    {
        public string Name { get; set; }

        public Type DeclaringType { get; set; }

        public Type ReflectedType { get; set; }

        public Assembly Assembly { get; set; }

        public RuntimeTypeHandle TypeHandle { get; set; }

        public string FullName { get; set; }

        public string Namespace { get; set; }

        public string AssemblyQualifiedName { get; set; }

        public Type BaseType { get; set; }

        public Type UnderlyingSystemType { get; set; }

        public bool IsNotPublic => throw new NotImplementedException();

        public bool IsPublic => throw new NotImplementedException();

        public bool IsNestedPublic => throw new NotImplementedException();

        public bool IsNestedPrivate => throw new NotImplementedException();

        public bool IsNestedFamily => throw new NotImplementedException();

        public bool IsNestedAssembly => throw new NotImplementedException();

        public bool IsNestedFamANDAssem => throw new NotImplementedException();

        public bool IsNestedFamORAssem => throw new NotImplementedException();

        public bool IsAutoLayout => throw new NotImplementedException();

        public bool IsLayoutSequential => throw new NotImplementedException();

        public bool IsExplicitLayout => throw new NotImplementedException();

        public bool IsClass => throw new NotImplementedException();

        public bool IsInterface => throw new NotImplementedException();

        public bool IsValueType => throw new NotImplementedException();

        public bool IsAbstract => throw new NotImplementedException();

        public bool IsSealed => throw new NotImplementedException();

        public bool IsEnum => throw new NotImplementedException();

        public bool IsSpecialName => throw new NotImplementedException();

        public bool IsImport => throw new NotImplementedException();

        public bool IsSerializable => throw new NotImplementedException();

        public bool IsAnsiClass => throw new NotImplementedException();

        public bool IsUnicodeClass => throw new NotImplementedException();

        public bool IsAutoClass => throw new NotImplementedException();

        public bool IsArray => throw new NotImplementedException();

        public bool IsByRef => throw new NotImplementedException();

        public bool IsPointer => throw new NotImplementedException();

        public bool IsPrimitive => throw new NotImplementedException();

        public bool IsCOMObject => throw new NotImplementedException();

        public bool HasElementType => throw new NotImplementedException();

        public bool IsContextful => throw new NotImplementedException();

        public bool IsMarshalByRef => throw new NotImplementedException();

        public override string ToString()
        {
            return $"Type: {Name}";
        }
    }
}
