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
   
    class Mapper51 : IMapper
    {
        int bank = 0;
        int mode = 1;
        CPUMemory map;
        public Mapper51(CPUMemory MAp)
        { map = MAp; }
        public void Write(ushort address, byte data)
        {
            if (address < 08000)
            {
                mode = ((data & 0x10) >> 3) | ((data & 0x02) >> 1);
                SetBank_CPU();
            }
            else
            {
                bank = (data & 0x0f) << 2;
                if (0xC000 <= address && address <= 0xDFFF)
                {
                    mode = (mode & 0x01) | ((data & 0x10) >> 3);
                }
                SetBank_CPU();
            }
        }

        public byte Read(ushort Address)
        {
            return 0;
        }

        public void SetUpMapperDefaults()
        {
            SetBank_CPU();
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
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
            switch (mode)
            {
                case 0:
                    map.cartridge.Mirroring = Mirroring.Vertical;
                    map.Switch8kPrgRomToSRAM( (bank | 0x2c | 3)*2);
                    map.Switch8kPrgRom((bank | 0x00 | 0) * 2, 0);
                    map.Switch8kPrgRom((bank | 0x00 | 1) * 2, 1);
                    map.Switch8kPrgRom((bank | 0x0c | 2) * 2, 2);
                    map.Switch8kPrgRom((bank | 0x0c | 3) * 2, 3);
                    break;
                case 1:
                    map.cartridge.Mirroring = Mirroring.Vertical;
                    map.Switch8kPrgRomToSRAM((bank | 0x20 | 3) * 2);
                    map.Switch8kPrgRom((bank | 0x00 | 0) * 2, 0);
                    map.Switch8kPrgRom((bank | 0x00 | 1) * 2, 1);
                    map.Switch8kPrgRom((bank | 0x00 | 2) * 2, 2);
                    map.Switch8kPrgRom((bank | 0x00 | 3) * 2, 3);
                    break;
                case 2:
                    map.cartridge.Mirroring = Mirroring.Vertical;
                    map.Switch8kPrgRomToSRAM( (bank | 0x2e | 3) * 2);
                    map.Switch8kPrgRom((bank | 0x02 | 0) * 2, 0);
                    map.Switch8kPrgRom((bank | 0x02 | 1) * 2, 1);
                    map.Switch8kPrgRom((bank | 0x0e | 2) * 2, 2);
                    map.Switch8kPrgRom((bank | 0x0e | 3) * 2, 3);
                    break;
                case 3:
                    map.cartridge.Mirroring = Mirroring.Horizontal;
                    map.Switch8kPrgRomToSRAM( (bank | 0x20 | 3) * 2);
                    map.Switch8kPrgRom((bank | 0x00 | 0) * 2, 0);
                    map.Switch8kPrgRom((bank | 0x00 | 1) * 2, 1);
                    map.Switch8kPrgRom((bank | 0x00 | 2) * 2, 2);
                    map.Switch8kPrgRom((bank | 0x00 | 3) * 2, 3);
                    break;
            }
        }

        public void SaveState(System.IO.Stream stream)
        {

            stream.WriteByte((byte)((bank & 0xFF000000) >> 24));
            stream.WriteByte((byte)((bank & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((bank & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((bank & 0x000000FF)));

            stream.WriteByte((byte)((mode & 0xFF000000) >> 24));
            stream.WriteByte((byte)((mode & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((mode & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((mode & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            bank = (int)(stream.ReadByte() << 24);
            bank |= (int)(stream.ReadByte() << 16);
            bank |= (int)(stream.ReadByte() << 8);
            bank |= stream.ReadByte();

            mode = (int)(stream.ReadByte() << 24);
            mode |= (int)(stream.ReadByte() << 16);
            mode |= (int)(stream.ReadByte() << 8);
            mode |= stream.ReadByte();
        }
    }
}
