using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.Components;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Managers;


namespace OpenGL_Game.Systems
{
    class SystemCameraSphereCollision : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_SPHERE_COLLISION);

        CollisionManager collisionManager;
        Camera camera;

        public SystemCameraSphereCollision() 
        {
        }

        public SystemCameraSphereCollision(CollisionManager collisionManager, Camera camera)
        {
            this.collisionManager = collisionManager;
            this.camera = camera;
        }

        public String Name
        {
            get { return "SystemCameraSphereCollision"; }
        }

        public void OnAction(Entity entity)
        {

            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent collComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_SPHERE_COLLISION;
                });
                ComponentSphereCollision collision = (ComponentSphereCollision)collComponent;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });
                ComponentPosition position = ((ComponentPosition)positionComponent);

                Collision(entity, position, collision);

            }
        }

        public void Collision(Entity entity, ComponentPosition position, ComponentSphereCollision coll)
        {
            if((position.Position - camera.cameraPosition).Length < coll.GetRadius() + camera.GetRadius())
            {
                collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.SPHERE_SPHERE);
            }
        }
    }
}
