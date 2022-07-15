/*********************************************************************\
*This file is part of My X Nes                                        *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2010 - 2011 Ala Hadid                                  *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My X Nes is free software: you can redistribute it and/or modify     *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My X Nes is distributed in the hope that it will be useful,          *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with My X Nes.  If not, see <http://www.gnu.org/licenses/>.    *
\*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyNes.Nes;
using MyNes.Nes.Output.Audio;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;

namespace MyXNes
{
    class Sound_XNA : IAudioDevice
    {
        DynamicSoundEffectInstance SoundEngine;
        NesApu apu;
        bool pause = false;
        DateTime prevTime;
        byte[] buff = new byte[0];

        public Sound_XNA(NesApu apu)
        { this.apu = apu;  }
        public NesApu APU
        {
            get
            {
                return apu;
            }
            set
            {
                apu = value;
            }
        }

        public IntPtr Handle
        {
            get { return IntPtr.Zero; }
        }

        public void Initialize()
        {
            SoundEngine = new DynamicSoundEffectInstance(44100, AudioChannels.Mono);
            SoundEngine.BufferNeeded += new EventHandler<EventArgs>(SoundEngine_BufferNeeded);
            prevTime = DateTime.Now;
            SoundEngine.Play();
        }

        void SoundEngine_BufferNeeded(object sender, EventArgs e)
        {
            //If the sound engine need buffer, give the smame buffer to it again !
            //otherwise will make bad noises
            if (pause)
                return;
            if (buff.Length != 0)
            {
                SoundEngine.SubmitBuffer(buff);
            }
        }

        public void UpdateBuffer()
        {
            if (pause)
                return;
            int count = SoundEngine.GetSampleSizeInBytes(DateTime.Now - prevTime);
            prevTime = DateTime.Now;

            buff = new byte[count];
            for (int i = 0; i < buff.Length; i++)
            {
                int sample = apu.PullSample();
                buff[i] = (byte)((sample & 0xFF00) >> 8); i++;
                buff[i] = (byte)((sample & 0xFF));
            }

            if (buff.Length != 0)
            {
                SoundEngine.SubmitBuffer(buff);
            }
        }

        public bool IsStereo
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public void Pause()
        {
            if (!pause)
                SoundEngine.Stop();
            pause = true;
        }

        public void Play()
        {
            if (pause)
            {
                SoundEngine.Resume();
                prevTime = DateTime.Now;
            }
            pause = false;
        }

        public IWaveRecorder Recorder
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public void Shutdown()
        {
            SoundEngine.Stop();
            SoundEngine.Dispose();
        }

        public void SetVolume(int Vol)
        {

        }
        public void SetVolume(float Vol)
        {
            SoundEngine.Volume = Vol;
        }

        public int Volume
        {
            get { return 0; }
        }

        public void SetPan(int Pan)
        {
      
        }
    }
}
