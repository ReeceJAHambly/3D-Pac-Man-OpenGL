using System;

namespace OpenGL_Game.Components
{
    [FlagsAttribute]
    enum ComponentTypes {
        COMPONENT_NONE     = 0,
	    COMPONENT_POSITION = 1 << 0,
        COMPONENT_GEOMETRY = 1 << 1,
        COMPONENT_TEXTURE  = 1 << 2,
        COMPONENT_VELOCITY = 1 << 3,
        COMPONENT_AUDIO    = 1 << 4,
        COMPONENT_SPHERE_COLLISION = 1 << 5,
        COMPONENT_LINE_COLLISION = 1 << 6,
        COMPONENT_ENEMY = 1 << 7,
        COMPONENT_SKYBOX = 1 << 8,
        COMPONENT_BUMP_RENDER = 1 << 9
    }

    interface IComponent
    {
        void Close();
        ComponentTypes ComponentType
        {
            get;
        }
    }
}
