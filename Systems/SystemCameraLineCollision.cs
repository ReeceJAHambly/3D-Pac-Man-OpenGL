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
    class SystemCameraLineCollision : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_LINE_COLLISION);

        CollisionManager collisionManager;
        Camera camera;

        public SystemCameraLineCollision()
        {

        }

        public SystemCameraLineCollision(CollisionManager collisionManager, Camera camera)
        {
            this.collisionManager = collisionManager;
            this.camera = camera;
        }

        public String Name
        {
            get { return "SystemCameraLineCollision"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent collComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_LINE_COLLISION;
                });
                ComponentLineCollision collision = (ComponentLineCollision)collComponent;

                Vector2[] points = ((ComponentLineCollision)collision).GetPoints();
                for (int i = 0; i < points.Length - 1; i++)
                {
                    if (CameraToLineCollision(points[i], points[i + 1]))
                    {
                        collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.LINE_LINE);
                    }
                }

                if (CameraToLineCollision(points[0], points[points.Length - 1])) //need to check the last against the first as well
                {
                    collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.LINE_LINE);
                }
            }
        }

        public bool CameraToLineCollision(Vector2 pPoint1, Vector2 pPoint2)
        {
            //get the actual line of the 2 vectors passed in
            Vector2 lineToCheckAgainst = pPoint2 - pPoint1;
            Vector2 wallNormal = new Vector2(-lineToCheckAgainst.Y, lineToCheckAgainst.X);
            wallNormal.Normalize();

            //find the lines from each point to the camera
            Vector2 point1ToCamera = camera.cameraPosition.Xz - pPoint1;
            Vector2 point2ToCamera = camera.cameraPosition.Xz - pPoint2;

            if (point1ToCamera.Length > lineToCheckAgainst.Length || point2ToCamera.Length > lineToCheckAgainst.Length) //if the lineBetween 2 points != the hypoteneuse then it definitely won't collide
            {
                return false;
            }

            //find the angle, between one of the wall points and camera, and the wall, can then use it to find the distance of the camera to the wall. If it's less than the camrea radius, they collide
            double dotOfLines = Vector2.Dot(lineToCheckAgainst, point1ToCamera);
            double angle = Math.Acos((dotOfLines) / (point1ToCamera.Length * lineToCheckAgainst.Length));

            double distanceFromCameraToWall = point1ToCamera.Length * (Math.Sin(angle));

            if (distanceFromCameraToWall <= camera.GetRadius())//1 = camera radius
            {
                return true;
            }


            //create direction vector with 
            return false;


        }
    }
}
