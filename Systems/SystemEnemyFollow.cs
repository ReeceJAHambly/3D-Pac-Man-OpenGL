using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenTK;

namespace OpenGL_Game.Systems
{
    class SystemEnemyFollow : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_ENEMY
            | ComponentTypes.COMPONENT_VELOCITY | ComponentTypes.COMPONENT_AUDIO);
        Camera camera;
        Dictionary<Entity, bool> currentEnemies;

        public SystemEnemyFollow(ref Camera pCamera)
        {
            camera = pCamera;
            currentEnemies = new Dictionary<Entity, bool>();
        }

        public string Name
        {
            get { return "SystemEnemyFollow"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                bool value;
                if (!currentEnemies.TryGetValue(entity, out value))
                {
                    currentEnemies.Add(entity, false);
                }

                List<IComponent> components = entity.Components;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });

                IComponent velocityComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_VELOCITY;
                });
                Vector3 velocity = ((ComponentVelocity)velocityComponent).Velocity;

                IComponent audioComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
                });



                IComponent travellerComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_ENEMY;
                });
                Vector3 position = ((ComponentPosition)positionComponent).Position;
                Vector3 destination = ((ComponentEnemy)travellerComponent).Destination;
                Dictionary<Vector3, Vector3[]> neighbours = ((ComponentEnemy)travellerComponent).Neighbours;

                Vector3 vectorBetween = camera.cameraPosition - position;

                if (Vector3.Dot(velocity, vectorBetween) > 0)
                {
                    if (currentEnemies[entity] == true)
                    {
                        if (Vector3.Distance(destination, position) <= 0.1f)//then it has reached that node
                        {
                            destination = FindClosestNodeToPlayer(neighbours[destination]);
                            ((ComponentEnemy)travellerComponent).Destination = destination;
                        }
                    }
                    else
                    {
                        ((ComponentAudio)audioComponent).NonLooping();
                    }
                    currentEnemies[entity] = true;
                }
                else
                {
                    if (currentEnemies[entity] == true)
                    {
                        if (Vector3.Distance(destination, position) <= 0.1f)//then it has reached that node
                        {
                            destination = FindClosestNodeToPlayer(neighbours[destination]);
                            ((ComponentEnemy)travellerComponent).Destination = destination;
                        }
                    }
                    currentEnemies[entity] = false;
                }

                if (currentEnemies[entity] == false)
                {
                    if (Vector3.Distance(destination, position) <= 0.1f)//then it has reached that node
                    {
                        destination = FindClosestNodeToPlayer(neighbours[destination]);
                        ((ComponentEnemy)travellerComponent).Destination = destination;
                    }

                    ((ComponentVelocity)velocityComponent).Velocity = CreateVelocityDirection(position, destination);
                }
                else
                {
                    
                    if (Vector3.Distance(camera.cameraPosition, position) < Vector3.Distance(destination, position))
                    {
                        ((ComponentVelocity)velocityComponent).Velocity = CreateVelocityDirection(position, camera.cameraPosition);

                    }
                    else
                    {
                        if (Vector3.Distance(destination, position) <= 0.1f)//then it has reached that node
                        {
                            destination = FindClosestNodeToPlayer(neighbours[destination]);
                            ((ComponentEnemy)travellerComponent).Destination = destination;
                        }

                        ((ComponentVelocity)velocityComponent).Velocity = CreateVelocityDirection(position, destination);
                    }
                }
            }
        }

        public Vector3 CreateVelocityDirection(Vector3 entityPosition, Vector3 nodePosition)
        {
            Vector3 direction = nodePosition - entityPosition;
            direction.Normalize();
            direction *= new Vector3(1.5f, 1.5f, 1.5f);
            return direction;
        }

        public Vector3 FindClosestNodeToPlayer(Vector3[] neighbours)
        {
            float closestDist = -1;
            Vector3 closestNode = new Vector3(0, 0, 0); //means can't have a position of 0,0,0
            for (int i = 0; i < neighbours.Length; i++)
            {
                Vector3 index = neighbours[i];
                if (Vector3.Distance(new Vector3(0, 0, 0), closestNode) == 0)
                {
                    closestNode = index;
                    closestDist = Vector3.Distance(camera.cameraPosition, neighbours[i]);
                }
                else
                {
                    float newDist = Vector3.Distance(camera.cameraPosition, neighbours[i]);
                    if (newDist != 0 && newDist < closestDist)
                    {
                        closestDist = newDist;
                        closestNode = index;
                    }
                }
            }
            return closestNode;
        }
    }
}
