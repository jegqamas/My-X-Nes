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

    public class Mmc5ExternalComponent : NesApuExternalComponent
    {
        public ChannelMmc5Pcm ChannelPcm;
        public ChannelMmc5Sqr ChannelSq1;
        public ChannelMmc5Sqr ChannelSq2;

        public bool EnabledSq1 = true;
        public bool EnabledSq2 = true;
        public bool EnabledPcm = true;

        public Mmc5ExternalComponent(NES nes)
        {
            ChannelPcm = new ChannelMmc5Pcm(nes);
            ChannelSq1 = new ChannelMmc5Sqr(nes);
            ChannelSq2 = new ChannelMmc5Sqr(nes);
        }
        public override void ClockHalf()
        {
            ChannelSq1.ClockHalf();
            ChannelSq2.ClockHalf();
            ChannelPcm.ClockHalf();
        }
        public override void ClockQuad()
        {
            ChannelSq1.ClockQuad();
            ChannelSq2.ClockQuad();
            ChannelPcm.ClockQuad();
        }
        public override int RenderSample(float rate)
        {
            int sample = 0;
            if (EnabledPcm)
                sample += ChannelPcm.RenderSample();
            if (EnabledSq1)
                sample += ChannelSq1.RenderSample();
            if (EnabledSq2)
                sample += ChannelSq2.RenderSample();

            return sample;
        }

        public override void SaveState(System.IO.Stream stream)
        {
            ChannelPcm.SaveState(stream);
            ChannelSq1.SaveState(stream);
            ChannelSq2.SaveState(stream);
        }
        public override void LoadState(System.IO.Stream stream)
        {
            ChannelPcm.LoadState(stream);
            ChannelSq1.LoadState(stream);
            ChannelSq2.LoadState(stream);
        }
    }
}