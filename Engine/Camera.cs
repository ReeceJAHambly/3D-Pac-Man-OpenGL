using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class Camera
    {
        public Matrix4 view, projection;
        public Vector3 cameraPosition, cameraDirection, cameraUp, previousCameraPosition;
        private Vector3 targetPosition;
        public float radius = 1;

        public Camera()
        {
            cameraPosition = new Vector3(0.0f, 0.0f, 0.0f);
            cameraDirection = new Vector3(0.0f, 0.0f, -1.0f);
            cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            previousCameraPosition = cameraPosition;
            UpdateView();
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 1.0f, 0.1f, 100f);
        }

        public Camera(Vector3 cameraPos, Vector3 targetPos, float ratio, float near, float far)
        {
            cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            cameraPosition = cameraPos;
            cameraDirection = targetPos-cameraPos;
            cameraDirection.Normalize();
            UpdateView();
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), ratio, near, far);
        }

        public void MoveForward(float move)
        {
            previousCameraPosition = cameraPosition;
           // previousCameraPosition += move * cameraPosition;
            cameraPosition += move * cameraDirection;         
            UpdateView();
        }

        public void Translate(Vector3 move)
        {
            previousCameraPosition = cameraPosition;

            cameraPosition += move;        
            UpdateView();
        }


        public void RotateY(float angle)
        {
            cameraDirection = Matrix3.CreateRotationY(angle) * cameraDirection;
            UpdateView();
        }
        
        public float GetRadius()
        {
            return radius;
        }

        public void UpdateView()
        {
            targetPosition = cameraPosition + cameraDirection;
            view = Matrix4.LookAt(cameraPosition, targetPosition, cameraUp);
        }
    }
}
