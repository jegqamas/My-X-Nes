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
   
    public class SS5BExternalUnit : NesApuExternalComponent
    {
        public ChannelSS5BWave Wave1;
        public ChannelSS5BWave Wave2;
        public ChannelSS5BWave Wave3;


        public bool Wave1Enabled = true;
        public bool Wave2Enabled = true;
        public bool Wave3Enabled = true;

        public SS5BExternalUnit(NES nes)
        {
            Wave1 = new ChannelSS5BWave(nes);
            Wave2 = new ChannelSS5BWave(nes);
            Wave3 = new ChannelSS5BWave(nes);
        }

        public override void ClockHalf() { }
        public override void ClockQuad() { }
        public override int RenderSample(float rate)
        {
            int sample = 0;

            if (this.Wave1Enabled) sample += this.Wave1.RenderSample(rate);
            if (this.Wave2Enabled) sample += this.Wave2.RenderSample(rate);
            if (this.Wave3Enabled) sample += this.Wave3.RenderSample(rate);

            return sample;
        }
        public override void SaveState(System.IO.Stream stream)
        {
            Wave1.SaveState(stream);
            Wave2.SaveState(stream);
            Wave3.SaveState(stream);
        }
        public override void LoadState(System.IO.Stream stream)
        {
            Wave1.LoadState(stream);
            Wave2.LoadState(stream);
            Wave3.LoadState(stream);
        }
    }
}
