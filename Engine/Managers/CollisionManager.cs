using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Objects;

namespace OpenGL_Game.Managers
{
    enum COLLISIONTYPE
    {
        SPHERE_SPHERE,
        LINE_LINE
    }

    struct Collision
    {
        public Entity entity;
        public COLLISIONTYPE collisionType;
    }
    abstract class CollisionManager
    {
        protected List<Collision> collisionManifold = new List<Collision>();
        public CollisionManager()
        {

        }

        public void ClearManifold()
        {
            collisionManifold.Clear();
        }

        public void CollisionBetweenCamera(Entity entity, COLLISIONTYPE collisionType)
        {
            //If collision is in manifold, we do not need to add it
            foreach(Collision coll in collisionManifold)
            {
                if(coll.entity == entity)
                {
                    return;
                }
            }

            Collision collision;
            collision.entity = entity;
            collision.collisionType = collisionType;
            collisionManifold.Add(collision);
        }

        public abstract void ProcessCollisions();
    }
}
