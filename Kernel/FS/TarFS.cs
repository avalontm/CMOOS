using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Helpers;
using System.Runtime.InteropServices;
using System.Text;

namespace MOOS.FS
{
    internal unsafe class TarFS : FileRamSystem
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct posix_tar_header
        {
            public fixed byte name[100];
            public fixed byte mode[8];
            public fixed byte uid[8];
            public fixed byte gid[8];
            public fixed byte size[12];
            public fixed byte mtime[12];
            public fixed byte chksum[8];
            public byte typeflag;
            public fixed byte linkname[100];
            public fixed byte magic[6];
            public fixed byte version[2];
            public fixed byte uname[32];
            public fixed byte gname[32];
            public fixed byte devmajor[8];
            public fixed byte devminor[8];
            public fixed byte prefix[155];
            public fixed byte padding[12]; //Nothing of interest, but relevant for checksum
        };

        public class TarHeader
        {
            public string name { set; get; }
            public string mode { set; get; } 
            public int uid { set; get; }
            public int gid { set; get; } 
            public ulong size { set; get; } 
            public int mtime { set; get; }
            public int chksum { set; get; } 
            public char typeflag { set; get; }
            public int linkname { set; get; } 
            public int magic { set; get; } 
            public int version { set; get; } 
            public int uname { set; get; } 
            public int gname { set; get; } 
            public int devmajor { set; get; } 
            public int devminor { set; get; } 
            public int prefix { set; get; } 
            public byte[] content { set; get; }

            public byte[] Build()
            {
                byte[] data = new byte[(uint)(512+size)];

                ConvertTo(name, data, 0);
                ConvertTo(mode, data, 100);
                //ConvertTo(uid, data, 108); // USERID
                //ConvertTo(gid, data, 116); //GROUPID
                ConvertTo(ConvertToOctal(size, 11), data, 124);
                data[156] = (byte)typeflag;

                //Content
                ConvertTo(content, data, 512);
                
                return data;
            }
        }

        public TarFS()
        {
            Console.WriteLine("[Initrd] Initializing Ramdisk");
        }

        static void ConvertTo(string value, byte[] data, int start)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            byte[] buffer = Encoding.ASCII.GetBytes(value);

            for (int i = 0; i < buffer.Length; i++)
            {
                data[start + i] = buffer[i];
            }

            buffer.Dispose();
        }

        static void ConvertTo(byte[] buffer, byte[] data, int start)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return;
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                data[start + i] = buffer[i];
            }

            buffer.Dispose();
        }

        static string ConvertToOctal(ulong value, int size)
        {
            char[] buffer = new char[size];

            int octalNumber = 0;
            int exp = 1;

            while (value > 0)
            {
                int digit = (int)(value % 8);
                octalNumber += digit * exp;
                value /= 8;
                exp *= 10;
            }

            string octalSting = octalNumber.ToString();

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = '0';
            }

            for (int i = (size - octalSting.Length) - 1; i >= 0; i--)
            {
                buffer[(octalSting.Length + i)] = (char)(octalSting[i]);
            }
            return new string(buffer);
        }

        [DllImport("*")]
        static extern ulong mystrtoul(byte* nptr, byte** endptr, int @base);

        public override List<FileInfo> GetFiles(string Directory)
        {
            ulong sec = 0;
            posix_tar_header hdr;
            posix_tar_header* ptr = &hdr;

            List<FileInfo> list = new List<FileInfo>();
            while (DiskRam.Instance.Read(sec, 1, (byte*)ptr) && hdr.name[0])
            {
                sec++;
                ulong size = mystrtoul(hdr.size, null, 8);
                string name = string.FromASCII((nint)hdr.name, Strings.strlen(hdr.name) - (hdr.name[Strings.strlen(hdr.name) - 1] == '/' ? 1 : 0));

                if (IsInDirectory(name, Directory))
                {
                    FileInfo info = new FileInfo();
                    info.Param0 = sec;
                    info.Size = size;
                    info.Name = name.Substring(name.LastIndexOf('/') + 1);
                    info.Ext = "";

                    if (hdr.typeflag == '5') info.Attribute |= FileAttribute.Directory;
                    list.Add(info);
                }
                name.Dispose();
                sec += SizeToSec(size);
            }
            return list;
        }

        public override byte[] ReadAllBytes(string Name)
        {
            string dir = null;
            if (Name.IndexOf('/') == -1)
            {
                dir = "";
            }
            else
            {
                dir = $"{Name.Substring(0, Name.LastIndexOf('/'))}/";
            }

            string fname = Name.Substring(dir.Length);
            byte[] buffer = null;
            List<FileInfo> list = GetFiles(dir);

            for (int i = 0; i < list.Count; i++)
            {
                if (!string.IsNullOrEmpty(list[i].Name) && list[i].Name.Equals(fname))
                {
                    buffer = new byte[(uint)SizeToSec(list[i].Size) * 512];
                    fixed (byte* ptr = buffer)
                    {
                        DiskRam.Instance.Read(list[i].Param0, (uint)SizeToSec(list[i].Size), ptr);
                    }
                    buffer.Length = (int)list[i].Size;
                    //Disposing
                    for (i = 0; i < list.Count; i++)
                    {
                        list[i].Dispose();
                    }
                    list.Dispose();
                    fname.Dispose();
                    return buffer;
                }
            }
            //Panic.Error($"{Name} is not found!");
            return buffer;
        }

        ulong GetSec()
        {
            ulong sec = 0;
            posix_tar_header hdr;
            posix_tar_header* ptr = &hdr;

            while (DiskRam.Instance.Read(sec, 1, (byte*)ptr) && hdr.name[0])
            {
                sec++;
                ulong size = mystrtoul(hdr.size, null, 8);
                sec += SizeToSec(size);
            }
            return sec;
        }

        public override void WriteAllBytes(string name, byte[] content) 
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (name[0] == '/')
            {
                name = name.Substring(1);
            }

            int index = name.LastIndexOf('/');
            string dir = name.Substring(0, index + 1);
            ulong sec = GetSec();

            
            TarHeader hdr = new TarHeader();

            hdr.name = name;
            hdr.mode = "00100664";
            hdr.typeflag = '0'; // NORMAL
            hdr.size = content.Length;
            hdr.content = content;

            fixed (byte* ptr = hdr.Build())
            {
                DiskRam.Instance.Write(sec, (uint)(hdr.size+512), ptr);
            }

            dir.Dispose();
        }

        public override void CreateDirectory(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (name[0] == '/')
            {
                name = name.Substring(1);
            }

            if (name[name.Length-1] == '/')
            {
                name = name.Substring(0, name.Length - 1);
            }

            ulong sec = GetSec();

            TarHeader hdr = new TarHeader();

            hdr.name = name;
            hdr.mode = "0000000";
            hdr.typeflag = '5'; // DIRECTORY
            hdr.size = 0;

            fixed (byte* ptr = hdr.Build())
            {
                DiskRam.Instance.Write(sec, (uint)(hdr.size + 512), ptr);
            }
        }

        public override void Format() 
        { 

        }

        public override void Delete(string Name)
        {

        }

    }
}
