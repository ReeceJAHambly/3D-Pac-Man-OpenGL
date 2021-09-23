using System.Collections.Generic;
using OpenGL_Game.Systems;
using OpenGL_Game.Objects;

namespace OpenGL_Game.Managers
{
    class SystemManager
    {
        List<ISystem> systemList = new List<ISystem>();

        public SystemManager()
        {
        }

        public void ActionSystems(EntityManager entityManager,bool Movement, bool collidable)
        {
            List<Entity> entityList = entityManager.Entities();
            foreach(ISystem system in systemList)
            {
                foreach(Entity entity in entityList)
                {
                    if(entity.Name.Contains("Wall") && system.Name == "SystemCameraLineCollision")
                    {
                        if(collidable)
                        {
                            system.OnAction(entity);
                        }
                    }
                    else if(entity.Name.Contains("Patroller") || entity.Name.Contains("Enemy"))
                    {
                        if(Movement == true)
                        {
                            system.OnAction(entity);                           
                        }
                        else
                        {
                            string RenderName = "SystemRender";
                            if (RenderName == system.Name)
                            {
                                system.OnAction(entity);
                            }
                        }
                       
                    }
                    else
                    {
                        system.OnAction(entity);
                    }
                   
                }
            }
        }

        public void AddSystem(ISystem system)
        {
            ISystem result = FindSystem(system.Name);
            //Debug.Assert(result != null, "System '" + system.Name + "' already exists");
            systemList.Add(system);
        }

        public ISystem GetSystem(string name)
        {
            return (FindSystem(name));
        }

        private ISystem FindSystem(string name)
        {
            return systemList.Find(delegate(ISystem system)
            {
                return system.Name == name;
            }
            );
        }
    }
}
