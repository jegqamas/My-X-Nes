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

    class Mapper205 : IMapper
    {
        CPUMemory map;
        byte commandNumber;
        byte prgAddressSelect;
        byte chrAddressSelect;
        byte irq_enabled;
        byte irq_count, irq_reload = 0xff;
        byte prg0 = 0;
        byte prg1 = 1;
        byte chr0, chr1, chr2, chr3, chr4, chr5 = 0;
        int mode = 0;

        public Mapper205(CPUMemory MAP)
        { map = MAP; }
        public void Write(ushort address, byte data)
        {
            if (address < 0x8000)
            {
                mode = data << 4 & 0x30;
                SetPRG(); SetCHR();
            }
            else
            {
                address &= 0xE001;
                if (address == 0x8000)
                {
                    commandNumber = (byte)(data & 0x7);
                    prgAddressSelect = (byte)(data & 0x40);
                    chrAddressSelect = (byte)(data & 0x80);
                    SetPRG(); SetCHR();
                }
                else if (address == 0x8001)
                {
                    if (commandNumber == 0)
                    {
                        chr0 = (byte)(data - (data % 2)); SetCHR();
                    }
                    else if (commandNumber == 1)
                    {
                        chr1 = (byte)(data - (data % 2)); SetCHR();
                    }
                    else if (commandNumber == 2)
                    {
                        chr2 = (byte)(data & (map.cartridge.CHR_PAGES * 8 - 1)); SetCHR();
                    }
                    else if (commandNumber == 3)
                    {
                        chr3 = data; SetCHR();
                    }
                    else if (commandNumber == 4)
                    {
                        chr4 = data; SetCHR();
                    }
                    else if (commandNumber == 5)
                    {
                        chr5 = data; SetCHR();
                    }
                    else if (commandNumber == 6)
                    {
                        prg0 = data; SetPRG();
                    }
                    else if (commandNumber == 7)
                    {
                        prg1 = data; SetPRG();
                    }
                }
                else if (address == 0xA000)
                {
                    if ((data & 0x1) == 0)
                    {
                        map.cartridge.Mirroring = Mirroring.Vertical;
                    }
                    else
                    {
                        map.cartridge.Mirroring = Mirroring.Horizontal;
                    }
                }
                else if (address == 0xA001)
                {
                    map.IsSRAMReadOnly = ((data & 0x80) == 0);
                }
                /*IRQ registers*/
                else if (address == 0xC000)
                {
                    irq_reload = data;
                }
                else if (address == 0xC001)
                {
                    //irq_count = irq_reload;
                    irq_count = 0;
                }
                else if (address == 0xE000)
                {
                    irq_enabled = 0;
                    map.cpu.SetIRQ(false);
                }
                else if (address == 0xE001)
                {
                    irq_enabled = 1;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            map.ppu.isMMC3IRQ = true;
            prgAddressSelect = 0;
            chrAddressSelect = 0;
            SetPRG();
            if (map.cartridge.IsVRAM)
                map.FillCHR(8);
            map.Switch4kChrRom(0, 0);

            CONSOLE.WriteLine(this,"Mapper 205 is unstable !!", DebugStatus.Warning);
        }
        public void TickScanlineTimer()
        {
            if (irq_count > 0)
                irq_count--;
            else
                irq_count = irq_reload;
            if (irq_count == 0)
            {
                if (irq_enabled == 1)
                {
                    map.cpu.SetIRQ(true);
                }
            }
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        { }
        public bool WriteUnder8000
        { get { return true; } }
        public bool WriteUnder6000
        { get { return true; } }

        void SetPRG()
        {
            int and = (mode & 0x20) == 0x20 ? 0x0F : 0x1F;

            if (prgAddressSelect != 0)
            {
                map.Switch8kPrgRom((mode | (and & (((map.cartridge.PRG_PAGES * 2) - 2)))) * 2, 0);
                map.Switch8kPrgRom((mode | (and & (prg1))) * 2, 1);
                map.Switch8kPrgRom((mode | (and & (prg0))) * 2, 2);
                map.Switch8kPrgRom((mode | (and & (((map.cartridge.PRG_PAGES * 2) - 1)))) * 2, 3);
            }
            else
            {
                map.Switch8kPrgRom((mode | (and & (prg0))) * 2, 0);
                map.Switch8kPrgRom((mode | (and & (prg1))) * 2, 1);
                map.Switch8kPrgRom((mode | ((and & ((map.cartridge.PRG_PAGES * 2) - 2)))) * 2, 2);
                map.Switch8kPrgRom((mode | (and & (((map.cartridge.PRG_PAGES * 2) - 1)))) * 2, 3);
            }
        }
        void SetCHR()
        {
            if (chrAddressSelect == 0)
            {
                int or = mode << 2;
                map.Switch2kChrRom(or | chr0, 0);
                map.Switch2kChrRom(or | chr1, 1);
                or <<= 1;
                map.Switch1kChrRom(or | chr2, 4);
                map.Switch1kChrRom(or | chr3, 5);
                map.Switch1kChrRom(or | chr4, 6);
                map.Switch1kChrRom(or | chr5, 7);
            }
            else
            {
                int or = mode << 2;
                map.Switch2kChrRom(or | chr0, 2);
                map.Switch2kChrRom(or | chr1, 3);
                or <<= 1;
                map.Switch1kChrRom(or | chr2, 0);
                map.Switch1kChrRom(or | chr3, 1);
                map.Switch1kChrRom(or | chr4, 2);
                map.Switch1kChrRom(or | chr5, 3);
            }
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
            stream.WriteByte(commandNumber);
            stream.WriteByte(prgAddressSelect);
            stream.WriteByte(chrAddressSelect);
            stream.WriteByte(irq_enabled);
            stream.WriteByte(irq_count);
            stream.WriteByte(irq_reload);
            stream.WriteByte(prg0);
            stream.WriteByte(prg1);
            stream.WriteByte(chr0);
            stream.WriteByte(chr1);
            stream.WriteByte(chr2);
            stream.WriteByte(chr3);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
        }
        public void LoadState(System.IO.Stream stream)
        {
            commandNumber = (byte)stream.ReadByte();
            prgAddressSelect = (byte)stream.ReadByte();
            chrAddressSelect = (byte)stream.ReadByte();
            irq_enabled = (byte)stream.ReadByte();
            irq_count = (byte)stream.ReadByte();
            irq_reload = (byte)stream.ReadByte();
            prg0 = (byte)stream.ReadByte();
            prg1 = (byte)stream.ReadByte();
            chr0 = (byte)stream.ReadByte();
            chr1 = (byte)stream.ReadByte();
            chr2 = (byte)stream.ReadByte();
            chr3 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
        }
    }
}
