using Framework.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.ECS.Components
{
    public class CameraControllerComponent : IComponent
    {
        public float LookSpeed { get; set; }
        public float MoveSpeed { get; set; }
    }
}
