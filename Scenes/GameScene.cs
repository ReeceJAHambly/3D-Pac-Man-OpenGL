using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Audio.OpenAL;
using OpenGL_Game.OBJLoader;


namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        //Arranged for efficiency
        public EntityManager entityManager;
        SystemManager systemManager;
        PacManCollisionManager collisionManager;
        PacManInputManager inputManager;
        public Camera camera;

        public static GameScene gameInstance;
        public float time = 0; //Time keeping for PowerUps
        bool[] keysPressed = new bool[255];
        public static float dt = 0;
        public static bool collidable = true, movement = true;
        public bool PowerUpActive = false;
        public int score = 0, lives = 3; //To keep score and number of lives
        public int remaining; // How many points they're to collect 
        public float PowerUpTimeLimit = 5; //Can change how long the power-up effect happens

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            collisionManager = new PacManCollisionManager(gameInstance);
            

            // Set the title of the window
            sceneManager.Title = "Game";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            // Set Keyboard events to go to a method in this class
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;
            sceneManager.keyboardUpDelegate += Keyboard_KeyUp;

            // Enable Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);

            // Set Camera
            camera = new Camera(new Vector3(-7.5f,1,7.5f), new Vector3(-28.5f, 1, 28.5f), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f); //Sets camera to face towards centre of arena
            inputManager = new PacManInputManager(ref camera); //InputManger specifically for the game 
            CreateEntities(); //Creates Entities
            CreateSystems(); //Create Systems
        }

        private void CreateEntities()
        {
            Entity newEntity;

            newEntity = new Entity("Maze");  //The arena 
            newEntity.AddComponent(new ComponentGeometry("Geometry/Maze/maze.obj"));
            newEntity.AddComponent(new ComponentPosition(0f, 0f, 0.0f));
            newEntity.AddComponent(new ComponentBumpRender());
            entityManager.AddEntity(newEntity); //Adds the entities to entity manager 

            newEntity = new Entity("SkyBox"); // Was meant for skybox but just makes grey cube near spawn area 
            newEntity.AddComponent(new ComponentGeometry("Geometry/Skybox/SkyBox.obj"));
            newEntity.AddComponent(new ComponentPosition(-8.5f, 1f, 8.5f));
            //newEntity.AddComponent(new ComponentSkybox()); Commented out as it doesnt work
            //entityManager.AddEntity(newEntity);

            newEntity = new Entity("MazeFloor"); //Floor with a metal texture and large surrounding walls separate from the arena 
            newEntity.AddComponent(new ComponentGeometry("Geometry/Floor/Floor.obj"));
            newEntity.AddComponent(new ComponentPosition(-28.5f, -1f, 28.5f)); // Is the centre of the maze
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("PowerUp1"); //PowerUp grants double points and lets you eat the enemies for 5 seconds 
            newEntity.AddComponent(new ComponentGeometry("Geometry/PowerUp/powerup.obj"));
            newEntity.AddComponent(new ComponentPosition(-2.5f, 0.75f, 54.5f));
            newEntity.AddComponent(new ComponentBumpRender()); //Component for the bump rendering 
            newEntity.AddComponent(new ComponentSphereCollision(0.5f)); //Collision detection for spheres component with range from centre 
            newEntity.AddComponent(new ComponentAudio("Audio/ball.wav", -2.5f,0.75f,54.5f));
            remaining++; //Counts how many points they're and increments by one for each
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("PowerUp2"); //PowerUp grants double points and lets you eat the enemies for 5 seconds 
            newEntity.AddComponent(new ComponentGeometry("Geometry/PowerUp/powerup.obj"));
            newEntity.AddComponent(new ComponentPosition(-54.0f, 0.75f, 54.5f));
            newEntity.AddComponent(new ComponentBumpRender());
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            newEntity.AddComponent(new ComponentAudio("Audio/ball.wav", -54.0f,0.75f,54.5f));
            remaining++;
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("PowerUp3"); //PowerUp grants double points and lets you eat the enemies for 5 seconds 
            newEntity.AddComponent(new ComponentGeometry("Geometry/PowerUp/powerup.obj"));
            newEntity.AddComponent(new ComponentPosition(-54.0f, 0.75f, 2.5f));
            newEntity.AddComponent(new ComponentBumpRender());
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            newEntity.AddComponent(new ComponentAudio("Audio/ball.wav", -54.0f, 0.75f, 2.5f));
            remaining++;
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("PowerUp4");  //PowerUp grants double points and lets you eat the enemies for 5 seconds 
            newEntity.AddComponent(new ComponentGeometry("Geometry/PowerUp/powerup.obj"));
            newEntity.AddComponent(new ComponentPosition(-2.5f, 0.75f, 2.5f));
            newEntity.AddComponent(new ComponentBumpRender());
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            newEntity.AddComponent(new ComponentAudio("Audio/ball.wav", -2.5f, 0.75f, 2.5f));
            remaining++;
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("PowerUp5");  //PowerUp grants double points and lets you eat the enemies for 5 seconds 
            newEntity.AddComponent(new ComponentGeometry("Geometry/PowerUp/powerup.obj"));
            newEntity.AddComponent(new ComponentPosition(-28.5f, 0.75f, 28.5f));
            newEntity.AddComponent(new ComponentBumpRender());
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            newEntity.AddComponent(new ComponentAudio("Audio/ball.wav", -28.5f, 0.75f, 28.5f));
            remaining++;
            entityManager.AddEntity(newEntity);

            GeneratePoints(newEntity); //Places the point objects around the map

            newEntity = new Entity("WallOuter");
            //Creates points on the outer walls for the line line collision
            Vector2[] WallOuterPositions = new Vector2[]
            {
                new Vector2(-1,1),
                new Vector2(-16,1),
                new Vector2(-16,6),
                new Vector2(-41,6),
                new Vector2(-41,1),
                new Vector2(-56,1),
                new Vector2(-56,16),
                new Vector2(-51,16),
                new Vector2(-51,41),
                new Vector2(-56,41),
                new Vector2(-56,56),
                new Vector2(-41,56),
                new Vector2(-41,51),
                new Vector2(-16,51),
                new Vector2(-16,56),
                new Vector2(-1,56),
                new Vector2(-1,41),
                new Vector2(-6,41),
                new Vector2(-6,16),
                new Vector2(-1,16)
            };
            newEntity.AddComponent(new ComponentLineCollision(WallOuterPositions)); //Line collision takes positions where the maze wall is 
            entityManager.AddEntity(newEntity);


            newEntity = new Entity("WallTop");
            Vector2[] InnerWallTopPosition = new Vector2[]
            {
                //Positions for the top inner wall
                new Vector2(-36,31),
                new Vector2(-46,31),
                new Vector2(-46,41),
                new Vector2(-41,41),
                new Vector2(-41,46),
                new Vector2(-31,46),
                new Vector2(-31,36),
                new Vector2(-36,36)
            };
            newEntity.AddComponent(new ComponentLineCollision(InnerWallTopPosition));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("WallBottom");
            Vector2[] InnerWallBottomPosition = new Vector2[]
            {
                //Positions for the bottom inner wall 
                new Vector2(-11,16),
                new Vector2(-16,16),
                new Vector2(-16,11),
                new Vector2(-26,11),
                new Vector2(-26,21),
                new Vector2(-21,21),
                new Vector2(-21,26),
                new Vector2(-11,26)
            };
            newEntity.AddComponent(new ComponentLineCollision(InnerWallBottomPosition));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("WallLeft");
            Vector2[] InnerWallLeftPosition = {
                //Positions for the left inner wall
                new Vector2(-11f,31.0f),
                new Vector2(-21f,31.0f),
                new Vector2(-21f,36.0f),
                new Vector2(-26f,36.0f),
                new Vector2(-26f,46.0f),
                new Vector2(-16f,46.0f),
                new Vector2(-16f,41.0f),
                new Vector2(-11f,41.0f)
            };
            newEntity.AddComponent(new ComponentLineCollision(InnerWallLeftPosition));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("WallRight");
            Vector2[] InnerWallRightPosition = {
                //Positions for the right inner wall 
                new Vector2(-31f,11.0f),
                new Vector2(-41f,11.0f),
                new Vector2(-41f,16.0f),
                new Vector2(-46f,16.0f),
                new Vector2(-46f,26.0f),
                new Vector2(-36f,26.0f),
                new Vector2(-36f,21.0f),
                new Vector2(-31f,21.0f)
            };
            newEntity.AddComponent(new ComponentLineCollision(InnerWallRightPosition)); //Positions for the the wall are given to the component line collisions
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Enemy1"); //Enemy that follows the player when close 
            newEntity.AddComponent(new ComponentPosition(-28.5f, 1.0f, 28.5f));
            newEntity.AddComponent(new ComponentVelocity(0.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            Vector3[] nodes = BuildEnemyNetwork(); //Creates the positions it move betweens 
            newEntity.AddComponent(new ComponentEnemy(nodes, BuildEnemyNetworkNeighbours(nodes))); // Positions are given to component enemy 
            ComponentAudio audioComponent = new ComponentAudio("Audio/buzz.wav", -28.5f, 1.0f, 28.5f);
            newEntity.AddComponent(audioComponent);
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Patroller1");
            newEntity.AddComponent(new ComponentPosition(-28.5f, 1.0f, 28.5f));
            newEntity.AddComponent(new ComponentVelocity(0.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            nodes = BuildPatrolNetwork(1); // ID == 1 
            //IDs are given so a particular network canbe 
            newEntity.AddComponent(new ComponentEnemy(nodes, BuildPatrolNetwork1Neighbours(nodes)));
           entityManager.AddEntity(newEntity);

            newEntity = new Entity("Patroller2"); //ID is 2, could add component 
            //newEntity.AddComponent(new ComponentPosition(-3, 2, 53));
            newEntity.AddComponent(new ComponentPosition(-28.5f, 1.0f, 28.5f));
            newEntity.AddComponent(new ComponentVelocity(0.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            //audioComponent.Play();
            nodes = BuildPatrolNetwork(2); // ID == 2
            newEntity.AddComponent(new ComponentEnemy(nodes, BuildPatrolNetwork1Neighbours(nodes)));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Patroller3"); //ID is 3, could add component 
            //newEntity.AddComponent(new ComponentPosition(-3, 2, 53));
            newEntity.AddComponent(new ComponentPosition(-28.5f, 1.0f, 28.5f));
            newEntity.AddComponent(new ComponentVelocity(0.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            newEntity.AddComponent(new ComponentSphereCollision(0.5f));
            //audioComponent.Play();
            nodes = BuildPatrolNetwork(3); // ID == 3
            newEntity.AddComponent(new ComponentEnemy(nodes, BuildPatrolNetwork1Neighbours(nodes)));
            entityManager.AddEntity(newEntity);
        }

        private void GeneratePoints(Entity newEntity)
        {
            //Creates the ball points in different positions and gives them different names
            //Positions are around the edges and the centre
            for(int i = 0; i < 9; i++)
            {
                newEntity = new Entity("LowRightBall" + i);
                newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                newEntity.AddComponent(new ComponentPosition(-16.0f - (i * 3), 0.75f, 8.5f));             
                newEntity.AddComponent(new ComponentSphereCollision(0.4f));
                //ComponentAudio audioComponent = new ComponentAudio("Audio/ball.wav", -28.5f, 1.0f, 28.5f);
                //newEntity.AddComponent(audioComponent);
                newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -16.0f - (i * 3), 0.75f, 8.5f));
                entityManager.AddEntity(newEntity);

                newEntity = new Entity("HighRightBall" + i);
                newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                newEntity.AddComponent(new ComponentPosition(-48.5f, 0.75f, 16.0f + (i * 3)));              
                newEntity.AddComponent(new ComponentSphereCollision(0.4f));
                //ComponentAudio audioComponent = new ComponentAudio("Audio/ball.wav", -28.5f, 1.0f, 28.5f);
                newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -48.5f, 0.75f, 16.0f + (i * 3)));
                // newEntity.AddComponent(audioComponent);
                entityManager.AddEntity(newEntity);

                newEntity = new Entity("LowLeftBall" + i);
                newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                newEntity.AddComponent(new ComponentPosition(-8.5f, 0.75f, 16.0f + (i * 3)));              
                newEntity.AddComponent(new ComponentSphereCollision(0.4f));
               // ComponentAudio audioComponent = new ComponentAudio("Audio/ball.wav", -28.5f, 1.0f, 28.5f);
                newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -8.5f, 0.75f, 16.0f + (i * 3)));
               // newEntity.AddComponent(audioComponent);
                entityManager.AddEntity(newEntity);

                newEntity = new Entity("HighLeftBall" + i);
                newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                newEntity.AddComponent(new ComponentPosition(-16.0f - (i * 3), 0.75f, 48.5f));              
                newEntity.AddComponent(new ComponentSphereCollision(0.4f));
                newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -16.0f - (i * 3), 0.75f, 48.5f));

                entityManager.AddEntity(newEntity);

                if (i < 3)
                {
                    
                    newEntity = new Entity("MidBallLowRight" + i);
                    newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                    newEntity.AddComponent(new ComponentPosition(-25.0f - (i * 3), 0.75f, 23.0f));                  
                    newEntity.AddComponent(new ComponentSphereCollision(0.4f));
                    newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -25.0f - (i * 3), 0.75f, 23.0f));
                    entityManager.AddEntity(newEntity);

                    newEntity = new Entity("MidBallHighRight" + i);
                    newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                    newEntity.AddComponent(new ComponentPosition(-34.0f, 0.75f, 26.0f + (i * 3)));                   
                    newEntity.AddComponent(new ComponentSphereCollision(0.4f));
                    newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -34.0f, 0.75f, 26.0f + (i * 3)));
                    entityManager.AddEntity(newEntity);

                    newEntity = new Entity("MidBallLowLeft" + i);
                    newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                    newEntity.AddComponent(new ComponentPosition(-22.0f, 0.75f, 26.0f + (i * 3)));                  
                    newEntity.AddComponent(new ComponentSphereCollision(0.4f));
                    newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -22.0f, 0.75f, 26.0f + (i * 3)));
                    entityManager.AddEntity(newEntity);

                    newEntity = new Entity("MidBallHighLeft" + i);
                    newEntity.AddComponent(new ComponentGeometry("Geometry/Ball/Ball.obj"));
                    newEntity.AddComponent(new ComponentPosition(-25.0f - (i * 3), 0.75f, 35.0f));                   
                    newEntity.AddComponent(new ComponentSphereCollision(0.4f));
                    newEntity.AddComponent(new ComponentAudio("Audio/kick.wav", -25.0f - (i * 3), 0.75f, 35.0f));
                    entityManager.AddEntity(newEntity);
                }
            }
            remaining = entityManager.CalculateNumberOfPoints();
        }

        private void CreateSystems()
        {
            //Adds all the systems to the systemManager 
            ISystem newSystem;

            newSystem = new SystemRender();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemPhysics();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemAudio();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemCameraSphereCollision(collisionManager,camera);
            systemManager.AddSystem(newSystem);

            newSystem = new SystemCameraLineCollision(collisionManager,camera);
            systemManager.AddSystem(newSystem);

            newSystem = new SystemEnemyFollow(ref camera);
            systemManager.AddSystem(newSystem);

            newSystem = new SystemEnemyPatrol(ref camera);
            systemManager.AddSystem(newSystem);

            newSystem = new SystemRenderBump();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemSky(ref camera);
            systemManager.AddSystem(newSystem);
        }

        //Enemy movement positionss 
        private Vector3[] BuildEnemyNetwork()
        {
            Vector3[] nodePositions =
            {                         
                new Vector3(-8.5f,1, 16),
                new Vector3(-16f,1, 8.5f),
                new Vector3(-28.5f,1, 8.5f),
                new Vector3(-41,1, 8.5f),
                new Vector3(-48.5f,1, 16),
                new Vector3(-48.5f,1, 28.5f),
                new Vector3(-48.5f,1, 41),
                new Vector3(-41,1, 48.5f),
                new Vector3(-28.5f,1, 48.5f),
                new Vector3(-16,1, 48.5f),
                new Vector3(-8.5f,1, 41),
                new Vector3(-8.5f,1, 28.5f),
                new Vector3(-21f,1, 28.5f),
                new Vector3(-28.5f,1, 36),
                new Vector3(-36,1, 28.5f),
                new Vector3(-28.5f,1, 21),
                new Vector3(-28.5f,1, 28.5f)
        };
            return nodePositions;
        }
        //
        private Dictionary<Vector3, Vector3[]> BuildEnemyNetworkNeighbours(Vector3[] nodePositions) 
        {
            Dictionary<Vector3, Vector3[]> neighbours = new Dictionary<Vector3, Vector3[]>();          
            List<Vector3> nodeNeighbours = new List<Vector3>();

            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 1]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - 6]);
            neighbours.Add(nodePositions[0], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[0]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 2]);
            neighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 1], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 1]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 2]);
            nodeNeighbours.Add(nodePositions[15]);
            neighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 2], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 2]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 4]);
            neighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 3], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 3]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 5]);
            neighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 4], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 3]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - nodePositions.Length + 6]);
            nodeNeighbours.Add(nodePositions[14]);
            neighbours.Add(nodePositions[5], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[5]);
            nodeNeighbours.Add(nodePositions[7]);
            neighbours.Add(nodePositions[6], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[6]);
            nodeNeighbours.Add(nodePositions[8]);
            neighbours.Add(nodePositions[7], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[7]);
            nodeNeighbours.Add(nodePositions[9]);
            nodeNeighbours.Add(nodePositions[13]);
            neighbours.Add(nodePositions[8], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[8]);
            nodeNeighbours.Add(nodePositions[10]);
            neighbours.Add(nodePositions[9], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[9]);
            nodeNeighbours.Add(nodePositions[11]);
            neighbours.Add(nodePositions[10], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[0]);
            nodeNeighbours.Add(nodePositions[10]);
            nodeNeighbours.Add(nodePositions[12]);
            neighbours.Add(nodePositions[11], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[11]);
            nodeNeighbours.Add(nodePositions[13]);
            nodeNeighbours.Add(nodePositions[14]);
            nodeNeighbours.Add(nodePositions[15]);
            neighbours.Add(nodePositions[12], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[8]);
            nodeNeighbours.Add(nodePositions[12]);
            nodeNeighbours.Add(nodePositions[14]);
            nodeNeighbours.Add(nodePositions[15]);
            neighbours.Add(nodePositions[13], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[5]);
            nodeNeighbours.Add(nodePositions[12]);
            nodeNeighbours.Add(nodePositions[13]);
            nodeNeighbours.Add(nodePositions[15]);
            neighbours.Add(nodePositions[14], nodeNeighbours.ToArray());
            
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[2]);
            nodeNeighbours.Add(nodePositions[12]);
            nodeNeighbours.Add(nodePositions[13]);
            nodeNeighbours.Add(nodePositions[14]);
            neighbours.Add(nodePositions[15], nodeNeighbours.ToArray());
         
            nodeNeighbours = new List<Vector3>();
            nodeNeighbours.Add(nodePositions[nodePositions.Length - 5]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - 4]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - 3]);
            nodeNeighbours.Add(nodePositions[nodePositions.Length - 2]);
            neighbours.Add(nodePositions[nodePositions.Length - 1], nodeNeighbours.ToArray());

            return neighbours;
        }

        private Vector3[] BuildPatrolNetwork(int ID)
        {
            Vector3[] nodePositions = null;
            if (ID == 1)
            {
                Vector3[] positions =
                {
                new Vector3(-28.5f, 1, 38.5f),
                new Vector3(-28.5f, 1, 48.0f),
                new Vector3(-4f, 1, 48.0f),
                new Vector3(-3, 1, 43),
                new Vector3(-13f, 1, 43),
                new Vector3(-13, 1, 53),
                new Vector3(-3, 1, 53),
                new Vector3(-9f, 1.0f, 43.0f),
                new Vector3(-9f, 1.0f, 41.5f),
                new Vector3(-9f, 1.0f, 28.5f),
                new Vector3(-28.5f, 1, 28.5f)
                };
                nodePositions = positions;
                return nodePositions;
            }
            if(ID == 2)
            {
                Vector3[] positions =
                {
              
                new Vector3(-34.5f, 1f, 28.5f),
                new Vector3(-48.5f, 1f, 28.5f),
                new Vector3(-48.5f, 1f, 13.5f),
                new Vector3(-53.5f, 1f, 13.5f),
                new Vector3(-53.5f, 1f, 4.5f),
                new Vector3(-43.5f, 1f, 4.5f),
                new Vector3(-43.5f, 1f, 8.5f),
                new Vector3(-28.5f, 1f, 8.5f),
                new Vector3(-28.5f,1f,28.5f)
                };
                nodePositions = positions;
                return nodePositions;
            }
            if(ID == 3)
            {
                Vector3[] positions =
                {
                    //new Vector3(-28.5f, 1f, 8.5f),
                    new Vector3(-28.5f, 1f, 8.5f),
                    new Vector3(-8.5f, 1f, 8.5f),
                    new Vector3(-8.5f, 1f, 48.5f),
                    new Vector3(-48.5f, 1f, 48.5f),
                    new Vector3(-48.5f, 1f, 8.5f),
                    new Vector3(-28.4f, 1f, 8.5f),
                    new Vector3(-28.5f,1f,28.5f)
                };
                nodePositions = positions;
                return nodePositions;
            }
            else
            {
                System.Console.WriteLine("No ID given");
                return nodePositions;
            }
           
        }

        private Dictionary<Vector3, Vector3[]> BuildPatrolNetwork1Neighbours(Vector3[] nodePositions) //unique to the game, so some set values for declarations are used
        {
            Dictionary<Vector3, Vector3[]> neighbours = new Dictionary<Vector3, Vector3[]>();

            for (int i = 0; i < nodePositions.Length; i++)
            {
                if (i + 1 < nodePositions.Length)//checking to see if at last index
                {
                    List<Vector3> nodeNeighbours = new List<Vector3>();
                    nodeNeighbours.Add(nodePositions[i + 1]); //want it to go in a loop, so will just go to the next node
                    neighbours.Add(nodePositions[i], nodeNeighbours.ToArray());
                }
                else
                {
                    List<Vector3> nodeNeighbours = new List<Vector3>();
                    nodeNeighbours.Add(nodePositions[0]); //want it to go in a loop, so will just go to the next node
                    neighbours.Add(nodePositions[i], nodeNeighbours.ToArray());
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Update(FrameEventArgs e)
        {
            dt = (float)e.Time;
            System.Console.WriteLine("fps=" + (int)(1.0 / dt));
            // NEW for Audio
            // Update OpenAL Listener Position and Orientation based on the camera
            AL.Listener(ALListener3f.Position, ref camera.cameraPosition);
            AL.Listener(ALListenerfv.Orientation, ref camera.cameraDirection, ref camera.cameraUp);
            
            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();

            time += dt;
            Console.WriteLine(time);
            
            if(PowerUpActive == true)
            {                
                if (time >= collisionManager.startTime + PowerUpTimeLimit)
                {                    
                    PowerUpActive = false;                   
                }
            }
            
            if(remaining == 0)
            {
                sceneManager.ChangeScene(SceneTypes.SCENE_GAME_WIN);
            }

            if(lives == 0)
            {
                sceneManager.ChangeScene(SceneTypes.SCENE_GAME_OVER);
            }

           
            collisionManager.ProcessCollisions();        
            inputManager.ResolveInputs();
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);          

            // Render score
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.clearColour = Color.Transparent;
            GUI.Label(new Rectangle((int)width - (int) width, (int)height - (int)height + 10, (int)width, (int)(fontSize)), "Score: " + score * 1000, 18, StringAlignment.Near, Color.White);
            GUI.Label(new Rectangle((int)width - (int)width/8, (int)height - (int)height + 10, (int)width, (int)(fontSize)), "Lives: " + lives.ToString(), 18, StringAlignment.Near, Color.White);
            GUI.Label(new Rectangle((int)width - (int)width / 8 * 3, (int)height - (int)height + 10, (int)width, (int)(fontSize)), "PowerUp: " + PowerUpActive.ToString(), 18, StringAlignment.Near, Color.White);
            
            if (PowerUpActive == true)
            {
                GUI.Label(new Rectangle((int)width - (int)width + 150, (int)height - (int) height + 10, (int)width, (int)(fontSize)), "X2", 18, StringAlignment.Near, Color.Green);
            }

            GUI.Label(new Rectangle(0, (int)height - 30, (int)width, (int)(fontSize)), "Movement(1): ", 11, StringAlignment.Near, Color.White);
            if (movement == true)
            {
                GUI.Label(new Rectangle(95, (int)height - 30, (int)width, (int)(fontSize)), movement.ToString(), 11, StringAlignment.Near, Color.Green);
            }
            else
            {
                GUI.Label(new Rectangle(95, (int)height - 30, (int)width, (int)(fontSize)), movement.ToString(), 11, StringAlignment.Near, Color.Red);
            }

            GUI.Label(new Rectangle(200, (int)height - 30, (int)width, (int)(fontSize)), "WallCollidable(2): ", 11, StringAlignment.Near, Color.White);
            if(collidable == true)
            {
                GUI.Label(new Rectangle(320, (int)height - 30, (int)width, (int)(fontSize)), collidable.ToString(), 11, StringAlignment.Near, Color.Green);
            }
            else 
            {
                GUI.Label(new Rectangle(320, (int)height - 30, (int)width, (int)(fontSize)), collidable.ToString(), 11, StringAlignment.Near, Color.Red);            
            }
            systemManager.ActionSystems(entityManager, movement, collidable);
            GUI.Render();
        }

        public void ResetOneEnemy(string EntityName)
        {
            for (int i = 0; i < entityManager.Entities().Count; i++)
            {
                if (entityManager.Entities()[i].Name == EntityName)
                {
                    List<IComponent> components = entityManager.Entities()[i].Components;
                    IComponent positionComponent = components.Find(delegate (IComponent component)
                    {
                        return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                    });
                    ((ComponentPosition)positionComponent).Position = new Vector3(-28.5f, 1.0f, 28.5f);

                    IComponent EnemyComponent = components.Find(delegate (IComponent component)
                    {
                        return component.ComponentType == ComponentTypes.COMPONENT_ENEMY;
                    });

                    ((ComponentEnemy)EnemyComponent).Destination = new Vector3(-28.5f, 1.0f, 28.5f);
                }
            }
        }

        public void ResetEnemies()
        {
            for (int i = 0; i < entityManager.Entities().Count; i++)
            {
                if (entityManager.Entities()[i].Name.Contains("Enemy") || entityManager.Entities()[i].Name.Contains("Patroller"))
                {
                    List<IComponent> components = entityManager.Entities()[i].Components;
                    IComponent positionComponent = components.Find(delegate (IComponent component)
                    {
                        return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                    });
                    ((ComponentPosition)positionComponent).Position = new Vector3(-28.5f, 1.0f, 28.5f);

                    IComponent EnemyComponent = components.Find(delegate (IComponent component)
                    {
                        return component.ComponentType == ComponentTypes.COMPONENT_ENEMY;
                    });

                    ((ComponentEnemy)EnemyComponent).Destination = new Vector3(-28.5f, 1.0f, 28.5f);
                }
            }
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            sceneManager.keyboardUpDelegate -= Keyboard_KeyUp;
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;         
            ResourceManager.RemoveAllAssets();
            entityManager.Close();
        }

        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            
            inputManager.AddToCurrentInputs((char)e.Key);
            movement = inputManager.movement;
        }
        public void Keyboard_KeyUp(KeyboardKeyEventArgs e)
        {
            //movement = inputManager.movement;
            inputManager.RemoveFromCurrentInputs((char)e.Key);
        }
    }
}
