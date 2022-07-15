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
   
    public abstract class NesApuChannel 
    {
        protected static byte[] DurationTable = 
        {
            0x0A, 0xFE, 0x14, 0x02, 0x28, 0x04, 0x50, 0x06, 0xA0, 0x08, 0x3C, 0x0A, 0x0E, 0x0C, 0x1A, 0x0E,
            0x0C, 0x10, 0x18, 0x12, 0x30, 0x14, 0x60, 0x16, 0xC0, 0x18, 0x48, 0x1A, 0x10, 0x1C, 0x20, 0x1E,
        };

        protected int freqTimer = 0;
        protected double frequency = 0;
        protected double sampleDelay = 0;
        protected double sampleTimer = 0;
        public NES nes;

        public NesApuChannel(NES nes)
        {
            this.nes = nes;
        }

        public abstract void ClockHalf();
        public abstract void ClockQuad();
        public abstract void Poke1(int addr, byte data);
        public abstract void Poke2(int addr, byte data);
        public abstract void Poke3(int addr, byte data);
        public abstract void Poke4(int addr, byte data);
        public virtual void SaveState(System.IO.Stream stream) 
        {
            stream.WriteByte((byte)((freqTimer & 0xFF000000) >> 24));
            stream.WriteByte((byte)((freqTimer & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((freqTimer & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((freqTimer & 0x000000FF)));

            string[] frequency_ = frequency.ToString().Split(new char[] { '.' });
            int freq_left = int.Parse(frequency_[0]);
            int freq_right = int.Parse(frequency_.Length == 2 ? frequency_[1].Substring(0, 9) : "0");
            stream.WriteByte((byte)((freq_left & 0xFF000000) >> 24));
            stream.WriteByte((byte)((freq_left & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((freq_left & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((freq_left & 0x000000FF)));
            stream.WriteByte((byte)((freq_right & 0xFF000000) >> 24));
            stream.WriteByte((byte)((freq_right & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((freq_right & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((freq_right & 0x000000FF)));

            string[] sampleDelay_ = sampleDelay.ToString().Split(new char[] { '.' });
            int smpl_left = int.Parse(sampleDelay_[0]);
            int smpl_right = int.Parse(sampleDelay_.Length == 2 ? sampleDelay_[1].Substring(0, 9) : "0");
            stream.WriteByte((byte)((smpl_left & 0xFF000000) >> 24));
            stream.WriteByte((byte)((smpl_left & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((smpl_left & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((smpl_left & 0x000000FF)));
            stream.WriteByte((byte)((smpl_right & 0xFF000000) >> 24));
            stream.WriteByte((byte)((smpl_right & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((smpl_right & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((smpl_right & 0x000000FF)));

            string[] sampleTimer_ = sampleTimer.ToString().Split(new char[] { '.' });
            int timer_left = int.Parse(sampleTimer_[0]);
            int timer_right = int.Parse(sampleTimer_.Length == 2 ? sampleTimer_[1].Substring(0, 9) : "0");
            stream.WriteByte((byte)((timer_left & 0xFF000000) >> 24));
            stream.WriteByte((byte)((timer_left & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((timer_left & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((timer_left & 0x000000FF)));
            stream.WriteByte((byte)((timer_right & 0xFF000000) >> 24));
            stream.WriteByte((byte)((timer_right & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((timer_right & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((timer_right & 0x000000FF)));
        }
        public virtual void LoadState(System.IO.Stream stream) 
        {
            freqTimer = (int)(stream.ReadByte() << 24);
            freqTimer |= (int)(stream.ReadByte() << 16);
            freqTimer |= (int)(stream.ReadByte() << 8);
            freqTimer |= stream.ReadByte();

            int smpl_left = 0;
            int smpl_right = 0;
            smpl_left = (int)(stream.ReadByte() << 24);
            smpl_left |= (int)(stream.ReadByte() << 16);
            smpl_left |= (int)(stream.ReadByte() << 8);
            smpl_left |= stream.ReadByte();
            smpl_right = (int)(stream.ReadByte() << 24);
            smpl_right |= (int)(stream.ReadByte() << 16);
            smpl_right |= (int)(stream.ReadByte() << 8);
            smpl_right |= stream.ReadByte();
            frequency = double.Parse(smpl_left.ToString() + "." + smpl_right.ToString());

            smpl_left = (int)(stream.ReadByte() << 24);
            smpl_left |= (int)(stream.ReadByte() << 16);
            smpl_left |= (int)(stream.ReadByte() << 8);
            smpl_left |= stream.ReadByte();
            smpl_right = (int)(stream.ReadByte() << 24);
            smpl_right |= (int)(stream.ReadByte() << 16);
            smpl_right |= (int)(stream.ReadByte() << 8);
            smpl_right |= stream.ReadByte();
            sampleDelay = double.Parse(smpl_left.ToString() + "." + smpl_right.ToString());

            smpl_left = (int)(stream.ReadByte() << 24);
            smpl_left |= (int)(stream.ReadByte() << 16);
            smpl_left |= (int)(stream.ReadByte() << 8);
            smpl_left |= stream.ReadByte();
            smpl_right = (int)(stream.ReadByte() << 24);
            smpl_right |= (int)(stream.ReadByte() << 16);
            smpl_right |= (int)(stream.ReadByte() << 8);
            smpl_right |= stream.ReadByte();
            sampleTimer = double.Parse(smpl_left.ToString() + "." + smpl_right.ToString());
        }

        public virtual int RenderSample()
        {
            return 0;
        }
        public virtual int RenderSample(float rate)
        {
            return 0;
        }
    }
}