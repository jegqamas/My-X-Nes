using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNes.Nes
{
   
    public class ChannelSS5BWave  : NesApuChannel
    {
        private static readonly int[] DutyForm =
        {
             0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0, ~0,
        };

        private int volume = 0;
        private int dutyCount = 0;
        private int waveLength = 0;
        private float timer = 0;
        private new float frequency = 1;

        public bool Enabled = false;

        public ChannelSS5BWave(NES nes)
            : base(nes)
        {
        }

        public override void ClockHalf() { }
        public override void ClockQuad() { }
        public override void Poke1(int addr, byte data)
        {
            this.waveLength = (this.waveLength & 0x0F00) | (data << 0 & 0x00FF);
            this.frequency = (this.waveLength + 1);
        }
        public override void Poke2(int addr, byte data)
        {
            this.waveLength = (this.waveLength & 0x00FF) | (data << 8 & 0x0F00);
            this.frequency = (this.waveLength + 1);
        }
        public override void Poke3(int addr, byte data)
        {
            this.volume = (data & 0x0F);
        }
        public override void Poke4(int addr, byte data) { }
        public override int RenderSample(float rate)
        {
            if (Enabled)
            {
                float sum = timer;
                timer -= rate;

                if (timer >= 0)
                {
                    return (volume & DutyForm[dutyCount]);
                }
                else
                {
                    sum *= (volume & DutyForm[dutyCount]);

                    do
                    {
                        sum += Math.Min(-timer, frequency) * (volume & DutyForm[dutyCount = (dutyCount + 1) & 0x1F]);
                        timer += frequency;
                    }
                    while (timer < 0);

                    return (int)(sum / rate);
                }
            }
            else
            {
                return 0;
            }
        }

        public override void SaveState(System.IO.Stream stream)
        {
            base.SaveState(stream);
            stream.WriteByte((byte)((volume & 0xFF000000) >> 24));
            stream.WriteByte((byte)((volume & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((volume & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((volume & 0x000000FF)));

            stream.WriteByte((byte)((dutyCount & 0xFF000000) >> 24));
            stream.WriteByte((byte)((dutyCount & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((dutyCount & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((dutyCount & 0x000000FF)));

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
            volume = (int)(stream.ReadByte() << 24);
            volume |= (int)(stream.ReadByte() << 16);
            volume |= (int)(stream.ReadByte() << 8);
            volume |= stream.ReadByte();

            dutyCount = (int)(stream.ReadByte() << 24);
            dutyCount |= (int)(stream.ReadByte() << 16);
            dutyCount |= (int)(stream.ReadByte() << 8);
            dutyCount |= stream.ReadByte();

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
