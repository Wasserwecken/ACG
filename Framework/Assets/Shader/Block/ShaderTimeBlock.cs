﻿using Framework.ECS.Components.Scene;
using System.IO;

namespace Framework.Assets.Shader.Block
{
    public class ShaderTimeBlock : ShaderBlockBase
    {
        public TimeComponent Time;


        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Time.Total);
            writer.Write(Time.TotalSin);
            writer.Write(Time.DeltaFrame);
            writer.Write(Time.DeltaFixed);
        }
    }
}
