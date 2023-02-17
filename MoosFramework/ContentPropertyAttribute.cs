using System;

namespace Moos.Framework
{
    internal class ContentPropertyAttribute : Attribute
    {
        public string Content { set; get; }

        public ContentPropertyAttribute(string content)
        {
            Content = content;
        }
    }
}