using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Components;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Systems
{
    class SystemCollisionSphere : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_COLLISION_SPHERE);

        CollisionManager CollisionManager;
        Camera camera;

        public SystemCollsionCameraSphere()
        {

        }

        public SystemCollisionCameraSphere(CollisionManager collisionManager, Camera camera)
        {
            this.collisionManager = collisionManager;
            this.camera = camera;
        }

        public string Name
        {
            get { return "SystemCollisionSphere"; }
        }

        public void OnActio(Entity entity)
        {
            if((entity.Mask & MASK))
            {
                List<IComponent> components = entity.Components;

                IComponent collComponent = components.Find(delegate (IComponent component)
                { 
                    return component.ComponentType == ComponentTypes.COMPONENET_COLLISION_SPHERE;
                });
                ComponentCollisionSphere collision = (ComponentCollisionSphere)collComponent;
            }
        }
    }
}
