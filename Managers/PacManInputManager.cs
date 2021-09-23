using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Scenes;
using OpenTK.Input;
namespace OpenGL_Game.Managers
{
    class PacManInputManager : InputManager
    {
        public bool movement = true;
        protected Camera mCamera;
        protected bool[] mKeysPressed;
        public PacManInputManager(ref Camera pCamera)
        {
          mCamera = pCamera;
          mKeysPressed = new bool[255];
        }

        public override void ResolveInputs()
        {
            if (mKeysPressed[(char)Key.Up] || mKeysPressed[(char)Key.W])
            {
                mCamera.MoveForward(0.1f);
            }
            if (mKeysPressed[(char)Key.Down] || mKeysPressed[(char)Key.S])
            {
                mCamera.MoveForward(-0.1f);
            }
            if (mKeysPressed[(char)Key.Left] || mKeysPressed[(char)Key.A])
            {
                mCamera.RotateY(-0.05f);
            }
            if (mKeysPressed[(char)Key.Right] || mKeysPressed[(char)Key.D])
            {
                mCamera.RotateY(0.05f);
            }
            if (mKeysPressed[(char)Key.Number1])
            {
                movement = !movement; //Inconsisent, might have to press a few times to work
                //GameScene.movement = !GameScene.movement; //Might have to press a few times to work
            }
            if (mKeysPressed[(char)Key.Number2])
            {
                GameScene.collidable = !GameScene.collidable; //Might have to press a few times to work
            }
            //GameScene.movement = movement;
        }

        

        public void AddToCurrentInputs(char e)
        {
            mKeysPressed[e] = true;
        }

        public void RemoveFromCurrentInputs(char e)
        {
            mKeysPressed[e] = false;
        }
    }
}
