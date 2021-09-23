using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.Components;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Objects;
using System.IO;

namespace OpenGL_Game.Systems
{
    class SystemRenderBump : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION |
            ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_BUMP_RENDER);

        protected int pgmID;
        protected int vsID;
        protected int fsID;
        protected int uniform_stex;
        
        protected int uniform_mmodelviewproj;
        protected int uniform_modelview;
        protected int uniform_mmodel;

        protected int uniform_bumptex;
        protected int uniform_displacetex;
        //protected int uniform_diffuse;

        public SystemRenderBump()
        {
            pgmID = GL.CreateProgram();
            LoadShader("Shaders/vsBump.glsl", ShaderType.VertexShader, pgmID, out vsID);
            LoadShader("Shaders/fsBump.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            uniform_stex = GL.GetUniformLocation(pgmID, "s_texture");
            uniform_bumptex = GL.GetUniformLocation(pgmID, "s_bumpMap");
            uniform_displacetex = GL.GetUniformLocation(pgmID, "s_heightMap");
            uniform_mmodelviewproj = GL.GetUniformLocation(pgmID, "ModelViewProjMat");
            uniform_mmodel = GL.GetUniformLocation(pgmID, "ModelMat");
            uniform_modelview = GL.GetUniformLocation(pgmID, "ModelView");
            //uniform_diffuse = GL.GetUniformLocation(pgmID, "v_diffuse");
        }

        public string Name
        {
            get { return "SystemRenderBump"; }
        }


        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK) 
            {
                List<IComponent> components = entity.Components;

                IComponent geometryComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_GEOMETRY;
                });
                Geometry geometry = ((ComponentGeometry)geometryComponent).Geometry();

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });
                Vector3 position = ((ComponentPosition)positionComponent).Position;
                Matrix4 model = Matrix4.CreateTranslation(position);

                Draw(model, geometry);
            }
        }


        public void Draw(Matrix4 model, Geometry geometry)
        {
            GL.UseProgram(pgmID);

            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.UniformMatrix4(uniform_mmodel, false, ref model);
            Matrix4 modelView = model; //* GameScene.gameInstance.camera.view; Wanted in engine 
            Matrix4 modelViewProjection = model; //* GameScene.gameInstance.camera.view * GameScene.gameInstance.camera.projection; // Wanted in engine
            GL.UniformMatrix4(uniform_mmodelviewproj, false, ref modelViewProjection);
            GL.UniformMatrix4(uniform_modelview, false, ref modelView);


            geometry.Render();

            GL.UseProgram(0);
        }

        void LoadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }
    }
}
