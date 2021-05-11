using ACG.Framework.Assets;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.ECS.Systems.Render
{
    public static class Renderer
    {
        /// <summary>
        /// 
        /// </summary>
        public static void UseFrameBuffer(FramebufferAsset framebuffer)
        {
            if (framebuffer.Handle <= 0)
                GPUSync.Push(framebuffer);

            GL.BindFramebuffer(framebuffer.Target, framebuffer.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UseShader(ShaderProgramAsset shader)
        {
            GL.UseProgram(shader.Handle);

            foreach (var binding in shader.BlockBindings.Values)
                GL.BindBufferBase(binding.Target, binding.Layout, binding.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UseShaderBlock(IShaderBlock block, ShaderProgramAsset shader)
        {
            if (shader.IdentifierToLayout.TryGetValue(block.Name, out var blockLayout))
                GL.BindBufferBase(block.Target, blockLayout, block.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UseMaterial(MaterialAsset material, ShaderProgramAsset shader)
        {
            // MATERIAL SETTINGS
            GL.ShadeModel(material.Model);
            GL.FrontFace(material.FaceDirection);

            if (material.IsDepthTesting)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(material.DepthTest);
            }
            else
                GL.Disable(EnableCap.DepthTest);

            if (material.IsCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(material.CullingMode);
            }
            else
                GL.Disable(EnableCap.CullFace);

            if (material.IsTransparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(material.SourceBlend, material.DestinationBlend);
            }
            else
                GL.Disable(EnableCap.Blend);

            // MATERIAL UNIFORMS
            foreach (var uniform in material.UniformFloats)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                    GL.Uniform1(layout, uniform.Value);

            foreach (var uniform in material.UniformVecs)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                    GL.Uniform4(layout, uniform.Value);

            foreach (var uniform in material.UniformMats)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                {
                    var foo = uniform.Value;
                    GL.UniformMatrix4(layout, false, ref foo);
                }

            foreach (var uniform in material.UniformTextures)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                {
                    GL.Uniform1(layout, layout);
                    GL.ActiveTexture(TextureUnit.Texture0 + layout);
                    GL.BindTexture(uniform.Value.Target, uniform.Value.Handle);
                }

            foreach (var uniformTexture in shader.UniformInfos.Where
                (f => f.Type == ActiveUniformType.Sampler2D && !material.UniformTextures.ContainsKey(f.Name)))
            {
                GL.Uniform1(uniformTexture.Layout, uniformTexture.Layout);
                GL.ActiveTexture(TextureUnit.Texture0 + uniformTexture.Layout);

                if (uniformTexture.Name.ToLower().Contains("normal"))
                    GL.BindTexture(Defaults.Texture.Normal.Target, Defaults.Texture.Normal.Handle);
                else
                    GL.BindTexture(Defaults.Texture.White.Target, Defaults.Texture.White.Handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Draw(VertexPrimitiveAsset primitive)
        {
            if (primitive.Handle <= 0)
                GPUSync.Push(primitive);

            GL.BindVertexArray(primitive.Handle);
            GL.PolygonMode(MaterialFace.FrontAndBack, primitive.Mode);

            if (primitive.IndicieBuffer != null)
                GL.DrawElements(primitive.Type, primitive.IndicieBuffer.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(primitive.Type, 0, primitive.ArrayBuffer.ElementCount);
        }

    }
}
