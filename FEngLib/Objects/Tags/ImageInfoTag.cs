﻿using System.IO;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Tags;

namespace FEngLib.Object.Tags
{
    public class ImageInfoTag : Tag
    {
        public ImageInfoTag(IObject<ObjectData> frontendObject) : base(frontendObject)
        {
        }

        public uint ImageFlags { get; set; }

        public override void Read(BinaryReader br, FrontendChunkBlock chunkBlock, Package package,
            ushort id,
            ushort length)
        {
            ImageFlags = br.ReadUInt32();
        }
    }
}