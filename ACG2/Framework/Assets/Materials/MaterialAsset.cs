using System.Collections.Generic;
using System.Diagnostics;
using Framework.Assets.Textures;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework.Assets.Materials
{
    [DebuggerDisplay("Name: {Name}, Uniforms: {UniformFloats.Count + UniformVecs.Count + UniformMats.Count}")]
    public class MaterialAsset
    {
        public string Name { get; set; }
        public bool IsTransparent { get; set; }
        public bool IsCulling { get; set; }
        public bool IsDepthTesting { get; set; }
        public ShadingModel Model { get; set; }
        public CullFaceMode CullingMode { get; set; }
        public FrontFaceDirection FaceDirection { get; set; }
        public BlendingFactor SourceBlend { get; set; }
        public BlendingFactor DestinationBlend { get; set; }

        public Dictionary<string, float> UniformFloats;
        public Dictionary<string, Vector4> UniformVecs;
        public Dictionary<string, Matrix4> UniformMats;
        public Dictionary<string, TextureBaseAsset> UniformTextures;

        /// <summary>
        /// 
        /// </summary>
        public MaterialAsset(string name)
        {
            Name = name;

            IsTransparent = false;
            IsCulling = false;
            IsDepthTesting = true;
            Model = ShadingModel.Smooth;
            CullingMode = CullFaceMode.Back;
            FaceDirection = FrontFaceDirection.Ccw;
            SourceBlend = BlendingFactor.SrcAlpha;
            DestinationBlend = BlendingFactor.OneMinusSrcAlpha;

            UniformFloats = new Dictionary<string, float>();
            UniformVecs = new Dictionary<string, Vector4>();
            UniformMats = new Dictionary<string, Matrix4>();
            UniformTextures = new Dictionary<string, TextureBaseAsset>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Use()
        {
            GL.ShadeModel(Model);
            GL.FrontFace(FaceDirection);

            if (IsDepthTesting)
                GL.Enable(EnableCap.DepthTest);

            if (IsCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullingMode);
            }
            else
                GL.Disable(EnableCap.CullFace);

            if (IsTransparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(SourceBlend, DestinationBlend);
            }
            else
                GL.Disable(EnableCap.Blend);
        }

        public void SetUniform(string name, float value) => SetUniform(name, value, UniformFloats);
        public void SetUniform(string name, Vector4 value) => SetUniform(name, value, UniformVecs);
        public void SetUniform(string name, Matrix4 value) => SetUniform(name, value, UniformMats);
        public void SetUniform(string name, TextureBaseAsset texture) => SetUniform(name, texture, UniformTextures);

        /// <summary>
        /// 
        /// </summary>
        private void SetUniform<TValue>(string name, TValue value, IDictionary<string, TValue> uniforms)
        {
            if (uniforms.ContainsKey(name))
                uniforms[name] = value;
            else
                uniforms.Add(name, value);
        }
    }
}
