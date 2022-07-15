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
namespace MyNes.Nes
{
   
    public class Vrc6ExternalComponent : NesApuExternalComponent
    {
        public ChannelVrc6Saw ChannelSaw;
        public ChannelVrc6Sqr ChannelSq1;
        public ChannelVrc6Sqr ChannelSq2;
        
        public bool EnableSaw = true;
        
        public bool EnableSq1 = true;
        
        public bool EnableSq2 = true;
        public Vrc6ExternalComponent(NES nes)
        {
            ChannelSaw = new ChannelVrc6Saw(nes);
            ChannelSq1 = new ChannelVrc6Sqr(nes);
            ChannelSq2 = new ChannelVrc6Sqr(nes);
        }
        public override void ClockHalf()
        {
        }
        public override void ClockQuad()
        {
        }
        public override int RenderSample(float rate)
        {
            int sample = 0;
            if (EnableSaw)
                sample += ChannelSaw.RenderSample();
            if (EnableSq1)
                sample += ChannelSq1.RenderSample();
            if (EnableSq2)
                sample += ChannelSq2.RenderSample();

            return sample;
        }

        public override void SaveState(System.IO.Stream stream)
        {
            ChannelSaw.SaveState(stream);
            ChannelSq1.SaveState(stream);
            ChannelSq2.SaveState(stream);
        }
        public override void LoadState(System.IO.Stream stream)
        {
            ChannelSaw.LoadState(stream);
            ChannelSq1.LoadState(stream);
            ChannelSq2.LoadState(stream);
        }
    }
}