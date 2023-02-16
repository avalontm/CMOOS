using System;

namespace MoosFramework
{
    internal class ContentPropertyAttribute : Attribute
    {
        public string Content { set; get; }

        public ContentPropertyAttribute(string content)
        {
            this.Content = content;
        }
    }
}