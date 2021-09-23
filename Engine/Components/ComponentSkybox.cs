using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentSkybox : IComponent
    {
        public ComponentSkybox() 
        {

        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_SKYBOX; }
        }


        public void Close()
        {

        }
    }
}
