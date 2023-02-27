using System;
using System.Xml.Linq;

namespace XamlConversion.Parsers
{
    class ObjectParser : XamlParser
    {
        public ObjectParser(XamlConvertor.State state)
            : base(state)
        {}

        public string VariableName { get; protected set; }

        public Type Type { get; protected set; }

        [Obsolete]
#pragma warning disable CS0809 // El miembro obsoleto invalida un miembro no obsoleto
        protected override void ParseName(XName name)
#pragma warning restore CS0809 // El miembro obsoleto invalida un miembro no obsoleto
        {
            Type = GetTypeFromXName(name);

            VariableName = CreateObject(Type, name.LocalName);
        }

        [Obsolete]
#pragma warning disable CS0809 // El miembro obsoleto invalida un miembro no obsoleto
        protected override void ParseAttribute(XAttribute attribute)
#pragma warning restore CS0809 // El miembro obsoleto invalida un miembro no obsoleto
        {
            if (attribute.IsNamespaceDeclaration)
                return;

            var propertyName = attribute.Name.LocalName;
            SetProperty(VariableName, Type, propertyName, attribute.Value);
        }

        protected override void ParseElement(XElement element)
        {
            // is it a property?
            if (element.Name.LocalName.Contains("."))
            {
                var propertyParser = new PropertyParser(State, this);
                propertyParser.Parse(element);
            }
            else
            {
                //throw new NotImplementedException();
            }
        }

        protected override void ParseEnd()
        {}
    }
}