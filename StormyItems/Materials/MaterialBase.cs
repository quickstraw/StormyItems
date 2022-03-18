using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StormyItems.Materials
{
    public abstract class MaterialBase
    {
        public Shader Shader;
        public Material Material;

        public abstract void Init();
    }
}
