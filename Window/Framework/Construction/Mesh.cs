﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class MeshComponent
    {
        public TransformData Transform { get; set; }
        public MeshObject Mesh { get; set; }
        public Material Material { get; set; }
    }
}
