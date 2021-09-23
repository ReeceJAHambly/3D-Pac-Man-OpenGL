using OpenTK;

namespace OpenGL_Game.Scenes
{
    enum SceneTypes
    {
        SCENE_NONE,
        SCENE_MAIN_MENU,
        SCENE_GAME,
        SCENE_GAME_OVER,
        SCENE_GAME_WIN
    }
    interface IScene
    {
        void Render(FrameEventArgs e);
        void Update(FrameEventArgs e);
        void Close();
    }
}
