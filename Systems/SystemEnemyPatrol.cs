using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenTK;

namespace OpenGL_Game.Systems
{
    class SystemEnemyPatrol : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_ENEMY | ComponentTypes.COMPONENT_VELOCITY);
        const ComponentTypes ENEMYMASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_ENEMY | ComponentTypes.COMPONENT_VELOCITY | ComponentTypes.COMPONENT_AUDIO);
        Camera camera;


        public SystemEnemyPatrol(ref Camera pCamera)
        {
           camera = pCamera;
        }

        public string Name
        {
            get { return "SystemEnemyPatrol"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK && (entity.Mask & ENEMYMASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });

                IComponent velocityComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_VELOCITY;
                });
                Vector3 velocitiy = ((ComponentVelocity)velocityComponent).Velocity;

                IComponent patrolComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_ENEMY;
                });
                Vector3 position = ((ComponentPosition)positionComponent).Position;
                Vector3 destination = ((ComponentEnemy)patrolComponent).Destination;
                Dictionary<Vector3, Vector3[]> neighbours = ((ComponentEnemy)patrolComponent).Neighbours;


                if(Vector3.Distance(position,destination) < 0.1f || (position.Xzy == destination.Xzy))
                {
                    Vector3 newDestination = neighbours[destination][0];
                    ((ComponentEnemy)patrolComponent).Destination = newDestination;
                    ((ComponentVelocity)velocityComponent).Velocity = CreateVelocityDirection(position, newDestination);
                }
            }
        }

        private Vector3 CreateVelocityDirection(Vector3 currentPostition, Vector3 destinationPosition)
        {
            Vector3 direction = destinationPosition - currentPostition;
            direction.Normalize();
            direction *= new Vector3(2f, 2f, 2f);
            return direction;
        }



    }
}
