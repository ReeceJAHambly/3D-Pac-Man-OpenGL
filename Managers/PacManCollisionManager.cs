using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenGL_Game.Scenes;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;

namespace OpenGL_Game.Managers
{
    class PacManCollisionManager : CollisionManager
    {
        GameScene gameScene;
        public float startTime;
        public PacManCollisionManager(GameScene gameScene)
        {
            this.gameScene = gameScene;
            
        }

        public override void ProcessCollisions()
        {        
            if (collisionManifold.Count > 0)
            {
                foreach (Collision collision in collisionManifold)
                {
                    string entityName = collision.entity.Name;
                    if(entityName.Contains("PowerUp"))
                    {
                        PowerUpCollisionCamera(collision.entity);
                    }
                    if (entityName.Contains("Enemy") || entityName.Contains("Patroller"))
                    {
                        if(gameScene.PowerUpActive == true)
                        {
                            MoonPowerUpCollisionCamera(collision.entity);
                        }
                        else
                        {
                            MoonCollisionCamera(collision.entity);
                        }
                        
                    }
                    if (entityName.Contains("Ball"))
                    {
                        BallCollisionCamera(collision.entity);
                    }
                        
                    if (entityName.Contains("Wall"))
                    {
                        WallCollisionCamera(collision.entity);
                    }
                }
             }           
            ClearManifold();
        }

        private void PowerUpCollisionCamera(Entity entity)
        {
            startTime = gameScene.time;
            gameScene.PowerUpActive = true;
            gameScene.score++;
            gameScene.remaining--;

            List<IComponent> components = entity.Components;

            IComponent audioComponent = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
            });

            ((ComponentAudio)audioComponent).NonLooping();


            gameScene.entityManager.RemoveEntity(entity.Name);
        }
        private void MoonPowerUpCollisionCamera(Entity entity)
        {
            gameScene.ResetOneEnemy(entity.Name);
        }
        private void MoonCollisionCamera(Entity entity)
        {
            gameScene.camera.cameraPosition = new Vector3(-7.5f, 1, 7.5f);
            gameScene.camera.cameraDirection = new Vector3(-7.5f, 1, 8.5f) - gameScene.camera.cameraPosition;
            gameScene.lives--;
            gameScene.ResetEnemies();
            gameScene.camera.UpdateView();
        }
        private void BallCollisionCamera(Entity entity)
        {
            if(gameScene.PowerUpActive == true) //Doubles score after eating PowerUp
            {
                gameScene.score = gameScene.score + 2;
            }
            else
            {
                gameScene.score++;
            }
            
            gameScene.remaining--;

            List<IComponent> components = entity.Components;

            IComponent audioComponent = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
            });

            ((ComponentAudio)audioComponent).NonLooping();

            gameScene.entityManager.RemoveEntity(entity.Name);
        }

        private void WallCollisionCamera(Entity entity)
        {           
            gameScene.camera.cameraPosition = gameScene.camera.previousCameraPosition;           
            gameScene.camera.UpdateView();
        }
    }
}
