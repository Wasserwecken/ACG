using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Framework
{
    public static class Definitions
    {
        public static class Buffer
        {
            public static Dictionary<string, VertexAttributeAsset> Attributes => new Dictionary<string, VertexAttributeAsset>()
            {
                {"POSITION", new VertexAttributeAsset("POSITION"        , 0, 12, false, VertexAttribPointerType.Float) },
                {"NORMAL", new VertexAttributeAsset("NORMAL"            , 1, 12, false, VertexAttribPointerType.Float) },
                {"TANGENT", new VertexAttributeAsset("TANGENT"          , 2, 16, false, VertexAttribPointerType.Float) },
                {"TEXCOORD_0", new VertexAttributeAsset("TEXCOORD_0"    , 3, 8, false, VertexAttribPointerType.Float) },
                {"TEXCOORD_1", new VertexAttributeAsset("TEXCOORD_1"    , 4, 8, false, VertexAttribPointerType.Float) },
                {"COLOR_0", new VertexAttributeAsset("COLOR_0"          , 5, 16, false, VertexAttribPointerType.Float) },
            };
        }
    }
}
