using MOOS.FS;
using MOOS.NET;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MOOS.Misc
{
    public unsafe class PNG : Image
    {
        public enum LodePNGColorType
        {
            LCT_GREY = 0, /*greyscale: 1,2,4,8,16 bit*/
            LCT_RGB = 2, /*RGB: 8,16 bit*/
            LCT_PALETTE = 3, /*palette: 1,2,4,8 bit*/
            LCT_GREY_ALPHA = 4, /*greyscale with alpha: 8,16 bit*/
            LCT_RGBA = 6 /*RGB with alpha: 8,16 bit*/
        }

        public PNG(string file)
        {
            onLoad(RamFile.Instance.ReadAllBytes(file), LodePNGColorType.LCT_RGBA, 8);
        }

        public PNG(byte[] file,LodePNGColorType type = LodePNGColorType.LCT_RGBA ,uint bitDepth = 8)
        {
            onLoad(file, type, bitDepth);
        }

        void onLoad(byte[] file, LodePNGColorType type = LodePNGColorType.LCT_RGBA, uint bitDepth = 8)
        {
            lock (this)
            {
                if (file == null)
                {
                    Width = 0;
                    Height = 0;
                    Bpp = 4;
                    RawData =new int[0];
                    return;
                }

                fixed (byte* p = file)
                {
                    lodepng_decode_memory(out uint* _out, out uint w, out uint h, p, file.Length, type, bitDepth);

                    if (_out == null) Panic.Error("lodepng error");
                    RawData = new int[w * h];
                    fixed (int* pdata = RawData)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            for (int y = 0; y < h; y++)
                            {
                                RawData[y * w + x] = (int)((_out[y * w + x] & 0xFF000000) | (SwapLeftRight32(_out[y * w + x] & 0x00FFFFFF)) >> 8);
                            }
                        }
                    }
                    Allocator.Free((System.IntPtr)_out);
                    Width = (int)w;
                    Height = (int)h;
                    Bpp = 4;
                }
            }
        }

        uint SwapLeftRight32(uint x)
        {
            return ((x & 0x000000ff) << 24) +
              ((x & 0x0000ff00) << 8) +
              ((x & 0x00ff0000) >> 8) +
              ((x & 0xff000000) >> 24);
        }

        [DllImport("*")]
        public static extern void lodepng_decode_memory(out uint* _out, out uint w, out uint h, byte* _in, int insize, LodePNGColorType colortype, uint bitdepth);
    }
}