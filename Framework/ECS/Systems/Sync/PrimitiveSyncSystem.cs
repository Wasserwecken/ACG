using Framework.ECS.Components.Scene;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.Extensions;
using Framework.Assets.Verticies.Attributes;
using OpenTK.Mathematics;

namespace Framework.ECS.Systems.Sync
{
    public class PrimitiveSyncSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderDataComponent = sceneComponents.First(f => f is RenderDataComponent) as RenderDataComponent;

            foreach(var primitive in renderDataComponent.Primitves)
                if (primitive.Handle <= 0)
                    Push(primitive);

            if (sceneComponents.TryGet<SkyboxComponent>(out var skyboxComponent))
                foreach (var primitive in skyboxComponent.Mesh.Primitives)
                    if (primitive.Handle <= 0)
                        Push(primitive);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Push(VertexPrimitiveAsset primitive)
        {
            // creating buffer bytes
            //if (!primitive.ArrayBuffer.Attributes.Any(f => f.Name == Definitions.Shader.Attribute.Tangent.Name))
                CreateTangets(primitive);

            var arrayBuffer = CreateBufferArrayData(primitive.ArrayBuffer);
            var indicieBuffer = CreateBufferIndicieData(primitive.IndicieBuffer);

            // GPU buffer reservation
            primitive.Handle = GL.GenVertexArray();
            primitive.ArrayBuffer.Handle = GL.GenBuffer();

            // send arraybuffer to GPU
            GL.BindVertexArray(primitive.Handle);
            GL.BindBuffer(primitive.ArrayBuffer.Target, primitive.ArrayBuffer.Handle);
            GL.BufferData(primitive.ArrayBuffer.Target, arrayBuffer.Length, arrayBuffer, primitive.ArrayBuffer.UsageHint);

            // send attribute infos to GPU
            var offset = 0;
            foreach (var attribute in primitive.ArrayBuffer.Attributes)
            {
                GL.VertexAttribPointer(attribute.Layout, attribute.Dimension, attribute.PointerType, attribute.Normalize, primitive.ArrayBuffer.ElementSize, offset);
                GL.EnableVertexAttribArray(attribute.Layout);
                offset += attribute.ElementSize;
            }

            // send indicie info to GPU
            if (primitive.IndicieBuffer != null)
            {
                primitive.IndicieBuffer.Handle = GL.GenBuffer();
                GL.BindBuffer(primitive.IndicieBuffer.Target, primitive.IndicieBuffer.Handle);
                GL.BufferData(primitive.IndicieBuffer.Target, indicieBuffer.Length, indicieBuffer, primitive.IndicieBuffer.UsageHint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateTangets(VertexPrimitiveAsset primitive)
        {
            var positionAttribute = primitive.ArrayBuffer.Attributes.First(f => f.Name == Definitions.Shader.Attribute.Position.Name) as VertexAttributeVector3;
            var uvAttribute = primitive.ArrayBuffer.Attributes.First(f => f.Name == Definitions.Shader.Attribute.UV.Name) as VertexAttributeVector2;
            var tangentAttribute = new VertexAttributeVector4(
                Definitions.Shader.Attribute.Tangent.Name,
                Definitions.Shader.Attribute.Tangent.Layout,
                Definitions.Shader.Attribute.Tangent.Normalize)
            { DataTyped = new Vector4[positionAttribute.ElementCount] };

            for(int i = 0; i < primitive.IndicieBuffer.Indicies.Length; i += 3)
            {
                var i1 = primitive.IndicieBuffer.Indicies[i + 0];
                var i2 = primitive.IndicieBuffer.Indicies[i + 1];
                var i3 = primitive.IndicieBuffer.Indicies[i + 2];
                var p1 = positionAttribute.DataTyped[i1];
                var p2 = positionAttribute.DataTyped[i2];
                var p3 = positionAttribute.DataTyped[i3];
                var uv1 = uvAttribute.DataTyped[i1];
                var uv2 = uvAttribute.DataTyped[i2];
                var uv3 = uvAttribute.DataTyped[i3];

                var edge1 = p2 - p1;
                var edge2 = p3 - p1;
                var uvDelta1 = uv2 - uv1;
                var uvDelta2 = uv3 - uv1;

                float f = 1.0f / (uvDelta1.X * uvDelta2.Y - uvDelta2.X * uvDelta1.Y);
                var tangent = new Vector4(new Vector3(
                        f * (uvDelta2.Y * edge1.X - uvDelta1.Y * edge2.X),
                        f * (uvDelta2.Y * edge1.Y - uvDelta1.Y * edge2.Y),
                        f * (uvDelta2.Y * edge1.Z - uvDelta1.Y * edge2.Z)
                    ).Normalized(), 1
                );

                tangentAttribute.DataTyped[i1] = tangent;
                tangentAttribute.DataTyped[i2] = tangent;
                tangentAttribute.DataTyped[i3] = tangent;
            }

            primitive.ArrayBuffer.Attributes.Add(tangentAttribute);
        }

        /// <summary>
        /// 
        /// </summary>
        private byte[] CreateBufferArrayData(BufferArrayAsset buffer)
        {
            // set buffer size
            var result = new byte[buffer.Attributes[0].ElementCount * buffer.ElementSize];

            // prepare
            foreach (var attribute in buffer.Attributes)
                attribute.UpdateByteData();

            // go through each data element
            for (int i = 0; i < buffer.ElementCount; i++)
            {
                var bufferIndex = i * buffer.ElementSize;

                // copy atribute data for a single element into the buffer object
                foreach (var attribute in buffer.Attributes)
                {
                    var attributeIndex = i * attribute.ElementSize;
                    System.Buffer.BlockCopy(attribute.DataBytes, attributeIndex, result, bufferIndex, attribute.ElementSize);
                    bufferIndex += attribute.ElementSize;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private byte[] CreateBufferIndicieData(BufferIndicieAsset buffer)
        {
            return buffer.Indicies.ToBytes();
        }
    }
}
