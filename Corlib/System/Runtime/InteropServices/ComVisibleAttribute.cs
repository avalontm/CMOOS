namespace System.Runtime.InteropServices
{
    internal class ComVisibleAttribute : Attribute
    {
        private bool m_visible;

        public bool Visible
        {
            get { return m_visible; }
        }

        public ComVisibleAttribute(bool visible)
        { 
            m_visible = visible;
        }
    }
}