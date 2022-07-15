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

    public class ChannelDpm : NesApuChannel
    {
        private static readonly int[] FrequencyEu = 
        { 
            0x018E, 0x0161, 0x013C, 0x0129, 0x010A, 0x00EC, 0x00D2, 0x00C7,
            0x00B1, 0x0095, 0x0084, 0x0077, 0x0062, 0x004E, 0x0043, 0x0032,
        };
        private static readonly int[] FrequencyUs = 
        { 
            0x01AC, 0x017C, 0x0154, 0x0140, 0x011E, 0x00FE, 0x00E2, 0x00D6,
            0x00BE, 0x00A0, 0x008E, 0x0080, 0x006A, 0x0054, 0x0048, 0x0036,
        };

        bool dmaEnabled = false;
        bool dmaLooping = false;
        int dmaAddr = 0;
        int dmaAddrRefresh = 0;
        int dmaBits = 0;
        int dmaByte = 0;
        int dmaSize = 0;
        int dmaSizeRefresh = 0;
        int output = 0;
        public bool IsPAL = false;
        public bool IrqEnabled = false;

        public bool Enabled
        {
            get
            {
                return dmaSize != 0;
            }
            set
            {
                if (dmaEnabled = value)
                {
                    if (dmaSize <= 0)
                    {
                        dmaSize = dmaSizeRefresh;
                        dmaBits = 7;
                        dmaAddr = dmaAddrRefresh;
                    }
                }
                else
                {
                    dmaSize = 0;
                }
            }
        }

        public ChannelDpm(NES nes)
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
            throw new NotImplementedException();
        }
        public override void ClockQuad()
        {
            throw new NotImplementedException();
        }
        public override void Poke1(int addr, byte data)
        {
            IrqEnabled = (data & 0x80) != 0;
            dmaLooping = (data & 0x40) != 0;

            this.nes.APU.DeltaIrqPending &= IrqEnabled;

            freqTimer = IsPAL ? FrequencyEu[data & 0x0F] : FrequencyUs[data & 0x0F];
            UpdateFrequency();
        }
        public override void Poke2(int addr, byte data)
        {
            output = (data & 0x7F);
        }
        public override void Poke3(int addr, byte data)
        {
            dmaAddrRefresh = (data << 6) | 0xC000;
        }
        public override void Poke4(int addr, byte data)
        {
            dmaSizeRefresh = (data << 4) | 0x0001;
        }
        public override int RenderSample()
        {
            if (dmaEnabled)
            {
                sampleTimer++;

                if (sampleTimer > sampleDelay)
                {
                    sampleTimer -= sampleDelay;

                    if (dmaBits == 7)
                    {
                        if (dmaSize != 0)
                        {
                            dmaBits = 0;
                            dmaByte = nes.Memory[Address: (ushort)dmaAddr];

                            dmaAddr = ((dmaAddr + 1) & 0x7FFF) | 0x8000;
                            dmaSize--;

                            if (dmaSize == 0)
                            {
                                if (dmaLooping)
                                {
                                    dmaAddr = dmaAddrRefresh;
                                    dmaSize = dmaSizeRefresh;
                                }
                                else if (IrqEnabled)
                                {
                                    nes.APU.DeltaIrqPending = true;
                                }
                            }
                        }
                        else
                        {
                            dmaEnabled = false;
                        }
                    }
                    else
                    {
                        dmaBits = (dmaBits + 1);
                        dmaByte = (dmaByte / 2);
                    }

                    if (dmaEnabled)
                    {
                        if ((dmaByte & 0x01) != 0)
                        {
                            output = Math.Min(output + 2, 0x7F);
                        }
                        else
                        {
                            output = Math.Max(output - 2, 0x00);
                        }
                    }
                }
            }

            if (nes.APU.DeltaIrqPending)
                nes.CPU.SetIRQ(true);

            return output;
        }

        public override void SaveState(System.IO.Stream stream)
        {
            byte status = 0;
            if (dmaEnabled)
                status |= 0x01;
            if (dmaLooping)
                status |= 0x02;
            stream.WriteByte(status);

            stream.WriteByte((byte)((dmaAddr & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dmaAddr & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dmaAddr & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dmaAddr & 0x000000FF)));

            stream.WriteByte((byte)((dmaAddrRefresh & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dmaAddrRefresh & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dmaAddrRefresh & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dmaAddrRefresh & 0x000000FF)));

            stream.WriteByte((byte)((dmaBits & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dmaBits & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dmaBits & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dmaBits & 0x000000FF)));

            stream.WriteByte((byte)((dmaByte & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dmaByte & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dmaByte & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dmaByte & 0x000000FF)));

            stream.WriteByte((byte)((dmaSize & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dmaSize & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dmaSize & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dmaSize & 0x000000FF)));

            stream.WriteByte((byte)((dmaSizeRefresh & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dmaSizeRefresh & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dmaSizeRefresh & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dmaSizeRefresh & 0x000000FF)));

            stream.WriteByte((byte)((output & 0xFF000000) >> 24));
            stream.WriteByte((byte)((output & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((output & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((output & 0x000000FF)));
        }
        public override void LoadState(System.IO.Stream stream)
        {
            byte status = (byte)stream.ReadByte();
            dmaEnabled = ((status & 0x01) == 0x01);
            dmaLooping = ((status & 0x02) == 0x02);

            dmaAddr = (int)(stream.ReadByte() << 24);
            dmaAddr |= (int)(stream.ReadByte() << 16);
            dmaAddr |= (int)(stream.ReadByte() << 8);
            dmaAddr |= stream.ReadByte();

            dmaAddrRefresh = (int)(stream.ReadByte() << 24);
            dmaAddrRefresh |= (int)(stream.ReadByte() << 16);
            dmaAddrRefresh |= (int)(stream.ReadByte() << 8);
            dmaAddrRefresh |= stream.ReadByte();

            dmaBits = (int)(stream.ReadByte() << 24);
            dmaBits |= (int)(stream.ReadByte() << 16);
            dmaBits |= (int)(stream.ReadByte() << 8);
            dmaBits |= stream.ReadByte();

            dmaByte = (int)(stream.ReadByte() << 24);
            dmaByte |= (int)(stream.ReadByte() << 16);
            dmaByte |= (int)(stream.ReadByte() << 8);
            dmaByte |= stream.ReadByte();

            dmaSize = (int)(stream.ReadByte() << 24);
            dmaSize |= (int)(stream.ReadByte() << 16);
            dmaSize |= (int)(stream.ReadByte() << 8);
            dmaSize |= stream.ReadByte();

            dmaSizeRefresh = (int)(stream.ReadByte() << 24);
            dmaSizeRefresh |= (int)(stream.ReadByte() << 16);
            dmaSizeRefresh |= (int)(stream.ReadByte() << 8);
            dmaSizeRefresh |= stream.ReadByte();

            output = (int)(stream.ReadByte() << 24);
            output |= (int)(stream.ReadByte() << 16);
            output |= (int)(stream.ReadByte() << 8);
            output |= stream.ReadByte();
        }
    }
}