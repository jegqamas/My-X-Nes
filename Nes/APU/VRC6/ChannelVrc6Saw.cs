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
   
    public class ChannelVrc6Saw : NesApuChannel
    {
        bool enabled = false;
        int accum = 0;
        int accumRate = 0;
        int accumStep = 0;

        public ChannelVrc6Saw(NES nes)
            : base(nes)
        {
        }

        void UpdateFrequency()
        {
            frequency = 1789772.72 / (freqTimer + 1);
            sampleDelay = 44100.00 / (frequency);
        }

        public override void ClockHalf()
        {
        }
        public override void ClockQuad()
        {
        }
        public override void Poke1(int addr, byte data)
        {
            accumRate = (byte)(data & 0x3F);
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
        public override int RenderSample()
        {
            if (enabled)
            {
                sampleTimer++;

                if (sampleTimer >= sampleDelay)
                {
                    sampleTimer -= sampleDelay;

                    switch (++accumStep)
                    {
                        case 2:
                        case 4:
                        case 6:
                        case 8:
                        case 10:
                        case 12:
                            accum += accumRate;
                            break;

                        case 14:
                            accum = 0;
                            accumStep = 0;
                            break;
                    }
                }

                return (accum >> 3) & 0x1F;
            }

            return 0;
        }

        public override void SaveState(System.IO.Stream stream)
        {
            base.SaveState(stream);
            stream.WriteByte((byte)(enabled ? 1 : 0));
            stream.WriteByte((byte)((accum & 0xFF000000) >> 24));
            stream.WriteByte((byte)((accum & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((accum & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((accum & 0x000000FF)));

            stream.WriteByte((byte)((accumRate & 0xFF000000) >> 24));
            stream.WriteByte((byte)((accumRate & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((accumRate & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((accumRate & 0x000000FF)));

            stream.WriteByte((byte)((accumStep & 0xFF000000) >> 24));
            stream.WriteByte((byte)((accumStep & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((accumStep & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((accumStep & 0x000000FF)));
        }
        public override void LoadState(System.IO.Stream stream)
        {
            base.LoadState(stream);
            enabled = stream.ReadByte() == 1;
            accum = (int)(stream.ReadByte() << 24);
            accum |= (int)(stream.ReadByte() << 16);
            accum |= (int)(stream.ReadByte() << 8);
            accum |= stream.ReadByte();

            accumRate = (int)(stream.ReadByte() << 24);
            accumRate |= (int)(stream.ReadByte() << 16);
            accumRate |= (int)(stream.ReadByte() << 8);
            accumRate |= stream.ReadByte();

            accumStep = (int)(stream.ReadByte() << 24);
            accumStep |= (int)(stream.ReadByte() << 16);
            accumStep |= (int)(stream.ReadByte() << 8);
            accumStep |= stream.ReadByte();
        }
    }
}