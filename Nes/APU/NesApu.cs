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
using System.IO;
using MyNes.Nes.Output.Audio;

namespace MyNes.Nes
{

    public sealed class NesApu
    {
        private const double NLN_SQR_0 = 95.52;
        private const double NLN_SQR_1 = 8128.0;
        private const double NLN_TND_0 = 163.67;
        private const double NLN_TND_1 = 24329.0;

        private int rPos = 0;
        private int wPos = 0;
        public int[] soundBuffer = new int[44100];

        private float sampleTimer = 0;
        private float sampleDelay = (1789772.72f / 44100.00f) - 0.19f;

        NES nes;

        public IAudioDevice Output;
        public ChannelSqr ChannelSq1;
        public ChannelSqr ChannelSq2;
        public ChannelTri ChannelTri;
        public ChannelNoi ChannelNoi;
        public ChannelDpm ChannelDpm;
        public NesApuExternalComponent External;
        public bool DeltaIrqPending = false;
        public bool EnabledSq1 = true;
        public bool EnabledSq2 = true;
        public bool EnabledTri = true;
        public bool EnabledNoi = true;
        public bool EnabledPcm = true;

        public bool FrameIrqEnabled = false;
        public bool FrameIrqPending = false;
        public bool SequencingMode = false;
        public int Cycles = 0;

        public NesApu(NES nes)
        {
            this.nes = nes;
            ChannelSq1 = new ChannelSqr(nes);
            ChannelSq2 = new ChannelSqr(nes);
            ChannelTri = new ChannelTri(nes);
            ChannelNoi = new ChannelNoi(nes);
            ChannelDpm = new ChannelDpm(nes);
        }

        void AddSample(float sampleRate)
        {
            var tndSample = 0;
            var sqrSample = 0;

            if (EnabledSq1)
                sqrSample += ChannelSq1.RenderSample(sampleRate);
            if (EnabledSq2)
                sqrSample += ChannelSq2.RenderSample(sampleRate);

            if (EnabledTri)
                tndSample += ChannelTri.RenderSample(sampleRate) * 3;
            if (EnabledNoi)
                tndSample += ChannelNoi.RenderSample(sampleRate) * 2;
            if (EnabledPcm)
                tndSample += ChannelDpm.RenderSample() * 1;

            int output = (int)(NesApuMixer.MixSamples(sqrSample, tndSample) * 128);

            if (this.External != null)
                output += this.External.RenderSample(sampleRate);

            soundBuffer[wPos % soundBuffer.Length] = output;
            wPos++;
        }
        void ClockQuad()
        {
            ChannelSq1.ClockQuad();
            ChannelSq2.ClockQuad();
            ChannelNoi.ClockQuad();
            ChannelTri.ClockQuad();
            if (External != null)
                External.ClockQuad();
        }
        void ClockHalf()
        {
            ChannelSq1.UpdateSweep(1);
            ChannelSq2.UpdateSweep(0);
            ChannelSq1.ClockHalf();
            ChannelSq2.ClockHalf();
            ChannelNoi.ClockHalf();
            ChannelTri.ClockHalf();
            if (External != null)
                External.ClockHalf();
        }

        public void ClearBuffer()
        {
            wPos = 0;
            rPos = 0;
        }
        public void Execute()
        {
            Cycles++;
            if (!SequencingMode)
            {
                //Step 1 | 3
                if (Cycles == 7459 | Cycles == 22373)
                    ClockQuad();

                //Step 2
                if (Cycles == 14916)
                {
                    ClockQuad();
                    ClockHalf();
                }

                if (Cycles == 29831)
                    if (FrameIrqEnabled)
                        FrameIrqPending = true;
                //Step 4
                if (Cycles == 29832)
                {
                    ClockQuad();
                    ClockHalf();
                }
                //SET FRAME IRQ FLAG
                if (Cycles == 29833)
                {
                    if (FrameIrqEnabled)
                        FrameIrqPending = true;
                    Cycles = 3;
                }
                //SET CPU IRQ
                if (FrameIrqPending)
                    nes.CPU.SetIRQ(true);
            }
            else
            {
                //Step 0 | 2
                if (Cycles == 2 | Cycles == 14916)
                {
                    ClockQuad();
                    ClockHalf();
                }
                //Step 1 | 3
                if (Cycles == 7459 | Cycles == 22373)
                    ClockQuad();
                //Step 4, do nothing
                if (Cycles == 37282)
                    Cycles = 0;
            }
            if (sampleTimer > 0)
            {
                sampleTimer--;
            }
            else
            {
                sampleTimer += sampleDelay;
                AddSample(sampleDelay);
            }
        }
        public void UpdateBuffer()
        {
            if (Output != null)
                Output.UpdateBuffer(); 
        }
        public void Play()
        {
            if (Output != null)
                Output.Play();
        }
        public void Stop()
        {
            if (Output != null)
                Output.Pause();

            rPos = 0;
            wPos = 0;
        }
        public void SetVolume(int Vol)
        {
            Output.SetVolume(Vol);
        }
        public int PullSample()
        {
            while (rPos >= wPos)
            {
                AddSample(sampleDelay);
            }

            rPos++;
            return soundBuffer[(rPos - 1) % soundBuffer.Length];
        }
        public void SoftReset()
        {
            this.ChannelSq1.Enabled = false;
            this.ChannelSq2.Enabled = false;
            this.ChannelTri.Enabled = false;
            this.ChannelNoi.Enabled = false;
            this.ChannelDpm.Enabled = false;

            this.DeltaIrqPending = false;
            this.FrameIrqEnabled = false;
            this.FrameIrqPending = false;
        }

        public byte Dump4015(int addr)
        {
            byte data = 0;

            if (this.ChannelSq1.Enabled) data |= 0x01;
            if (this.ChannelSq2.Enabled) data |= 0x02;
            if (this.ChannelTri.Enabled) data |= 0x04;
            if (this.ChannelNoi.Enabled) data |= 0x08;
            if (this.ChannelDpm.Enabled) data |= 0x10;
            if (this.FrameIrqPending) data |= 0x40;
            if (this.DeltaIrqPending) data |= 0x80;

            return data;
        }
        public byte PeekNull(int addr)
        {
            return 0;
        }
        public byte Peek4015(int addr)
        {
            byte data = 0;

            if (this.ChannelSq1.Enabled) data |= 0x01;
            if (this.ChannelSq2.Enabled) data |= 0x02;
            if (this.ChannelTri.Enabled) data |= 0x04;
            if (this.ChannelNoi.Enabled) data |= 0x08;
            if (this.ChannelDpm.Enabled) data |= 0x10;
            if (this.FrameIrqPending) data |= 0x40;
            if (this.DeltaIrqPending) data |= 0x80;

            this.FrameIrqPending = false;

            return data;
        }
        public void Poke4015(int addr, byte data)
        {
            this.ChannelSq1.Enabled = (data & 0x01) != 0;
            this.ChannelSq2.Enabled = (data & 0x02) != 0;
            this.ChannelTri.Enabled = (data & 0x04) != 0;
            this.ChannelNoi.Enabled = (data & 0x08) != 0;
            this.ChannelDpm.Enabled = (data & 0x10) != 0;
            this.DeltaIrqPending = false;
        }
        public void Poke4017(int addr, byte data)
        {
            this.SequencingMode = (data & 0x80) != 0;
            this.FrameIrqEnabled = (data & 0x40) == 0;

            this.Cycles = 0;

            if (!FrameIrqEnabled)
                this.FrameIrqPending = false;
        }
        public void Poke5015(byte data)
        {
            if (External != null)
            {
                ((Mmc5ExternalComponent)External).ChannelSq1.Enabled = (data & 0x01) != 0;
                ((Mmc5ExternalComponent)External).ChannelSq2.Enabled = (data & 0x02) != 0;
            }
        }
        public byte Peek5015()
        {
            byte rt = 0;
            if (External != null)
            {
                if (((Mmc5ExternalComponent)External).ChannelSq1.Enabled)
                    rt |= 0x01;
                if (((Mmc5ExternalComponent)External).ChannelSq2.Enabled)
                    rt |= 0x02;
            }
            return rt;
        }
        public void Shutdown()
        {
            wPos = rPos = 0; sampleTimer = 0;
            if (Output != null)
            {
                Output.Shutdown();
                Output = null;
            }

            Output = null;
        }

        public void SaveState(Stream stream)
        {
            byte status = 0;
            if (DeltaIrqPending)
                status |= 0x01;
            if (FrameIrqEnabled)
                status |= 0x02;
            if (FrameIrqPending)
                status |= 0x04;
            if (SequencingMode)
                status |= 0x10;
            stream.WriteByte(status);

            stream.WriteByte((byte)((Cycles & 0xFF000000) >> 24));
            stream.WriteByte((byte)((Cycles & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((Cycles & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((Cycles & 0x000000FF)));

            ChannelSq1.SaveState(stream);
            ChannelSq2.SaveState(stream);
            ChannelTri.SaveState(stream);
            ChannelNoi.SaveState(stream);
            ChannelDpm.SaveState(stream);
            if (External != null)
                External.SaveState(stream);
        }
        public void LoadState(Stream stream)
        {
            byte status = (byte)stream.ReadByte();
            DeltaIrqPending = ((status & 0x1) == 0x1);
            FrameIrqEnabled = ((status & 0x2) == 0x2);
            FrameIrqPending = ((status & 0x4) == 0x4);
            SequencingMode = ((status & 0x10) == 0x10);

            Cycles = (int)(stream.ReadByte() << 24);
            Cycles |= (int)(stream.ReadByte() << 16);
            Cycles |= (int)(stream.ReadByte() << 8);
            Cycles |= stream.ReadByte();

            ChannelSq1.LoadState(stream);
            ChannelSq2.LoadState(stream);
            ChannelTri.LoadState(stream);
            ChannelNoi.LoadState(stream);
            ChannelDpm.LoadState(stream);
            if (External != null)
                External.LoadState(stream);
        }
    }
}