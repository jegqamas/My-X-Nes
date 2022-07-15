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
   
    class Mapper68 : IMapper
    {
        CPUMemory Map;
        byte[] reg = new byte[4];
        public Mapper68(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000:
                    Map.Switch2kChrRom(data * 2, 0);
                    break;
                case 0x9000:
                    Map.Switch2kChrRom(data * 2, 1);
                    break;
                case 0xA000:
                    Map.Switch2kChrRom(data * 2, 2);
                    break;
                case 0xB000:
                    Map.Switch2kChrRom(data * 2, 3);
                    break;

                case 0xC000:
                    reg[2] = data;
                    SetBank();
                    break;
                case 0xD000:
                    reg[3] = data;
                    SetBank();
                    break;
                case 0xE000:
                    reg[0] = (byte)((data & 0x10) >> 4);
                    reg[1] = (byte)(data & 0x03);
                    SetBank();
                    break;

                case 0xF000:
                    Map.Switch16kPrgRom(data * 4, 0);
                    break;
            }
        }

        public byte Read(ushort Address)
        {
            return 0;
        }
        void SetBank()
        {
            if (reg[0] != 0)
            {
                switch (reg[1])
                {
                    case 0:
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 0);
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 1);
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 2);
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 3);
                        break;
                    case 1:
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 0);
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 1);
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 2);
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 3);
                        break;
                    case 2:
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 0);
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 1);
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 2);
                        Map.Switch1kCHRToVRAM(reg[2] + 0x80, 3);
                        break;
                    case 3:
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 0);
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 1);
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 2);
                        Map.Switch1kCHRToVRAM(reg[3] + 0x80, 3);
                        break;
                }
            }
            else
            {
                switch (reg[1])
                {
                    case 0:
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                        break;
                    case 1:
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                        break;
                    case 2:
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        Map.cartridge.MirroringBase = 0x2000;
                        break;
                    case 3:
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        Map.cartridge.MirroringBase = 0x2400;
                        break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.cartridge.CHR_PAGES - 1) * 4, 1);
            Map.FillCRAM(32);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
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
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }
        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
        }
    }
}
