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

    public class ChannelTri : NesApuChannel
    {
        private const int MIN_FRQ = 3;

        private static readonly byte[] Sequence =
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
        };

        private NesApuDuration duration;
        private bool active = false;
        private bool halted = false;
        private bool linearControl = false;
        private int amp = 0;
        private int enabled = 0;
        private int linearCounter = 0;
        private int linearCounterLatch = 0;
        private int step = 0;
        private int waveForm = 0;
        new private float frequency = 0;
        private float timer = 0;

        public bool Enabled
        {
            get
            {
                return duration.Counter != 0;
            }
            set
            {
                enabled = (value) ? ~0 : 0;
                duration.Counter &= enabled;
            }
        }

        public ChannelTri(NES nes)
            : base(nes)
        {
        }

        private bool CanOutput()
        {
            return (duration.Counter != 0) && (linearCounter != 0) && (waveForm >= MIN_FRQ);
        }

        public override void ClockHalf()
        {
            duration.Clock();
            active = CanOutput();
        }
        public override void ClockQuad()
        {
            if (halted)
            {
                linearCounter = linearCounterLatch;
            }
            else
            {
                if (linearCounter != 0)
                {
                    linearCounter--;
                    active = CanOutput();
                }
            }

            halted &= linearControl;
        }
        public override void Poke1(int addr, byte data)
        {
            duration.Enabled = (data & 0x80) == 0;
            linearControl = (data & 0x80) != 0;
            linearCounterLatch = (data & 0x7F);
        }
        public override void Poke2(int addr, byte data)
        {
        }
        public override void Poke3(int addr, byte data)
        {
            waveForm = (waveForm & 0x0700) | (data << 0 & 0x00FF);
            frequency = (waveForm + 1);

            active = CanOutput();
        }
        public override void Poke4(int addr, byte data)
        {
            waveForm = (waveForm & 0x00FF) | (data << 8 & 0x0700);
            frequency = (waveForm + 1);

            duration.Counter = DurationTable[data >> 3] & enabled;

            halted = true;
            active = CanOutput();
        }
        public override int RenderSample(float rate)
        {
            if (active)
            {
                float sum = timer;
                timer -= rate;

                if (timer >= 0)
                {
                    amp = Sequence[step];
                }
                else
                {
                    sum *= Sequence[step];

                    do
                    {
                        sum += Math.Min(-timer, frequency) * Sequence[step = (step + 1) & 0x1F];
                        timer += frequency;
                    }
                    while (timer < 0);

                    amp = (int)((sum + rate / 2) / rate);
                }
            }
            return amp;
        }

        public override void SaveState(System.IO.Stream stream)
        {
            base.SaveState(stream);
            stream.WriteByte((byte)(duration.Enabled ? 1 : 0));
            stream.WriteByte((byte)((duration.Counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((duration.Counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((duration.Counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((duration.Counter & 0x000000FF)));

            byte status = 0;
            if (active)
                status |= 0x01;
            if (halted)
                status |= 0x02;
            if (linearControl)
                status |= 0x04;
            stream.WriteByte(status);

            stream.WriteByte((byte)((amp & 0xFF000000) >> 24));
            stream.WriteByte((byte)((amp & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((amp & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((amp & 0x000000FF)));

            stream.WriteByte((byte)((enabled & 0xFF000000) >> 24));
            stream.WriteByte((byte)((enabled & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((enabled & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((enabled & 0x000000FF)));

            stream.WriteByte((byte)((linearCounter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((linearCounter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((linearCounter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((linearCounter & 0x000000FF)));

            stream.WriteByte((byte)((linearCounterLatch & 0xFF000000) >> 24));
            stream.WriteByte((byte)((linearCounterLatch & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((linearCounterLatch & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((linearCounterLatch & 0x000000FF)));

            stream.WriteByte((byte)((step & 0xFF000000) >> 24));
            stream.WriteByte((byte)((step & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((step & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((step & 0x000000FF)));

            stream.WriteByte((byte)((waveForm & 0xFF000000) >> 24));
            stream.WriteByte((byte)((waveForm & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((waveForm & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((waveForm & 0x000000FF)));

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

            byte status = (byte)stream.ReadByte();
            active = ((status & 0x01) == 0x01);
            halted = ((status & 0x02) == 0x02);
            linearControl = ((status & 0x04) == 0x04);

            amp = (int)(stream.ReadByte() << 24);
            amp |= (int)(stream.ReadByte() << 16);
            amp |= (int)(stream.ReadByte() << 8);
            amp |= stream.ReadByte();

            enabled = (int)(stream.ReadByte() << 24);
            enabled |= (int)(stream.ReadByte() << 16);
            enabled |= (int)(stream.ReadByte() << 8);
            enabled |= stream.ReadByte();

            linearCounter = (int)(stream.ReadByte() << 24);
            linearCounter |= (int)(stream.ReadByte() << 16);
            linearCounter |= (int)(stream.ReadByte() << 8);
            linearCounter |= stream.ReadByte();

            linearCounterLatch = (int)(stream.ReadByte() << 24);
            linearCounterLatch |= (int)(stream.ReadByte() << 16);
            linearCounterLatch |= (int)(stream.ReadByte() << 8);
            linearCounterLatch |= stream.ReadByte();

            step = (int)(stream.ReadByte() << 24);
            step |= (int)(stream.ReadByte() << 16);
            step |= (int)(stream.ReadByte() << 8);
            step |= stream.ReadByte();

            waveForm = (int)(stream.ReadByte() << 24);
            waveForm |= (int)(stream.ReadByte() << 16);
            waveForm |= (int)(stream.ReadByte() << 8);
            waveForm |= stream.ReadByte();

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