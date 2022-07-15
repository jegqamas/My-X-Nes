/*********************************************************************\
*This file is part of My Nes                                          *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright © Ala Hadid 2009 - 2011                                    *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My Nes is free software: you can redistribute it and/or modify       *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My Nes is distributed in the hope that it will be useful,            *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with this program.  If not, see <http://www.gnu.org/licenses/>.*
\*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNes.Nes.Output.Audio
{
    public interface IAudioDevice
    {
        /// <summary>
        /// A pointer to the handle of the window that has requested sound. Refer to constructor on how to retrieve it for your application.
        /// </summary>
        IntPtr Handle { get; }
        /// <summary>
        /// Update the sound buffer
        /// </summary>
        void UpdateBuffer();
        /// <summary>
        /// Play the sound, resume if paused
        /// </summary>
        void Play();
        /// <summary>
        /// Pause the sound
        /// </summary>
        void Pause();
        /// <summary>
        /// Shutdown the sound device
        /// </summary>
        void Shutdown();
        /// <summary>
        /// Set the volume 
        /// </summary>
        /// <param name="Vol">The volume level (-3000 = min, 0 = max)</param>
        void SetVolume(int Vol);
        /// <summary>
        /// Set the pan
        /// </summary>
        /// <param name="Pan">The pan</param>
        void SetPan(int Pan);
        /// <summary>
        /// Initialize the sound device
        /// </summary>
        void Initialize();
        /// <summary>
        /// Get or set the nes apu, nedded for buffer writing
        /// </summary>
        NesApu APU { get; set; }
        /// <summary>
        /// Get the wave recorder
        /// </summary>
        IWaveRecorder Recorder { get; }
        /// <summary>
        /// Get the current volume level
        /// </summary>
        int Volume { get; }
    }
}