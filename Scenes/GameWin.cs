using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Scenes
{
    class GameWinScene : Scene
    {
        
        public GameWinScene(SceneManager sceneManager) : base(sceneManager)
        {
            
            // Set the title of the window
            sceneManager.Title = "GAME OVER";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

            sceneManager.mouseDelegate += Mouse_BottonPressed;
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;
        }

        public override void Update(FrameEventArgs e)
        {
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.Black;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
           
            GUI.Label(new Rectangle((int)width / 2 - 220 , (int)(fontSize / 2f) + 150, (int)width * 2, (int)(fontSize * 2f)), "Game Complete", (int)fontSize, StringAlignment.Near, Color.Green);
            //GUI.Label(new Rectangle((int)width / 2 - 220, (int)(fontSize / 2f) + 150, (int)width * 2, (int)(fontSize * 2f)), sceneManager.score, (int)fontSize, StringAlignment.Near, Color.Green);

            GUI.Label(new Rectangle((int) width /2 - 70, (int)height/2 + 20, (int)width, (int)(fontSize)), "Click for Main Menu", 11, StringAlignment.Near, Color.White);
            GUI.Render();
        }

        public void Mouse_BottonPressed(MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    sceneManager.ChangeScene(SceneTypes.SCENE_MAIN_MENU);
                    break;
            }
        }
        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
           
        }
        public override void Close()
        {
            sceneManager.mouseDelegate -= Mouse_BottonPressed;
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;
        }
    }
}