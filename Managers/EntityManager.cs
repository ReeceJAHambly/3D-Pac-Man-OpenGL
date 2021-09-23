using System.Collections.Generic;
using OpenGL_Game.Objects;
using System.Diagnostics;
using OpenGL_Game.Components;

namespace OpenGL_Game.Managers
{
    class EntityManager
    {
        List<Entity> entityList;
        int count = 0;

        public EntityManager()
        {
            entityList = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            Entity result = FindEntity(entity.Name);
            Debug.Assert(result == null, "Entity '" + entity.Name + "' already exists");
            entityList.Add(entity);
        }
        public int CalculateNumberOfPoints()
        {
           
            foreach(Entity var in entityList)
            {
                if (var.Name.Contains("Ball") || var.Name.Contains("PowerUp"))
                {
                    count++;
                    //return count;
                }
            }
            return count;
        }

        private Entity FindEntity(string name)
        {
            return entityList.Find(delegate(Entity e)
            {
                return e.Name == name;
            }
            );
        }

        public void RemoveEntity(string name)
        {
            for(int i = 0; i < entityList.Count; i++)
            {
                if(entityList[i].Name == name)
                {
                    entityList.RemoveAt(i);
                    return;
                }
                else if(entityList[i].Name.Contains(name))
                {
                    entityList.RemoveAt(i);
                    return;
                }
            }
        }
        public void Close()
        {
            for(int i = 0; i < entityList.Count; i++)
            {
                entityList[i].Close();
            }
        }
        public List<Entity> Entities()
        {
            return entityList;
        }
       
    }
}
