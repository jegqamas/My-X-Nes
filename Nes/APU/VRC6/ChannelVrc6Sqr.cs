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

    public class ChannelVrc6Sqr : NesApuChannel
    {
        bool dutySkip = false;
        bool enabled = false;
        bool waveStatus = false;
        int dutyForm = 0;
        int volume = 0;
        double dutyPercentage = 0;

        public ChannelVrc6Sqr(NES nes)
            : base(nes)
        {
        }

        void UpdateFrequency()
        {
            frequency = 1789772.72 / 16 / (freqTimer + 1);
            sampleDelay = 44100.00 / frequency;
        }

        public override void ClockHalf()
        {
        }
        public override void ClockQuad()
        {
        }
        public override int RenderSample()
        {
            if (enabled)
            {
                sampleTimer++;

                if (waveStatus && (sampleTimer > (sampleDelay * dutyPercentage)))
                {
                    sampleTimer -= sampleDelay * dutyPercentage;
                    waveStatus = !waveStatus;
                }
                else if (!waveStatus && (sampleTimer > (sampleDelay * (1.0 - dutyPercentage))))
                {
                    sampleTimer -= sampleDelay * (1.0 - dutyPercentage);
                    waveStatus = !waveStatus;
                }

                if (waveStatus)
                    return -volume;
                else
                    return volume;
            }

            return 0;
        }
        public override void Poke1(int addr, byte data)
        {
            volume = (data & 0x0F);
            dutyForm = (data & 0x70) >> 4;
            dutySkip = (data & 0x80) != 0;

            if (dutySkip)
                dutyForm = 0x0F;

            dutyPercentage = (dutyForm + 1) / 16.0;
        }
        public override void Poke2(int addr, byte data)
        {
            freqTimer = (freqTimer & 0x0F00) | ((data & 0xFF) << 0);
            UpdateFrequency();
        }
        public override void Poke3(int addr, byte data)
        {
            freqTimer = (freqTimer & 0x00FF) | ((data & 0x0F) << 8);
            UpdateFrequency();

            enabled = (data & 0x80) != 0;
        }
        public override void Poke4(int addr, byte data)
        {
        }

        public override void SaveState(System.IO.Stream stream)
        {
            base.SaveState(stream);
            byte status = 0;
            if (dutySkip)
                status |= 0x01;
            if (enabled)
                status |= 0x02;
            if (waveStatus)
                status |= 0x04;
            stream.WriteByte(status);

            stream.WriteByte((byte)((dutyForm & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dutyForm & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dutyForm & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dutyForm & 0x000000FF)));

            stream.WriteByte((byte)((volume & 0xFF000000) >> 24));
            stream.WriteByte((byte)((volume & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((volume & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((volume & 0x000000FF)));
        }
        public override void LoadState(System.IO.Stream stream)
        {
            base.LoadState(stream);
            byte status = (byte)stream.ReadByte();
            dutySkip = ((status & 0x01) == 0x01);
            enabled = ((status & 0x02) == 0x02);
            waveStatus = ((status & 0x04) == 0x04);

            dutyForm = (int)(stream.ReadByte() << 24);
            dutyForm |= (int)(stream.ReadByte() << 16);
            dutyForm |= (int)(stream.ReadByte() << 8);
            dutyForm |= stream.ReadByte();

            volume = (int)(stream.ReadByte() << 24);
            volume |= (int)(stream.ReadByte() << 16);
            volume |= (int)(stream.ReadByte() << 8);
            volume |= stream.ReadByte();

            dutyPercentage = (dutyForm + 1) / 16.0;
        }
    }
}