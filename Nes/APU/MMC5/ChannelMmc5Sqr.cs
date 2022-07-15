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

    public class ChannelMmc5Sqr : NesApuChannel
    {
        private static int[][] DutyForms =
        {
            new int[] { ~0, ~0, ~0, ~0, ~0, ~0, ~0,  0 },//87.5%
            new int[] { ~0, ~0, ~0, ~0, ~0, ~0,  0,  0 },//75.0%
            new int[] { ~0, ~0, ~0, ~0,  0,  0,  0,  0 },//50.0%
            new int[] { ~0, ~0,  0,  0,  0,  0,  0,  0 },//25.0%
        };

        private NesApuDuration duration;
        private NesApuEnvelope envelope;
        private int dutyForm = 0;
        private int dutyStep = 0;
        private int enabled = 0;

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
            }
        }

        public ChannelMmc5Sqr(NES nes)
            : base(nes)
        {
        }

        void UpdateFrequency()
        {
            if (freqTimer >= 0x08 && freqTimer <= 0x7FF)
            {
                frequency = 1789772.72 / 2 / (freqTimer + 1);
                sampleDelay = 44100.00 / frequency;
            }
        }

        public override void ClockHalf()
        {
            this.duration.Clock();
        }
        public override void ClockQuad()
        {
            this.envelope.Clock();
        }
        public override void Poke1(int addr, byte data)
        {
            envelope.Enabled = (data & 0x10) != 0;
            envelope.Looping = (data & 0x20) != 0;
            duration.Enabled = (data & 0x20) == 0;
            envelope.Sound = (byte)(data & 0x0F);

            dutyForm = (data & 0xC0) >> 6;
        }
        public override void Poke2(int addr, byte data)
        {
        }
        public override void Poke3(int addr, byte data)
        {
            freqTimer = (freqTimer & 0x0700) | ((data & 0xFF) << 0);
            UpdateFrequency();
        }
        public override void Poke4(int addr, byte data)
        {
            freqTimer = (freqTimer & 0x00FF) | ((data & 0x07) << 8);
            UpdateFrequency();

            duration.Counter = DurationTable[(data & 0xF8) >> 3] & enabled;
            envelope.Refresh = true;

            dutyStep = 0;
        }
        public override int RenderSample()
        {
            if (this.duration.Counter != 0 & freqTimer >= 0x08 && freqTimer <= 0x7FF)
            {
                this.sampleTimer++;

                if (this.sampleTimer >= this.sampleDelay)
                {
                    this.sampleTimer -= this.sampleDelay;

                    this.dutyStep = (this.dutyStep - 1) & 0x07;
                }

                return envelope.Sound & DutyForms[dutyForm][dutyStep];
            }

            return 0;
        }
        public override void SaveState(System.IO.Stream stream)
        {
            base.SaveState(stream);
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

            stream.WriteByte((byte)(duration.Enabled ? 1 : 0));
            stream.WriteByte((byte)((duration.Counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((duration.Counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((duration.Counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((duration.Counter & 0x000000FF)));
        }

        public override void LoadState(System.IO.Stream stream)
        {
            base.LoadState(stream);
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

            duration.Enabled = stream.ReadByte() == 1;
            duration.Counter = (int)(stream.ReadByte() << 24);
            duration.Counter |= (int)(stream.ReadByte() << 16);
            duration.Counter |= (int)(stream.ReadByte() << 8);
            duration.Counter |= stream.ReadByte();
        }
    }
}