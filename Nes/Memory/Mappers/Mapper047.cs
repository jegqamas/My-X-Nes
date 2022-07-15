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

    class Mapper47 : IMapper
    {
        /*compatible with "Super Spike V'Ball + Nintendo World Cup" only*/
        CPUMemory map;
        public Mapper47(CPUMemory MAP)
        { map = MAP; }
        byte[] reg = new byte[8];
        byte bank = 0;
        byte prg0 = 0;
        byte prg1 = 1;
        byte chr01 = 0;
        byte chr23 = 2;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;
        byte irq_enable = 0;
        byte irq_counter = 0;
        byte irq_latch = 0;
        public void Write(ushort address, byte data)
        {
            if (address == 0x6000)
            {
                bank = (byte)((data & 0x01) << 1);
                SetBank_CPU();
                SetBank_PPU();
            }
            else if (address >= 0x8000)
            {
                switch (address & 0xE001)
                {
                    case 0x8000:
                        reg[0] = data;
                        SetBank_CPU();
                        SetBank_PPU();
                        break;
                    case 0x8001:
                        reg[1] = data;
                        switch (reg[0] & 0x07)
                        {
                            case 0x00:
                                chr01 = (byte)(data & 0xFE);
                                SetBank_PPU();
                                break;
                            case 0x01:
                                chr23 = (byte)(data & 0xFE);
                                SetBank_PPU();
                                break;
                            case 0x02:
                                chr4 = data;
                                SetBank_PPU();
                                break;
                            case 0x03:
                                chr5 = data;
                                SetBank_PPU();
                                break;
                            case 0x04:
                                chr6 = data;
                                SetBank_PPU();
                                break;
                            case 0x05:
                                chr7 = data;
                                SetBank_PPU();
                                break;
                            case 0x06:
                                prg0 = data;
                                SetBank_CPU();
                                break;
                            case 0x07:
                                prg1 = data;
                                SetBank_CPU();
                                break;
                        }
                        break;
                    case 0xA000:
                        reg[2] = data;
                        if ((data & 0x01) == 0x01)
                            map.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            map.cartridge.Mirroring = Mirroring.Vertical;
                        break;
                    case 0xA001:
                        reg[3] = data;
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
        }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public void SetUpMapperDefaults()
        {
            if (!map.cartridge.IsVRAM)
            {
                chr01 = 0;
                chr23 = 2;
                chr4 = 4;
                chr5 = 5;
                chr6 = 6;
                chr7 = 7;
            }
            else
            {
                chr01 = chr23 = chr4 = chr5 = chr6 = chr7 = 0;
            }
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            SetBank_CPU();
            SetBank_PPU();
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
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }
        void SetBank_CPU()
        {
            if ((reg[0] & 0x40) == 0x40)
            {
                map.Switch8kPrgRom((bank * 8 + 14) * 2, 0);
                map.Switch8kPrgRom((bank * 8 + prg1) * 2, 1);
                map.Switch8kPrgRom((bank * 8 + prg0) * 2, 2);
                map.Switch8kPrgRom((bank * 8 + 15) * 2, 3);
            }
            else
            {
                map.Switch8kPrgRom((bank * 8 + prg0) * 2, 0);
                map.Switch8kPrgRom((bank * 8 + prg1) * 2, 1);
                map.Switch8kPrgRom((bank * 8 + 14) * 2, 2);
                map.Switch8kPrgRom((bank * 8 + 15) * 2, 3);
            }
        }
        void SetBank_PPU()
        {
            if (!map.cartridge.IsVRAM)
            {
                if ((reg[0] & 0x80) == 0x80)
                {
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr4, 0);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr5, 1);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr6, 2);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr7, 3);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr01 + 0, 4);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr01 + 1, 5);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr23 + 0, 6);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr23 + 1, 7);
                }
                else
                {
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr01 + 0, 0);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr01 + 1, 1);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr23 + 0, 2);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr23 + 1, 3);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr4, 4);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr5, 5);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr6, 6);
                    map.Switch1kChrRom((bank & 0x02) * 64 + chr7, 7);
                }
            }
        }
        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte(bank);
            stream.WriteByte(prg0);
            stream.WriteByte(prg1);
            stream.WriteByte(chr01);
            stream.WriteByte(chr23);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
            stream.WriteByte(chr6);
            stream.WriteByte(chr7);
            stream.WriteByte(irq_enable);
            stream.WriteByte(irq_counter);
            stream.WriteByte(irq_latch);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            bank = (byte)stream.ReadByte();
            prg0 = (byte)stream.ReadByte();
            prg1 = (byte)stream.ReadByte();
            chr01 = (byte)stream.ReadByte();
            chr23 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
            chr6 = (byte)stream.ReadByte();
            chr7 = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_counter = (byte)stream.ReadByte();
            irq_latch = (byte)stream.ReadByte();
        }
    }
}
