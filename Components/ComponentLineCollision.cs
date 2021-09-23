using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenGL_Game.Components
{
    class ComponentLineCollision : IComponent
    {
        public Vector2[] Points;

        public ComponentLineCollision()
        {
            
        }

        public ComponentLineCollision(Vector2[] points)
        {
            Points = points;
        }

        public Vector2[] GetPoints()
        {
            return Points;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_LINE_COLLISION; }
        }

        public void Close()
        {

        }

    }
}
