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

    public class ChannelSqr : NesApuChannel
    {
        /*private static readonly int[][] DutyForms =
        {
            new int[] {  0, ~0,  0,  0,  0,  0,  0,  0 }, // 12.5%
            new int[] {  0, ~0, ~0,  0,  0,  0,  0,  0 }, // 25.0%
            new int[] {  0, ~0, ~0, ~0, ~0,  0,  0,  0 }, // 50.0%
            new int[] { ~0,  0,  0, ~0, ~0, ~0, ~0, ~0 }, // 75.0% (25.0% negated)
        };*/
        /* 
           0  [--______________] 12.5%   
           1  [----____________] 25.0%   
           2  [--------________] 50.0% 
           3  [------------____] 75.0%
         */
        private static readonly int[][] DutyForms =
        {
            new int[] { ~0,  0,  0,  0,  0,  0,  0,  0 }, // 12.5%
            new int[] { ~0, ~0,  0,  0,  0,  0,  0,  0 }, // 25.0%
            new int[] { ~0, ~0, ~0, ~0,  0,  0,  0,  0 }, // 50.0%
            new int[] { ~0, ~0, ~0, ~0, ~0, ~0,  0,  0 }, // 75.0%
        };
        private NesApuDuration duration;
        private NesApuEnvelope envelope;
        private bool active = false;
        private bool sweepReload = false;
        private bool validFrequency = false;
        private int amp = 0;
        private int dutyForm = 0;
        private int dutyStep = 0;
        private int enabled = 0;
        private int sweepCount = 0;
        private int sweepRate = 0;
        private int sweepShift = 0;
        private int sweepIncrease = 0;
        private int waveLength = 0;
        new private float frequency = 0;
        private float timer = 0;

        public bool Enabled
        {
            get
            {
                return this.duration.Counter != 0;
            }
            set
            {
                this.enabled = value ? ~0 : 0;
                this.duration.Counter &= enabled;
                this.active = this.CanOutput();
            }
        }

        public ChannelSqr(NES nes)
            : base(nes)
        {
        }

        private void UpdateFrequency()
        {
            if (waveLength >= 0x08 && waveLength + (sweepIncrease & waveLength >> sweepShift) <= 0x7FF)
            {
                frequency = (waveLength + 1) << 1;
                validFrequency = true;
            }
            else
            {
                validFrequency = false;
            }

            active = CanOutput();
        }
        private bool CanOutput()
        {
            return (duration.Counter != 0) && (envelope.Sound != 0) && validFrequency;
        }

        public void UpdateSweep(int complement)
        {
            if (sweepRate != 0 && --sweepCount == 0)
            {
                sweepCount = sweepRate;

                if (waveLength >= 0x0008)
                {
                    int shifted = (waveLength >> sweepShift);

                    if (sweepIncrease == 0)
                    {
                        waveLength -= shifted + complement;
                        UpdateFrequency();
                    }
                    else if (waveLength + shifted <= 0x07FF)
                    {
                        waveLength += shifted;
                        UpdateFrequency();
                    }
                }
            }

            if (sweepReload)
            {
                sweepReload = false;
                sweepCount = sweepRate;
            }
        }

        public override void ClockHalf()
        {
            this.duration.Clock();
            this.active = CanOutput();
        }
        public override void ClockQuad()
        {
            this.envelope.Clock();
            this.active = CanOutput();
        }
        public override void Poke1(int addr, byte data)
        {
            this.dutyForm = (data & 0xC0) >> 6;
            this.duration.Enabled = (data & 0x20) == 0;
            this.envelope.Looping = (data & 0x20) != 0;
            this.envelope.Enabled = (data & 0x10) != 0;
            this.envelope.Sound = (byte)(data & 0x0F);
        }
        public override void Poke2(int addr, byte data)
        {
            sweepIncrease = (data & 0x08) != 0 ? 0 : ~0;
            sweepShift = (data & 0x07);
            sweepRate = 0;

            if ((data & (0x80 | 0x07)) > 0x80)
            {
                sweepRate = (data >> 4 & 0x07) + 1;
                sweepReload = true;
            }

            UpdateFrequency();
        }
        public override void Poke3(int addr, byte data)
        {
            this.waveLength = (this.waveLength & 0x0700) | (data << 0 & 0x00FF);
            this.UpdateFrequency();
        }
        public override void Poke4(int addr, byte data)
        {
            this.waveLength = (this.waveLength & 0x00FF) | (data << 8 & 0x0700);
            this.UpdateFrequency();

            this.duration.Counter = DurationTable[(data & 0xF8) >> 3] & enabled;
            this.envelope.Refresh = true;
            this.dutyStep = 0;

            this.active = this.CanOutput();
        }
        public override int RenderSample(float rate)
        {
            if (active)
            {
                float sum = timer;
                timer -= rate;

                int[] form = DutyForms[dutyForm];

                if (timer >= 0)
                {
                    amp = (envelope.Sound & form[dutyStep]);
                }
                else
                {
                    sum *= (envelope.Sound & form[dutyStep]);

                    do
                    {
                        sum += Math.Min(-timer, frequency) * (envelope.Sound & form[dutyStep = (dutyStep - 1) & 0x07]);
                        timer += frequency;
                    }
                    while (timer < 0);

                    amp = (int)((sum + rate / 2) / rate);
                }
            }
            else if (amp != 0)
            {
                amp--;
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
            envelope.SaveState(stream);

            byte status = 0;
            if (active)
                status |= 0x01;
            if (sweepReload)
                status |= 0x02;
            if (validFrequency)
                status |= 0x04;
            stream.WriteByte(status);

            stream.WriteByte((byte)((amp & 0xFF000000) >> 24));
            stream.WriteByte((byte)((amp & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((amp & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((amp & 0x000000FF)));

            stream.WriteByte((byte)((dutyForm & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dutyForm & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dutyForm & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dutyForm & 0x000000FF)));

            stream.WriteByte((byte)((dutyStep & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dutyStep & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dutyStep & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dutyStep & 0x000000FF)));

            stream.WriteByte((byte)((enabled & 0xFF000000) >> 24));
            stream.WriteByte((byte)((enabled & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((enabled & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((enabled & 0x000000FF)));

            stream.WriteByte((byte)((sweepCount & 0xFF000000) >> 24));
            stream.WriteByte((byte)((sweepCount & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((sweepCount & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((sweepCount & 0x000000FF)));

            stream.WriteByte((byte)((sweepRate & 0xFF000000) >> 24));
            stream.WriteByte((byte)((sweepRate & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((sweepRate & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((sweepRate & 0x000000FF)));

            stream.WriteByte((byte)((sweepShift & 0xFF000000) >> 24));
            stream.WriteByte((byte)((sweepShift & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((sweepShift & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((sweepShift & 0x000000FF)));

            stream.WriteByte((byte)((sweepIncrease & 0xFF000000) >> 24));
            stream.WriteByte((byte)((sweepIncrease & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((sweepIncrease & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((sweepIncrease & 0x000000FF)));

            stream.WriteByte((byte)((waveLength & 0xFF000000) >> 24));
            stream.WriteByte((byte)((waveLength & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((waveLength & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((waveLength & 0x000000FF)));

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

            byte status = (byte)stream.ReadByte();
            active = ((status & 0x01) == 0x01);
            sweepReload = ((status & 0x02) == 0x02);
            validFrequency = ((status & 0x04) == 0x04);

            amp = (int)(stream.ReadByte() << 24);
            amp |= (int)(stream.ReadByte() << 16);
            amp |= (int)(stream.ReadByte() << 8);
            amp |= stream.ReadByte();

            dutyForm = (int)(stream.ReadByte() << 24);
            dutyForm |= (int)(stream.ReadByte() << 16);
            dutyForm |= (int)(stream.ReadByte() << 8);
            dutyForm |= stream.ReadByte();

            dutyStep = (int)(stream.ReadByte() << 24);
            dutyStep |= (int)(stream.ReadByte() << 16);
            dutyStep |= (int)(stream.ReadByte() << 8);
            dutyStep |= stream.ReadByte();

            enabled = (int)(stream.ReadByte() << 24);
            enabled |= (int)(stream.ReadByte() << 16);
            enabled |= (int)(stream.ReadByte() << 8);
            enabled |= stream.ReadByte();

            sweepCount = (int)(stream.ReadByte() << 24);
            sweepCount |= (int)(stream.ReadByte() << 16);
            sweepCount |= (int)(stream.ReadByte() << 8);
            sweepCount |= stream.ReadByte();

            sweepRate = (int)(stream.ReadByte() << 24);
            sweepRate |= (int)(stream.ReadByte() << 16);
            sweepRate |= (int)(stream.ReadByte() << 8);
            sweepRate |= stream.ReadByte();

            sweepShift = (int)(stream.ReadByte() << 24);
            sweepShift |= (int)(stream.ReadByte() << 16);
            sweepShift |= (int)(stream.ReadByte() << 8);
            sweepShift |= stream.ReadByte();

            sweepIncrease = (int)(stream.ReadByte() << 24);
            sweepIncrease |= (int)(stream.ReadByte() << 16);
            sweepIncrease |= (int)(stream.ReadByte() << 8);
            sweepIncrease |= stream.ReadByte();

            waveLength = (int)(stream.ReadByte() << 24);
            waveLength |= (int)(stream.ReadByte() << 16);
            waveLength |= (int)(stream.ReadByte() << 8);
            waveLength |= stream.ReadByte();

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