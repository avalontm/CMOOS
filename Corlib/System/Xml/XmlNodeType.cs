namespace System.Xml
{
    //
    // Resumen:
    //     Specifies the type of node.
    public enum XmlNodeType
    {
        //
        // Resumen:
        //     This is returned by the System.Xml.XmlReader if a Read method has not been called.
        None = 0,
        //
        // Resumen:
        //     An element (for example, <item> ).
        Element = 1,
        //
        // Resumen:
        //     An attribute (for example, id='123' ).
        Attribute = 2,
        //
        // Resumen:
        //     The text content of a node.
        //     A System.Xml.XmlNodeType.Text node cannot have any child nodes. It can appear
        //     as the child node of the System.Xml.XmlNodeType.Attribute, System.Xml.XmlNodeType.DocumentFragment,
        //     System.Xml.XmlNodeType.Element, and System.Xml.XmlNodeType.EntityReference nodes.
        Text = 3,
        //
        // Resumen:
        //     A CDATA section (for example, <![CDATA[my escaped text]]> ).
        CDATA = 4,
        //
        // Resumen:
        //     A reference to an entity (for example, &num; ).
        EntityReference = 5,
        //
        // Resumen:
        //     An entity declaration (for example, <!ENTITY...> ).
        Entity = 6,
        //
        // Resumen:
        //     A processing instruction (for example, <?pi test?> ).
        ProcessingInstruction = 7,
        //
        // Resumen:
        //     A comment (for example, <!-- my comment --> ).
        Comment = 8,
        //
        // Resumen:
        //     A document object that, as the root of the document tree, provides access to
        //     the entire XML document.
        Document = 9,
        //
        // Resumen:
        //     The document type declaration, indicated by the following tag (for example, <!DOCTYPE...>
        //     ).
        DocumentType = 10,
        //
        // Resumen:
        //     A document fragment.
        DocumentFragment = 11,
        //
        // Resumen:
        //     A notation in the document type declaration (for example, <!NOTATION...> ).
        Notation = 12,
        //
        // Resumen:
        //     White space between markup.
        Whitespace = 13,
        //
        // Resumen:
        //     White space between markup in a mixed content model or white space within the
        //     xml:space="preserve" scope.
        SignificantWhitespace = 14,
        //
        // Resumen:
        //     An end element tag (for example, </item> ).
        EndElement = 15,
        //
        // Resumen:
        //     Returned when XmlReader gets to the end of the entity replacement as a result
        //     of a call to System.Xml.XmlReader.ResolveEntity.
        EndEntity = 16,
        //
        // Resumen:
        //     The XML declaration (for example, <?xml version='1.0'?> ).
        //     The System.Xml.XmlNodeType.XmlDeclaration node must be the first node in the
        //     document. It cannot have children. It is a child of the System.Xml.XmlNodeType.Document
        //     node. It can have attributes that provide version and encoding information.
        XmlDeclaration = 17
    }
}
