namespace System
{
    public abstract class Exception
    {
        private string _exceptionString;

        public Exception() { }

        public Exception(String str)
        {
            _exceptionString = str;
        }
    }
    internal sealed class InternalException : Exception
    {
        public InternalException(string str) :base(str)
        {
        
        }
    }

    internal sealed class NullReferenceException : Exception
    {
        public NullReferenceException() { }
    }

    internal sealed class InvalidOperationException : Exception
    {
        public InvalidOperationException() { }
    }

    internal sealed class ArgumentOutOfRangeException : Exception
    {
        public ArgumentOutOfRangeException() { }
    }

    internal sealed class IndexOutOfRangeException : Exception
    {
        public IndexOutOfRangeException() { }
    }

    internal sealed class ArgumentNullException : Exception
    {
        public ArgumentNullException() { }
    }

    internal sealed class NotImplementedException : Exception
    {
        public NotImplementedException() { }
    }

    internal sealed class NotSupportedException : Exception
    {
        public NotSupportedException() { }
    }

    internal sealed class PlatformNotSupportedException : Exception
    {
        public PlatformNotSupportedException() { }
    }

    internal sealed class InvalidCastException : Exception
    {
        public InvalidCastException() { }
    }

    internal sealed class ArrayTypeMismatchException : Exception
    {
        public ArrayTypeMismatchException() { }
    }

    internal sealed class OverflowException : Exception
    {
        public OverflowException() { }
    }

    internal sealed class ArithmeticException : Exception
    {
        public ArithmeticException() { }
    }

    internal sealed class DivideByZeroException : Exception
    {
        public DivideByZeroException() { }
    }

    internal class OutOfMemoryException : Exception
    {
        public OutOfMemoryException() { }
    }
}
