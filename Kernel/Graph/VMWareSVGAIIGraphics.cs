using MOOS.Driver;
using MOOS.FS;
using MOOS.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOOS.Graph
{
    [Obsolete("This graphics does not support unbuffered mode", true)]
    internal unsafe class VMWareSVGAIIGraphics : Graphics
    {
        VMWareSVGAII svga;

        public VMWareSVGAIIGraphics(ushort Width = 800, ushort Height = 600, ushort bbp = 32) : base(Width, Height, null)
        {
            svga = new VMWareSVGAII();
            svga.SetMode(Width, Height);
            Framebuffer.Initialize(Width, Height, bbp, svga.Video_Memory);
            base.VideoMemory = Framebuffer.FirstBuffer;

        }

        public override void Update()
        {
            svga.Update();
        }
    }
}
