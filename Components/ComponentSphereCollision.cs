using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentSphereCollision : IComponent
    {
        float radius;

        public ComponentSphereCollision(float pRadius)
        {
            radius = pRadius;
        }

        public float GetRadius()
        {
            return radius;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_SPHERE_COLLISION; }
        }

        public void Close()
        {

        }

    }
}
