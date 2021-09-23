using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenTK;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace OpenGL_Game.Components
{
    class ComponentAudio : IComponent
    {
        Vector3 sourcePosition;
        int audioSource;
        int audio;
        bool playing;

        public ComponentAudio(string audioName, Vector3 pSourcePosition)
        {
            playing = false;
            audioSource = AL.GenSource();
            audio = ResourceManager.LoadAudio(audioName);
            SetPosition(pSourcePosition);
            //AL.Source(audioSource, ALSourcei.Buffer, audioBuffer); // attach the buffer to a source
            // AL.Source(audioSource, ALSourceb.Looping, true); // source loops infinitely
            // sourcePosition = new Vector3(10.0f, 0.0f, 0.0f); // give the source a position
            // AL.Source(audioSource, ALSource3f.Position, ref sourcePosition);
            // play the ausio source
            //Start();
        }

        public ComponentAudio(string audioName, float x, float y, float z)
        {
            playing = false;
            audioSource = AL.GenSource();
            audio = ResourceManager.LoadAudio(audioName);
            SetPosition(new Vector3(x,y,z));         
        }

       

        public int Audio {
            get { return audio; }
        }

        public void LoadNewAudio(string audioName)
        {
            audioSource = AL.GenSource();
            audio = ResourceManager.LoadAudio(audioName);
            SetPosition(sourcePosition);
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }

        public void SetPosition(Vector3 emitterPosition)
        {
            sourcePosition = emitterPosition;
            AL.Source(audioSource, ALSource3f.Position, ref sourcePosition);
        }
        public void Play()
        {
            if(!playing)
            {
                AL.Source(audioSource, ALSourcei.Buffer, audio);
                AL.Source(audioSource, ALSourceb.Looping, true);


                AL.SourcePlay(audioSource);
                playing = true;
            }
            
        }
        public void NonLooping()
        {
            AL.Source(audioSource, ALSourcei.Buffer, audio);
            AL.SourcePlay(audioSource); 
        }

        public void Stop()
        {
            playing = false;
            AL.SourcePause(audioSource);
        }
        public void Close()
        {
            // NEW for Audio      
            AL.SourceStop(audioSource);
            AL.DeleteSource(audioSource);
        }

    }
}
