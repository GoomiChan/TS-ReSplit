﻿using System;
using System.IO;

namespace TS2
{
    // Lovely and simple :D
    // Untill I found out it was swizzled, still pretty simple, once you know >,>
    public class Texture
    {
        const byte OPAQUE_ALPHA_VALUE = 127;
        public int ID;
        public int UNK;
        public int Width;
        public int Height;

        public uint[] Palettle;
        public byte[] Pixels;

        public Texture() { }

        public Texture(byte[] Data)
        {
            Load(Data);

        }

        public static Texture Read(BinaryReader R)
        {
            var tex = new Texture();
            tex.Load(R);
            return tex;
        }

        public void Load(byte[] Data)
        {
            using (BinaryReader r = new BinaryReader(new MemoryStream(Data)))
            {
                Load(r);
            }
        }

        public void Load(BinaryReader R)
        {
            ID     = R.ReadInt32();
            UNK    = R.ReadInt32();
            Width  = R.ReadInt32();
            Height = R.ReadInt32();

            Palettle  = new uint[256];
            for (int i = 0; i < Palettle.Length; i++)
            {
                var color = R.ReadUInt32();

                // Scale the alpha, the file has max alpha as 127, so just double it
                byte alpha = (byte)((color & 0xFF) * 2);
                color      = color | (uint)alpha >> 24;

                // Ok so SWIZZLE
                // This was annoying to figure out, unswizzled looked so close to correct D:
                // To get the correct index for a color entry we need to swap bits 3 and 4
                // This was a nice find to figure this out: http://www.mygamedemos.com/projects/PS2X/doxy/texture_8cpp-source.html Line 162
                int swizIdx       = SwizzleIdx(i);
                Palettle[swizIdx] = color;
            }

            var numPixels = (int)(Width * Height);
            Pixels        = R.ReadBytes(numPixels);
        }

        public static int SwizzleIdx(int Idx)
        {
            int swizIdx = (Idx & 231) + ((Idx & 8) << 1) + ((Idx & 16) >> 1);
            return swizIdx;
        }
    }
}
