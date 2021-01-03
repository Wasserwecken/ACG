using System;
using System.Collections.Generic;
using System.Text;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Linq;

namespace Framework
{
    public class GLTFLoader
    {


        public static void Load()
        {
            var glTF = ModelRoot.Load("./Assets/acg.glb");

            var cameras = new Dictionary<Camera, PerspectiveCameraComponent>();
            var meshs = new Dictionary<Mesh, VertexObjectAsset>();
            var directionalLights = new Dictionary<PunctualLight, DirectionalLightComponent>();
            var pointLights = new Dictionary<PunctualLight, PointLightComponent>();
            var spotLights = new Dictionary<PunctualLight, SpotLightComponent>();

            foreach(var glTFmesh in glTF.LogicalMeshes)
            {
                var primitives = new List<VertexPrimitiveAsset>();

                foreach(var glTFprimitive in glTFmesh.Primitives)
                {
                    var vertexAttributes = new List<VertexAttributeAsset>();
                    foreach (var glTFaccessor in glTFprimitive.VertexAccessors)
                    {
                        var attributeAsset = new VertexAttributeAsset(
                            glTFaccessor.Key,
                            glTFaccessor.Value.LogicalIndex,
                            glTFaccessor.Value.Format.ByteSize,
                            glTFaccessor.Value.Format.Normalized,
                            (VertexAttribPointerType)glTFaccessor.Value.Format.Encoding
                        );

                        VertexAttributeSystem.Update(
                            attributeAsset,
                            glTFaccessor.Value.SourceBufferView.Content.ToArray()
                        );

                        vertexAttributes.Add(attributeAsset);
                    }

                    var arrayBuffer = new ArrayBufferAsset(vertexAttributes.ToArray(), BufferUsageHint.StaticDraw);
                    ArrayBufferSystem.Update(arrayBuffer);

                    var indicieBuffer = new IndicieBufferAsset(BufferUsageHint.StaticDraw);
                    IndicieBufferSystem.Update(glTFprimitive.GetIndices().ToArray(), indicieBuffer);

                    primitives.Add(new VertexPrimitiveAsset(arrayBuffer, indicieBuffer)
                    {
                        Mode = PolygonMode.Fill,
                        Type = (OpenTK.Graphics.OpenGL.PrimitiveType)glTFprimitive.DrawPrimitiveType
                    });
                }

                meshs.Add(glTFmesh, new VertexObjectAsset(glTFmesh.Name, primitives.ToArray()));
            }

            foreach(var glTFlight in glTF.LogicalPunctualLights)
            {
                switch(glTFlight.LightType)
                {
                    case PunctualLightType.Directional:
                        directionalLights.Add(glTFlight, new DirectionalLightComponent()
                        {
                            Color = new Vector3(glTFlight.Color.X, glTFlight.Color.Y, glTFlight.Color.Z) * glTFlight.Intensity,
                            AmbientFactor = 0.02f,
                        });
                        break;

                    case PunctualLightType.Point:
                        pointLights.Add(glTFlight, new PointLightComponent()
                        {
                            Color = new Vector3(glTFlight.Color.X, glTFlight.Color.Y, glTFlight.Color.Z) * glTFlight.Intensity,
                            AmbientFactor = 0.02f,
                        });
                        break;

                    case PunctualLightType.Spot:
                        spotLights.Add(glTFlight, new SpotLightComponent()
                        {
                            Color = new Vector3(glTFlight.Color.X, glTFlight.Color.Y, glTFlight.Color.Z) * glTFlight.Intensity,
                            AmbientFactor = 0.02f,
                            OuterAngle = glTFlight.OuterConeAngle,
                            InnerAngle = glTFlight.InnerConeAngle,
                        });
                        break;
                }
            }

            foreach(var glTFcamera in glTF.LogicalCameras)
            {
                if (glTFcamera.Settings.IsPerspective)
                {
                    var glTFperspective = (CameraPerspective)glTFcamera.Settings;
                    cameras.Add(glTFcamera, new PerspectiveCameraComponent()
                    {
                        NearClipping = glTFperspective.ZNear,
                        FarClipping = glTFperspective.ZFar,
                        FieldOfView = glTFperspective.VerticalFOV,
                        ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit,
                        ClearColor = new Vector4(0.3f)
                    });
                }
            }

        }
    }
}
