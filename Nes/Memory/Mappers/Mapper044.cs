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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNes.Nes
{

    class Mapper44 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[8];
        int bank = 0;
        int prg0 = 0;
        int prg1 = 1;

        int chr01 = 0;
        int chr23 = 2;
        int chr4 = 4;
        int chr5 = 5;
        int chr6 = 6;
        int chr7 = 7;

        int irq_enable = 0;
        int irq_counter = 0;
        int irq_latch = 0;
        public Mapper44(CPUMemory map)
        {
            this.map = map;
        }
        public void Write(ushort address, byte data)
        {
            if (address == 0x6000)
            {
                bank = (data & 0x06) >> 1;
                SetPRG();
                SetCHR();
            }
            switch (address & 0xE001)
            {
                case 0x8000:
                    reg[0] = data;
                    SetPRG();
                    SetCHR();
                    break;
                case 0x8001:
                    reg[1] = data;
                    switch (reg[0] & 0x07)
                    {
                        case 0x00:
                            chr01 = data & 0xFE;
                            SetCHR();
                            break;
                        case 0x01:
                            chr23 = data & 0xFE;
                            SetCHR();
                            break;
                        case 0x02:
                            chr4 = data;
                            SetCHR();
                            break;
                        case 0x03:
                            chr5 = data;
                            SetCHR();
                            break;
                        case 0x04:
                            chr6 = data;
                            SetCHR();
                            break;
                        case 0x05:
                            chr7 = data;
                            SetCHR();
                            break;
                        case 0x06:
                            prg0 = data;
                            SetPRG();
                            break;
                        case 0x07:
                            prg1 = data;
                            SetPRG();
                            break;
                    }
                    break;
                case 0xA000:
                    reg[2] = data;
                    if (map.cartridge.Mirroring != Mirroring.Four_Screen)
                    {
                        if ((data & 0x01) == 0x01)
                            map.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            map.cartridge.Mirroring = Mirroring.Vertical;
                    }
                    break;
                case 0xA001:
                    reg[3] = data;
                    bank = data & 0x07;
                    if (bank == 7)
                    {
                        bank = 6;
                    }
                    SetPRG();
                    SetCHR();
                    break;
                case 0xC000:
                    reg[4] = data;
                    irq_counter = data;
                    break;
                case 0xC001:
                    reg[5] = data;
                    irq_latch = data;
                    break;
                case 0xE000:
                    reg[6] = data;
                    irq_enable = 0;
                    break;
                case 0xE001:
                    reg[7] = data;
                    irq_enable = 1;
                    break;
            }
        }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public void SetUpMapperDefaults()
        {
            if (map.cartridge.IsVRAM)
            {
                chr01 = chr23 = chr4 = chr5 = chr6 = chr7 = 0;
                map.FillCHR(16);
            }
            SetPRG();
            SetCHR();
        }
        public void TickScanlineTimer()
        {
            if (irq_enable != 0)
            {
                if ((--irq_counter) == 0)
                {
                    irq_counter = irq_latch;
                    map.cpu.SetIRQ(true);
                }
            }
        }
        public void TickCycleTimer(int cycles)
        {

        }
        public void SoftReset()
        {

        }
        public bool WriteUnder8000
        {
            get { return true; }
        }
        public bool WriteUnder6000
        {
            get { return false; }
        }
        void SetPRG()
        {
            if ((reg[0] & 0x40) == 0x40)
            {
                map.Switch8kPrgRom((((bank == 6) ? 0x1e : 0x0e) | (bank << 4)) * 2, 0);
                map.Switch8kPrgRom((((bank == 6) ? 0x1f & prg1 : 0x0f & prg1) | (bank << 4)) * 2, 1);
                map.Switch8kPrgRom((((bank == 6) ? 0x1f & prg0 : 0x0f & prg0) | (bank << 4)) * 2, 2);
                map.Switch8kPrgRom((((bank == 6) ? 0x1f : 0x0f) | (bank << 4)) * 2, 3);
            }
            else
            {
                map.Switch8kPrgRom((((bank == 6) ? 0x1f & prg0 : 0x0f & prg0) | (bank << 4)) * 2, 0);
                map.Switch8kPrgRom((((bank == 6) ? 0x1f & prg1 : 0x0f & prg1) | (bank << 4)) * 2, 1);
                map.Switch8kPrgRom((((bank == 6) ? 0x1e : 0x0e) | (bank << 4)) * 2, 2);
                map.Switch8kPrgRom((((bank == 6) ? 0x1f : 0x0f) | (bank << 4)) * 2, 3);
            }
        }
        void SetCHR()
        {
            if ((reg[0] & 0x80) == 0x80)
            {
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr4 : 0x7f & chr4) | (bank << 7), 0);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr5 : 0x7f & chr5) | (bank << 7), 1);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr6 : 0x7f & chr6) | (bank << 7), 2);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr7 : 0x7f & chr7) | (bank << 7), 3);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr01 : 0x7f & chr01) | (bank << 7), 4);
                map.Switch1kChrRom(((bank == 6) ? 0xff & (chr01 + 1) : 0x7f & (chr01 + 1)) | (bank << 7), 5);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr23 : 0x7f & chr23) | (bank << 7), 6);
                map.Switch1kChrRom(((bank == 6) ? 0xff & (chr23 + 1) : 0x7f & (chr23 + 1)) | (bank << 7), 7);
            }
            else
            {
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr01 : 0x7f & chr01) | (bank << 7), 0);
                map.Switch1kChrRom(((bank == 6) ? 0xff & (chr01 + 1) : 0x7f & (chr01 + 1)) | (bank << 7), 1);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr23 : 0x7f & chr23) | (bank << 7), 2);
                map.Switch1kChrRom(((bank == 6) ? 0xff & (chr23 + 1) : 0x7f & (chr23 + 1)) | (bank << 7), 3);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr4 : 0x7f & chr4) | (bank << 7), 4);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr5 : 0x7f & chr5) | (bank << 7), 5);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr6 : 0x7f & chr6) | (bank << 7), 6);
                map.Switch1kChrRom(((bank == 6) ? 0xff & chr7 : 0x7f & chr7) | (bank << 7), 7);
            }
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte((byte)((bank & 0xFF000000) >> 24));
            stream.WriteByte((byte)((bank & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((bank & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((bank & 0x000000FF)));

            stream.WriteByte((byte)((prg0 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((prg0 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((prg0 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((prg0 & 0x000000FF)));

            stream.WriteByte((byte)((prg1 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((prg1 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((prg1 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((prg1 & 0x000000FF)));

            stream.WriteByte((byte)((chr01 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((chr01 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((chr01 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((chr01 & 0x000000FF)));

            stream.WriteByte((byte)((chr23 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((chr23 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((chr23 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((chr23 & 0x000000FF)));

            stream.WriteByte((byte)((chr4 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((chr4 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((chr4 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((chr4 & 0x000000FF)));

            stream.WriteByte((byte)((chr5 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((chr5 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((chr5 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((chr5 & 0x000000FF)));

            stream.WriteByte((byte)((chr6 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((chr6 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((chr6 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((chr6 & 0x000000FF)));

            stream.WriteByte((byte)((chr7 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((chr7 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((chr7 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((chr7 & 0x000000FF)));

            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));

            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));

            stream.WriteByte((byte)((irq_latch & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_latch & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_latch & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_latch & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            bank = (int)(stream.ReadByte() << 24);
            bank |= (int)(stream.ReadByte() << 16);
            bank |= (int)(stream.ReadByte() << 8);
            bank |= stream.ReadByte();
            prg0 = (int)(stream.ReadByte() << 24);
            prg0 |= (int)(stream.ReadByte() << 16);
            prg0 |= (int)(stream.ReadByte() << 8);
            prg0 |= stream.ReadByte();
            prg1 = (int)(stream.ReadByte() << 24);
            prg1 |= (int)(stream.ReadByte() << 16);
            prg1 |= (int)(stream.ReadByte() << 8);
            prg1 |= stream.ReadByte();
            chr01 = (int)(stream.ReadByte() << 24);
            chr01 |= (int)(stream.ReadByte() << 16);
            chr01 |= (int)(stream.ReadByte() << 8);
            chr01 |= stream.ReadByte();
            chr23 = (int)(stream.ReadByte() << 24);
            chr23 |= (int)(stream.ReadByte() << 16);
            chr23 |= (int)(stream.ReadByte() << 8);
            chr23 |= stream.ReadByte();
            chr4 = (int)(stream.ReadByte() << 24);
            chr4 |= (int)(stream.ReadByte() << 16);
            chr4 |= (int)(stream.ReadByte() << 8);
            chr4 |= stream.ReadByte();
            chr5 = (int)(stream.ReadByte() << 24);
            chr5 |= (int)(stream.ReadByte() << 16);
            chr5 |= (int)(stream.ReadByte() << 8);
            chr5 |= stream.ReadByte();
            chr6 = (int)(stream.ReadByte() << 24);
            chr6 |= (int)(stream.ReadByte() << 16);
            chr6 |= (int)(stream.ReadByte() << 8);
            chr6 |= stream.ReadByte();
            chr7 = (int)(stream.ReadByte() << 24);
            chr7 |= (int)(stream.ReadByte() << 16);
            chr7 |= (int)(stream.ReadByte() << 8);
            chr7 |= stream.ReadByte();
            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();
            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
            irq_latch = (int)(stream.ReadByte() << 24);
            irq_latch |= (int)(stream.ReadByte() << 16);
            irq_latch |= (int)(stream.ReadByte() << 8);
            irq_latch |= stream.ReadByte();
        }
    }
}
