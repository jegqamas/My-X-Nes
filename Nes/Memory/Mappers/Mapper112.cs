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

    class Mapper112 : IMapper
    {
        CPUMemory MEM;
        byte[] reg = new byte[4];
        int prg0 = 0;
        int prg1 = 1;
        byte chr01 = 0;
        byte chr23 = 2;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;
        public Mapper112(CPUMemory mem)
        {
            MEM = mem;
        }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                    reg[0] = data;
                    SetPRG();
                    SetCHR();
                    break;
                case 0xA000:
                    reg[1] = data;
                    switch (reg[0] & 0x07)
                    {
                        case 0x00:
                            prg0 = (data & ((MEM.cartridge.PRG_PAGES * 4) - 1));
                            SetPRG();
                            break;
                        case 0x01:
                            prg1 = (data & ((MEM.cartridge.PRG_PAGES * 4) - 1));
                            SetPRG();
                            break;
                        case 0x02:
                            chr01 = (byte)(data & 0xFE);
                            SetCHR();
                            break;
                        case 0x03:
                            chr23 = (byte)(data & 0xFE);
                            SetCHR();
                            break;
                        case 0x04:
                            chr4 = data;
                            SetCHR();
                            break;
                        case 0x05:
                            chr5 = data;
                            SetCHR();
                            break;
                        case 0x06:
                            chr6 = data;
                            SetCHR();
                            break;
                        case 0x07:
                            chr7 = data;
                            SetCHR();
                            break;
                    }
                    break;

                case 0xC000:
                    reg[3] = data;
                    SetCHR();
                    break;

                case 0xE000:
                    reg[2] = data;
                    if (MEM.cartridge.Mirroring != Mirroring.Four_Screen)
                    {
                        if ((data & 0x01) != 0)
                            MEM.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            MEM.cartridge.Mirroring = Mirroring.Vertical;
                    }
                    SetCHR();
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            SetPRG();
            if (MEM.cartridge.IsVRAM)
                MEM.FillCHR(16);
            SetCHR();
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {

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
        void SetPRG()
        {
            MEM.Switch8kPrgRom(prg0 * 2, 0);
            MEM.Switch8kPrgRom(prg1 * 2, 1);
            MEM.Switch8kPrgRom(((MEM.cartridge.PRG_PAGES * 2) - 2) * 2, 2);
            MEM.Switch8kPrgRom(((MEM.cartridge.PRG_PAGES * 2) - 1) * 2, 3);

        }
        void SetCHR()
        {
            if ((reg[2] & 0x02) != 0)
            {
                MEM.Switch1kChrRom(chr01, 0);
                MEM.Switch1kChrRom(chr01 + 1, 1);
                MEM.Switch1kChrRom(chr23, 2);
                MEM.Switch1kChrRom(chr23 + 1, 3);
                MEM.Switch1kChrRom(chr4, 4);
                MEM.Switch1kChrRom(chr5, 5);
                MEM.Switch1kChrRom(chr6, 6);
                MEM.Switch1kChrRom(chr7, 7);
            }
            else
            {
                MEM.Switch1kChrRom(((reg[3] << 6) & 0x100) + chr01, 0);
                MEM.Switch1kChrRom(((reg[3] << 6) & 0x100) + chr01 + 1, 1);
                MEM.Switch1kChrRom(((reg[3] << 5) & 0x100) + chr23, 2);
                MEM.Switch1kChrRom(((reg[3] << 5) & 0x100) + chr23 + 1, 3);
                MEM.Switch1kChrRom(((reg[3] << 4) & 0x100) + chr4, 4);
                MEM.Switch1kChrRom(((reg[3] << 3) & 0x100) + chr5, 5);
                MEM.Switch1kChrRom(((reg[3] << 2) & 0x100) + chr6, 6);
                MEM.Switch1kChrRom(((reg[3] << 1) & 0x100) + chr7, 7);
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
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte((byte)((prg0 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((prg0 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((prg0 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((prg0 & 0x000000FF)));
            stream.WriteByte((byte)((prg1 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((prg1 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((prg1 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((prg1 & 0x000000FF)));
            stream.WriteByte(chr01);
            stream.WriteByte(chr23);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
            stream.WriteByte(chr6);
            stream.WriteByte(chr7);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            prg0 = (int)(stream.ReadByte() << 24);
            prg0 |= (int)(stream.ReadByte() << 16);
            prg0 |= (int)(stream.ReadByte() << 8);
            prg0 |= stream.ReadByte();

            prg1 = (int)(stream.ReadByte() << 24);
            prg1 |= (int)(stream.ReadByte() << 16);
            prg1 |= (int)(stream.ReadByte() << 8);
            prg1 |= stream.ReadByte();
            chr01 = (byte)stream.ReadByte();
            chr23 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
            chr6 = (byte)stream.ReadByte();
            chr7 = (byte)stream.ReadByte();
        }
    }
}
