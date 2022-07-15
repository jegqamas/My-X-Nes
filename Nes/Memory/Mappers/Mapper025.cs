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
    class Mapper25 : IMapper
    {
        CPUMemory _Map;
        byte[] reg = new byte[11];
        bool SwapMode = false;
        int irq_latch = 0;
        byte irq_enable = 0;
        int irq_counter = 0;
        int irq_clock = 0;

        public Mapper25(CPUMemory Map)
        {
            _Map = Map;
        }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000:
                    if ((reg[10] & 0x02) == 0x02)
                    {
                        reg[9] = data;
                        _Map.Switch8kPrgRom(data * 2, 2);
                    }
                    else
                    {
                        reg[8] = data;
                        _Map.Switch8kPrgRom(data * 2, 0);
                    }
                    break;
                case 0xA000:
                    _Map.Switch8kPrgRom(data * 2, 1);
                    break;
            }
            switch (address & 0xF00F)
            {
                /*Swap mode*/
                case 0x9001:
                case 0x9004:
                    if ((reg[10] & 0x02) != (data & 0x02))
                    {
                        byte swap = reg[8];
                        reg[8] = reg[9];
                        reg[9] = swap;
                        _Map.Switch8kPrgRom(reg[8] * 2, 0);
                        _Map.Switch8kPrgRom(reg[9] * 2, 2);
                    }
                    reg[10] = data;
                    break;
                /*Mirroring Control*/
                case 0x9000:
                    data &= 0x03;
                    if (data == 0)
                        _Map.cartridge.Mirroring = Mirroring.Vertical;
                    else if (data == 1)
                        _Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else if (data == 2)
                    {
                        _Map.cartridge.Mirroring = Mirroring.One_Screen;
                        _Map.cartridge.MirroringBase = 0x2000;
                    }
                    else
                    {
                        _Map.cartridge.Mirroring = Mirroring.One_Screen;
                        _Map.cartridge.MirroringBase = 0x2400;
                    }
                    break;
                /*CHR Selection*/
                case 0xB000:
                    reg[0] = (byte)((reg[0] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[0], 0);
                    break;
                case 0xB002:
                case 0xB008:
                    reg[0] = (byte)((reg[0] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[0], 0);
                    break;

                case 0xB001:
                case 0xB004:
                    reg[1] = (byte)((reg[1] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[1], 1);
                    break;
                case 0xB003:
                case 0xB00C:
                    reg[1] = (byte)((reg[1] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[1], 1);
                    break;

                case 0xC000:
                    reg[2] = (byte)((reg[2] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[2], 2);
                    break;
                case 0xC002:
                case 0xC008:
                    reg[2] = (byte)((reg[2] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[2], 2);
                    break;

                case 0xC001:
                case 0xC004:
                    reg[3] = (byte)((reg[3] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[3], 3);
                    break;
                case 0xC003:
                case 0xC00C:
                    reg[3] = (byte)((reg[3] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[3], 3);
                    break;

                case 0xD000:
                    reg[4] = (byte)((reg[4] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[4], 4);
                    break;
                case 0xD002:
                case 0xD008:
                    reg[4] = (byte)((reg[4] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[4], 4);
                    break;

                case 0xD001:
                case 0xD004:
                    reg[5] = (byte)((reg[5] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[5], 5);
                    break;
                case 0xD003:
                case 0xD00C:
                    reg[5] = (byte)((reg[5] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[5], 5);
                    break;

                case 0xE000:
                    reg[6] = (byte)((reg[6] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[6], 6);
                    break;
                case 0xE002:
                case 0xE008:
                    reg[6] = (byte)((reg[6] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[6], 6);
                    break;

                case 0xE001:
                case 0xE004:
                    reg[7] = (byte)((reg[7] & 0xF0) | (data & 0x0F));
                    _Map.Switch1kChrRom(reg[7], 7);
                    break;
                case 0xE003:
                case 0xE00C:
                    reg[7] = (byte)((reg[7] & 0x0F) | ((data & 0x0F) << 4));
                    _Map.Switch1kChrRom(reg[7], 7);
                    break;
                /*IRQs*/
                case 0xF000:
                    irq_latch = (irq_latch & 0xF0) | (data & 0x0F);
                    break;

                case 0xF002:
                case 0xF008:
                    irq_latch = (irq_latch & 0x0F) | ((data & 0x0F) << 4);
                    break;

                case 0xF001:
                case 0xF004:
                    irq_enable = (byte)(data & 0x03);
                    irq_counter = irq_latch;
                    irq_clock = 0;
                    break;

                case 0xF003:
                case 0xF00C:
                    irq_enable = (byte)((irq_enable & 0x01) * 3);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            _Map.Switch16kPrgRom(0, 0);
            _Map.Switch16kPrgRom((_Map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (_Map.cartridge.IsVRAM)
                _Map.FillCHR(16);
            reg[9] = (byte)((_Map.cartridge.PRG_PAGES * 2) - 2);
            //_Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {
            if ((irq_enable & 0x02) != 0)
            {
                irq_clock += cycles * 3;
                while (irq_clock >= 341)
                {
                    irq_clock -= 341;
                    irq_counter++;
                    if (irq_counter == 0xFF)
                    {
                        irq_counter = irq_latch;
                        _Map.cpu.SetIRQ(true);
                    }
                }
            }
        }
        public void SoftReset()
        {

        }
        public bool WriteUnder8000
        {
            get { return false; }
        }
        public bool WriteUnder6000
        {
            get { return false; }
        }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }
        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte((byte)((irq_latch & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_latch & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_latch & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_latch & 0x000000FF)));

            stream.WriteByte(irq_enable);

            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));
            stream.WriteByte((byte)((irq_clock & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_clock & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_clock & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_clock & 0x000000FF)));
            stream.WriteByte((byte)(SwapMode ? 1 : 0));
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            irq_latch = (int)(stream.ReadByte() << 24);
            irq_latch |= (int)(stream.ReadByte() << 16);
            irq_latch |= (int)(stream.ReadByte() << 8);
            irq_latch |= stream.ReadByte();

            irq_enable = (byte)stream.ReadByte();

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
            irq_clock = (int)(stream.ReadByte() << 24);
            irq_clock |= (int)(stream.ReadByte() << 16);
            irq_clock |= (int)(stream.ReadByte() << 8);
            irq_clock |= stream.ReadByte();

            SwapMode = stream.ReadByte() == 1;
        }
    }
}
