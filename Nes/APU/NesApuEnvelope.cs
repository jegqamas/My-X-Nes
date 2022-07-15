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

    public struct NesApuEnvelope
    {
        public bool Enabled;
        public bool Looping;
        public bool Refresh;
        public int Delay;
        public byte Count;
        public int Timer;
        public byte Volume;

        public byte Sound
        {
            get
            {
                if (this.Enabled)
                {
                    return this.Volume;
                }
                else
                {
                    return this.Count;
                }
            }
            set
            {
                // if (this.Enabled)
                // {
                //      this.Delay = this.Volume = value;
                // }
                // else
                // {
                //     this.Delay = value;
                //}
                Delay = value;
                if (this.Enabled)
                    Volume = (byte)Delay;
                else
                    Volume = Count;
            }
        }

        public void Clock()
        {
            if (Refresh)
            {
                Refresh = false;
                Timer = Delay;
                Count = 0x0F;
            }
            else
            {
                if (Timer != 0)
                {
                    Timer--;
                }
                else
                {
                    Timer = Delay;

                    if (Count != 0)
                    {
                        Count--;
                    }
                    else
                    {
                        if (Looping)
                        {
                            Count = 0x0F;
                        }
                    }
                }
            }
        }

        public void SaveState(System.IO.Stream stream)
        {
            byte status = 0;
            if (Enabled)
                status |= 0x01;
            if (Looping)
                status |= 0x02;
            if (Refresh)
                status |= 0x04;
            stream.WriteByte(status);

            stream.WriteByte((byte)((Delay & 0xFF000000) >> 24));
            stream.WriteByte((byte)((Delay & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((Delay & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((Delay & 0x000000FF)));

            stream.WriteByte((byte)((Timer & 0xFF000000) >> 24));
            stream.WriteByte((byte)((Timer & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((Timer & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((Timer & 0x000000FF)));

            stream.WriteByte(Count);
            stream.WriteByte(Volume);
        }
        public void LoadState(System.IO.Stream stream)
        {
            byte status = (byte)stream.ReadByte();
            Enabled = ((status & 0x01) == 0x01);
            Looping = ((status & 0x02) == 0x02);
            Refresh = ((status & 0x04) == 0x04);

            Delay = (int)(stream.ReadByte() << 24);
            Delay |= (int)(stream.ReadByte() << 16);
            Delay |= (int)(stream.ReadByte() << 8);
            Delay |= stream.ReadByte();

            Timer = (int)(stream.ReadByte() << 24);
            Timer |= (int)(stream.ReadByte() << 16);
            Timer |= (int)(stream.ReadByte() << 8);
            Timer |= stream.ReadByte();

            Count = (byte)stream.ReadByte();
            Volume = (byte)stream.ReadByte();
        }
    }
}