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
   
    public class ChannelNoi : NesApuChannel
    {
        private static readonly int[] FrequencyTable = 
        { 
            0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0060, 0x0080, 0x00A0,
            0x00CA, 0x00FE, 0x017C, 0x01FC, 0x02FA, 0x03F8, 0x07F2, 0x0FE4,
        };

        private NesApuDuration duration;
        private NesApuEnvelope envelope;
        private bool active = false;
        private int enabled = 0;
        private int shiftBit = 8;
        private int shiftReg = 1;
        new private float frequency = FrequencyTable[0];
        private float timer = 0;

        public bool Enabled
        {
            get
            {
                return duration.Counter != 0;
            }
            set
            {
                enabled = value ? ~0 : 0;
                duration.Counter &= enabled;
                this.active = this.CanOutput();
            }
        }

        public ChannelNoi(NES nes)
            : base(nes)
        {
        }

        private bool CanOutput()
        {
            return duration.Counter != 0 && envelope.Sound != 0;
        }

        public override void ClockHalf()
        {
            this.duration.Clock();
            this.active = this.CanOutput();
        }
        public override void ClockQuad()
        {
            this.envelope.Clock();
            this.active = this.CanOutput();
        }
        public override void Poke1(int addr, byte data)
        {
            duration.Enabled = (data & 0x20) == 0;
            envelope.Looping = (data & 0x20) != 0;
            envelope.Enabled = (data & 0x10) != 0;
            envelope.Sound = (byte)(data & 0x0F);
        }
        public override void Poke2(int addr, byte data) { }
        public override void Poke3(int addr, byte data)
        {
            frequency = FrequencyTable[data & 0x0F];
            shiftBit = (data & 0x80) != 0 ? 8 : 13;
        }
        public override void Poke4(int addr, byte data)
        {
            this.duration.Counter = DurationTable[data >> 3] & enabled;
            this.envelope.Refresh = true;
            this.active = this.CanOutput();
        }
        public override int RenderSample(float rate)
        {
            float sum = timer;
            timer -= rate;

            if (active)
            {
                if (timer >= 0)
                {
                    if ((shiftReg & 0x01) == 0)
                        return envelope.Sound;

                }
                else
                {
                    if ((shiftReg & 0x01) != 0)
                        sum = 0;

                    do
                    {
                        shiftReg = (shiftReg >> 1) | ((shiftReg << 14 ^ shiftReg << shiftBit) & 0x4000);

                        if ((shiftReg & 0x01) == 0)
                            sum += Math.Min(-timer, frequency);

                        timer += frequency;
                    }
                    while (timer < 0);

                    return (int)(((sum * envelope.Sound) + rate / 2) / rate);
                }
            }
            else
            {
                while (timer < 0)
                {
                    shiftReg = (shiftReg >> 1) | ((shiftReg << 14 ^ shiftReg << shiftBit) & 0x4000);
                    timer += frequency;
                }
            }

            return 0;
        }

        public override void SaveState(System.IO.Stream stream)
        {
            base.SaveState(stream);
            stream.WriteByte((byte)(duration.Enabled ? 1 : 0));
            stream.WriteByte((byte)((duration.Counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((duration.Counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((duration.Counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((duration.Counter & 0x000000FF)));
            envelope.SaveState(stream);

            stream.WriteByte((byte)(active ? 1 : 0));

            stream.WriteByte((byte)((enabled & 0xFF000000) >> 24));
            stream.WriteByte((byte)((enabled & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((enabled & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((enabled & 0x000000FF)));

            stream.WriteByte((byte)((shiftBit & 0xFF000000) >> 24));
            stream.WriteByte((byte)((shiftBit & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((shiftBit & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((shiftBit & 0x000000FF)));

            stream.WriteByte((byte)((shiftReg & 0xFF000000) >> 24));
            stream.WriteByte((byte)((shiftReg & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((shiftReg & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((shiftReg & 0x000000FF)));

            string[] frequency_ = this.frequency.ToString().Split(new char[] { '.' });
            int freq_left = int.Parse(frequency_[0]);
            int freq_right = int.Parse(frequency_.Length == 2 ? frequency_[1] : "0");
            stream.WriteByte((byte)((freq_left & 0xFF000000) >> 24));
            stream.WriteByte((byte)((freq_left & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((freq_left & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((freq_left & 0x000000FF)));
            stream.WriteByte((byte)((freq_right & 0xFF000000) >> 24));
            stream.WriteByte((byte)((freq_right & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((freq_right & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((freq_right & 0x000000FF)));

            string[] timer_ = this.timer.ToString().Split(new char[] { '.' });
            freq_left = int.Parse(timer_[0]);
            freq_right = int.Parse(timer_.Length == 2 ? timer_[1] : "0");
            stream.WriteByte((byte)((freq_left & 0xFF000000) >> 24));
            stream.WriteByte((byte)((freq_left & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((freq_left & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((freq_left & 0x000000FF)));
            stream.WriteByte((byte)((freq_right & 0xFF000000) >> 24));
            stream.WriteByte((byte)((freq_right & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((freq_right & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((freq_right & 0x000000FF)));
        }
        public override void LoadState(System.IO.Stream stream)
        {
            base.LoadState(stream);
            duration.Enabled = stream.ReadByte() == 1;
            duration.Counter = (int)(stream.ReadByte() << 24);
            duration.Counter |= (int)(stream.ReadByte() << 16);
            duration.Counter |= (int)(stream.ReadByte() << 8);
            duration.Counter |= stream.ReadByte();
            envelope.LoadState(stream);

            active = stream.ReadByte() == 1;

            enabled = (int)(stream.ReadByte() << 24);
            enabled |= (int)(stream.ReadByte() << 16);
            enabled |= (int)(stream.ReadByte() << 8);
            enabled |= stream.ReadByte();

            shiftBit = (int)(stream.ReadByte() << 24);
            shiftBit |= (int)(stream.ReadByte() << 16);
            shiftBit |= (int)(stream.ReadByte() << 8);
            shiftBit |= stream.ReadByte();

            shiftReg = (int)(stream.ReadByte() << 24);
            shiftReg |= (int)(stream.ReadByte() << 16);
            shiftReg |= (int)(stream.ReadByte() << 8);
            shiftReg |= stream.ReadByte();

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
            this.frequency = float.Parse(smpl_left.ToString() + "." + smpl_right.ToString());

            smpl_left = (int)(stream.ReadByte() << 24);
            smpl_left |= (int)(stream.ReadByte() << 16);
            smpl_left |= (int)(stream.ReadByte() << 8);
            smpl_left |= stream.ReadByte();
            smpl_right = (int)(stream.ReadByte() << 24);
            smpl_right |= (int)(stream.ReadByte() << 16);
            smpl_right |= (int)(stream.ReadByte() << 8);
            smpl_right |= stream.ReadByte();
            this.timer = float.Parse(smpl_left.ToString() + "." + smpl_right.ToString());
        }
    }
}