using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public abstract class SystemBase
    {
        public virtual Type[] IO { get; }
    }


    public class TestSystem : SystemBase
    {

    }
}
