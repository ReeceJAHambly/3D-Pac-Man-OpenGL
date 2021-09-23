using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentBumpRender : IComponent
    {
        public ComponentBumpRender()
        {

        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_BUMP_RENDER; }
        }

        public void Close()
        {

        }
    }
}
