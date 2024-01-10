using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerHelpers;
using Internal.Runtime.CompilerServices;

namespace System
{
	public sealed unsafe class String
	{
		internal ref char GetRawStringData() => ref _firstChar;


		[Intrinsic]
		public static readonly string Empty = "";


		// The layout of the string type is a contract with the compiler.
		private int _length;
		internal char _firstChar;


		public int Length
		{
			[Intrinsic]
			get => _length;
			set => _length = value;
		}

		public unsafe char this[int index]
		{
			[Intrinsic]
			get => Unsafe.Add(ref _firstChar, index);

			set
			{
				fixed (char* p = &_firstChar)
				{
					p[index] = value;
				}
			}
		}


#pragma warning disable CS0824 // Constructor is marked external
		public extern unsafe String(char* ptr);
		public extern String(IntPtr ptr);
		public extern String(char[] buf);
		public extern unsafe String(char* ptr, int index, int length);
		public extern unsafe String(char[] buf, int index, int length);
#pragma warning restore CS0824 // Constructor is marked external

        public static unsafe string FromASCII(nint ptr, int length)
        {
            byte* p = (byte*)ptr;
            char* newp = stackalloc char[length];
            for (int i = 0; i < length; i++)
            {
                newp[i] = (char)p[i];
            }
            return new string(newp, 0, length);
        }

        private static unsafe string Ctor(char* ptr)
		{
			int len = (int)ptr->m_pEEType->BaseSize;
            int i = 0;

			while (ptr[i++] != '\0')
			{

			}

			return Ctor(ptr, 0, i - 1);
		}

		private static unsafe string Ctor(IntPtr ptr)
		{
			return Ctor((char*)ptr);
		}

		private static unsafe string Ctor(char[] buf)
		{
			fixed (char* _buf = buf)
			{
				return Ctor(_buf, 0, buf.Length);
			}
		}

		private static unsafe string Ctor(char* ptr, int index, int length)
		{
			EETypePtr et = EETypePtr.EETypePtrOf<string>();

			char* start = ptr + index;
			object data = StartupCodeHelpers.RhpNewArray(et._value, length);
			string s = Unsafe.As<object, string>(ref data);

			fixed (char* c = &s._firstChar)
			{
				memcpy((byte*)c, (byte*)start, (ulong)length * sizeof(char));
				c[length] = '\0';
			}

			return s;
		}

		[DllImport("*")]
		private static extern unsafe void memcpy(byte* dest, byte* src, ulong count);

		public int LastIndexOf(char j)
		{
			for (int i = Length - 1; i >= 0; i--)
			{
				if (this[i] == j)
				{
					return i;
				}
			}

			return -1;
		}

		private static unsafe string Ctor(char[] ptr, int index, int length)
		{
			fixed (char* _ptr = ptr)
			{
				return Ctor(_ptr, index, length);
			}
		}


		public override string ToString()
		{
			return this;
		}

		public override bool Equals(object obj)
		{
			return obj is string && Equals((string)obj);
		}

		public bool Equals(string val)
		{
			if (Length != val.Length)
			{
				return false;
			}

			for (int i = 0; i < Length; i++)
			{
				if (this[i] != val[i])
				{
					return false;
				}
			}

			return true;
		}

		public static bool operator ==(string a, string b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(string a, string b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public static string Concat(string a, string b)
		{
			int Length = a.Length + b.Length;
			char* ptr = stackalloc char[Length];
			int currentIndex = 0;
			for (int i = 0; i < a.Length; i++)
			{
				ptr[currentIndex] = a[i];
				currentIndex++;
			}
			for (int i = 0; i < b.Length; i++)
			{
				ptr[currentIndex] = b[i];
				currentIndex++;
			}
			return new string(ptr, 0, Length);
		}

		public int IndexOf(char j)
		{
			for (int i = 0; i < Length; i++)
			{
				if (this[i] == j)
				{
					return i;
				}
			}

			return -1;
		}

        public int IndexOf(char j, int start)
        {
            for (int i = start; i < Length; i++)
            {
                if (this[i] == j)
                {
                    return i;
                }
            }

            return -1;
        }

        public int IndexOf(string subcadena)
        {
            int _indice = -1;

            for (int i = 0; i <= this.Length - subcadena.Length; i++)
            {
                bool encontrado = true;

                for (int j = 0; j < subcadena.Length; j++)
                {
                    if (this[i + j] != subcadena[j])
                    {
                        encontrado = false;
                        break;
                    }
                }

                if (encontrado)
                {
                    _indice = i;
                    break;
                }
            }

            return _indice;
        }

		public static string charToString(char* charArray)
        {
            string str = "";
            for (int i = 0; charArray[i] != '\0'; i++)
            {
                str += charArray[i];
            }
            return str;
        }


        public static string Copy(string a)
        {
            int Length = a.Length;
            char* ptr = stackalloc char[Length];
            int currentIndex = 0;

            for (int i = 0; i < a.Length; i++)
            {
                ptr[currentIndex] = a[i];
                currentIndex++;
            }
            return new string(ptr, 0, Length);
        }

        public int IndexOf(string subcadena, int indiceInicial)
        {
            int _indice = -1;

            while (indiceInicial < this.Length)
            {
                int i = indiceInicial;
                int j = 0;

                while (i < this.Length && j < subcadena.Length && this[i] == subcadena[j])
                {
                    i++;
                    j++;
                }

                if (j == subcadena.Length)
                {
                    _indice = indiceInicial;
                    break;
                }

                indiceInicial++;
            }

            return _indice;
        }


        public static string Concat(string a, string b, string c)
		{
			string p1 = a + b;
			string p2 = p1 + c;
			p1.Dispose();
			return p2;
		}

		public static string Concat(string a, string b, string c, string d)
		{
			string p1 = a + b;
			string p2 = p1 + c;
			string p3 = p2 + d;
			p1.Dispose();
			p2.Dispose();
			return p3;
		}

		public static string Concat(params string[] vs)
		{
			string s = "";
			for (int i = 0; i < vs.Length; i++)
			{
				string tmp = s + vs[i];
				s.Dispose();
				s = tmp;
			}
			vs.Dispose();
			return s;
		}
		public static string Format(string format, params object[] args)
		{
			lock (format)
			{
				string res = Empty;
				for (int i = 0; i < format.Length; i++)
				{
					string chr;
					if ((i + 2) < format.Length && format[i] == '{' && format[i + 2] == '}')
					{
						chr = args[format[i + 1] - 0x30].ToString();
						i += 2;
					} else
					{
						chr = format[i].ToString();
					}
					string str = res + chr;
					chr.Dispose();
					res.Dispose();
					res = str;
				}

				for (int i = 0; i < args.Length; i++)
				{
					args[i].Dispose();
				}

				args.Dispose();
				return res;
			}
		}

		public string Remove(int startIndex)
		{
			return Substring(0, startIndex);
		}

		public string[] Split(char chr)
		{
			List<string> strings = new();
			string tmp = string.Empty;
			for (int i = 0; i < Length; i++)
			{
				if (this[i] == chr)
				{
					strings.Add(tmp);
					tmp = string.Empty;
				} else
				{
					tmp += this[i];
				}

				if (i == (Length - 1))
				{
					strings.Add(tmp);
					tmp = string.Empty;
				}
			}
			return strings.ToArray();
		}

		public unsafe string Substring(int startIndex)
		{
			if ((Length == 0) && (startIndex == 0))
			{
				return Empty;
			}
			// Usually one uses the extension method with non-null values
			// so all we need to worry about is startIndex compared to value.Length.
			/*
			if ((startIndex < 0) || (startIndex >= Length))
			{
				ThrowHelpers.ThrowArgumentOutOfRangeException("startIndex");
			}
			*/

			/*
			string substring = "";
			for (int i = startIndex; i < Length; i++)
			{
				substring += this[i];
			}
			return substring;
			*/
			fixed (char* ptr = this)
			{
				return new string(ptr, startIndex, Length - startIndex);
			}
		}
		public unsafe string Substring(int startIndex, int endIndex)
		{
			if ((Length == 0) && (startIndex == 0))
			{
				return Empty;
			}
			// Usually one uses the extension method with non-null values
			// so all we need to worry about is startIndex compared to value.Length.
			/*
			if ((startIndex < 0) || (startIndex >= Length) || (endIndex >= Length) || (endIndex <= startIndex))
			{
				ThrowHelpers.ThrowArgumentOutOfRangeException("startIndex");
			}
			*/

			/*
			string substring = "";
			for (int i = startIndex; i < endIndex; i++)
			{
				substring += this[i];
			}
			return substring;
			*/
			fixed(char* ptr = this)
            {
				return new string(ptr, startIndex, endIndex - startIndex);
            }
		}
#nullable enable
		public static bool IsNullOrEmpty(string? value)
#nullable disable
		{
			return value == null || value.Length == 0;
		}
#nullable enable
		public static bool IsNullOrWhiteSpace(string? value)
#nullable disable
		{
			if (value == null)
			{
				value.Dispose();
				return true;
			}

			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					value.Dispose();
					return false;
				}
			}
			value.Dispose();
			return true;
		}

		public bool EndsWith(char value)
		{
			int thisLen = Length;
			if (thisLen != 0)
			{
				if (this[thisLen - 1] == value)
				{
					thisLen.Dispose();
					return true;
				}
			}
			thisLen.Dispose();
			return false;
		}

		public bool EndsWith(string value)
		{
			if (value.Length > Length)
			{
				return false;
			}

			if (value == this)
			{
				return true;
			}

			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] != this[Length - value.Length + i])
				{
					return false;
				}
			}
			return true;
		}

		public string ToUpper()
		{
			fixed (char* pthis = this)
			{
				string output = new string(pthis, 0, this.Length);
				for (int i = 0; i < this.Length; i++)
				{
					output[i] = pthis[i].ToUpper();
				}
				return output;
			}
		}

		public string ToLower()
		{
			fixed(char* pthis = this)
            {
				string output = new string(pthis, 0, this.Length);
				for(int i = 0; i < this.Length; i++)
                {
					output[i] = pthis[i].ToLower();
                }
				return output;
			}
		}

		public static int strlen(byte* c)
		{
			int i = 0;
			while (c[i] != 0) i++;
			return i;
		}

		public static int strlen(char* c)
		{
			int i = 0;
			while (c[i] != 0) i++;
			return i;
		}

        public string PadLeft(int num, char chr)
        {
            string result = "";

            for (int i = 0; i < (num - this.Length); i++)
            {
                result += chr;
            }

            return result + this;
        }

        public string PadLeft(string str, int num)
        {
            string result = "";

            for (int i = 0; i < num; i++)
            {
                result += this[i];
            }

            result += str;

            return result;
        }

        public string Trim()
        {
            string result = "";

            for (int i = 0; i < this.Length; i++)
            {
                if (!char.IsWhiteSpace(this[i]))
				{
					result += this[i];
				}
            }

            return result;
        }

        public string Trim(char c)
        {
            string result = "";

            for (int i = 0; i < this.Length; i++)
            {
                if (!char.IsWhiteSpace(this[i]) && this[i] != c)
                {
                    result += this[i];
                }
            }

            return result;

        }
        public string Replace(string a, string b)
		{
            string result = "";

            for (int i = 0; i < this.Length; i++)
            {
				if (this[i] == a[0])
				{
					this[i] = b[0];
                }
                if (!char.IsWhiteSpace(this[i]))
                {
                    result += this[i];
                }
            }

            return result;
        }
    }
}
