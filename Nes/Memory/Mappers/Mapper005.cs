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
   
    class Mapper05 : IMapper
    {
        CPUMemory map;
        const byte EXRAMTABLE = 2;
        const byte FILLMODETABLE = 3;
        byte prgBankSize = 3;
        byte chrBankSize = 3;
        byte PRGRAMprotect1 = 0;
        byte PRGRAMprotect2 = 0;
        byte ExtendedRAMmode = 0;
        int IRQStatus = 0;
        int irq_scanline = 0;
        int irq_line = 0;
        int irq_clear = 0;
        int irq_enable = 0;

        byte Multiplier_A = 0;
        byte Multiplier_B = 0;

        int split_scroll = 0;
        int split_control = 0;
        int split_page = 0;

        public Mapper05(CPUMemory MAP)
        { map = MAP; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                //AUDIO
                case 0x5000: ((Mmc5ExternalComponent)map.apu.External).ChannelSq1.Poke1(0x5000, data); break;
                //case 0x5001:
                case 0x5002: ((Mmc5ExternalComponent)map.apu.External).ChannelSq1.Poke3(0x5002, data); break;
                case 0x5003: ((Mmc5ExternalComponent)map.apu.External).ChannelSq1.Poke4(0x5003, data); break;
                case 0x5004: ((Mmc5ExternalComponent)map.apu.External).ChannelSq2.Poke1(0x5004, data); break;
                //case 0x5005:
                case 0x5006: ((Mmc5ExternalComponent)map.apu.External).ChannelSq2.Poke3(0x5006, data); break;
                case 0x5007: ((Mmc5ExternalComponent)map.apu.External).ChannelSq2.Poke4(0x5007, data); break;
           //     case 0x5010: break;//??!!
                case 0x5011: ((Mmc5ExternalComponent)map.apu.External).ChannelPcm.Poke1(0x5011, data); break;
                case 0x5015: map.apu.Poke5015(data); break;
                //PRG mode ($5100) : 0 = 32k, 1 = 16k, 2 = 16k/8k, 3 = 8k
                case 0x5100: prgBankSize = (byte)(data & 0x3); break;
                //CHR mode ($5101) : 0 = 8k, 1 = 4k, 2 = 2k, 3 = 1k
                case 0x5101: chrBankSize = (byte)(data & 0x3); break;
                //PRG RAM Protect  ?
                case 0x5102: PRGRAMprotect1 = (byte)(data & 0x3); break;
                case 0x5103: PRGRAMprotect2 = (byte)(data & 0x3); break;
                //Extended RAM mode 
                case 0x5104:
                    ExtendedRAMmode = (byte)(data & 0x3);
                    map.ppu.EXRAMMODE = (byte)(data & 0x3);
                    break;
                //Nametable mapping
                case 0x5105:
                    map.SwitchVRAM((byte)(data & 0x3), 0);
                    map.SwitchVRAM((byte)((data & 0xC) >> 2), 1);
                    map.SwitchVRAM((byte)((data & 0x30) >> 4), 2);
                    map.SwitchVRAM((byte)((data & 0xC0) >> 6), 3);
                    break;
                //Fill-mode tile
                case 0x5106:
                    for (int i = 0; i < 0x3C0; i++)
                        map.ppu.MEM_PPU.NameTables[FILLMODETABLE][i] = data;
                    break;
                //Fill-mode color
                case 0x5107:
                    for (int i = 0x3C0; i < (0x3C0 + 0x40); i++)
                    {
                        byte value = (byte)((2 << (data & 0x03)) | (data & 0x03));
                        value |= (byte)((value & 0x0F) << 4);
                        map.ppu.MEM_PPU.NameTables[FILLMODETABLE][i] = value;
                    }
                    break;
                //PRG RAM bank
                case 0x5113:
                    map.Switch8kSRAM(data & 0x7);
                    break;
                //PRG bank 0
                case 0x5114:
                    if ((data & 0x80) == 0x80)
                    {
                        if (prgBankSize == 3)
                            map.Switch8kPrgRom((data & 0x7F) * 2, 0);
                    }
                    else if (prgBankSize == 3)
                    {
                        map.Switch8kSRAMToPRG((data & 0x07) * 2, 0);
                    }

                    break;
                //PRG bank 1
                case 0x5115:
                    if ((data & 0x80) == 0x80)
                    {
                        if (prgBankSize == 1)
                            map.Switch16kPrgRom(((data & 0x7E) >> 1) * 4, 0);
                        else if (prgBankSize == 2)
                            map.Switch16kPrgRom(((data & 0x7E) >> 1) * 4, 0);
                        else if (prgBankSize == 3)
                            map.Switch8kPrgRom((data & 0x7F) * 2, 1);
                    }
                    else
                    {
                        if (prgBankSize == 1 || prgBankSize == 2)
                        {
                            map.Switch8kSRAMToPRG(((data & 0x06) + 0) * 2, 0);
                            map.Switch8kSRAMToPRG(((data & 0x06) + 1) * 2, 1);
                        }
                        else if (prgBankSize == 3)
                        {
                            map.Switch8kSRAMToPRG((data & 0x07) * 2, 1);
                        }
                    }
                    break;
                //PRG bank 2 
                case 0x5116:
                    if ((data & 0x80) == 0x80)
                    {
                        if (prgBankSize == 2)
                            map.Switch8kPrgRom((data & 0x7f) * 2, 2);
                        else if (prgBankSize == 3)
                            map.Switch8kPrgRom((data & 0x7f) * 2, 2);
                    }
                    else if (prgBankSize == 2 || prgBankSize == 3)
                    {
                        map.Switch8kSRAMToPRG((data & 0x07) * 2, 1);
                    }
                    break;
                //PRG bank 3
                case 0x5117:
                    if (prgBankSize == 0)
                        map.Switch32kPrgRom(((data & 0x7C) >> 2) * 8);
                    else if (prgBankSize == 1)
                        map.Switch16kPrgRom(((data & 0x7E) >> 1) * 4, 1);
                    else if (prgBankSize == 2)
                        map.Switch8kPrgRom((data & 0x7f) * 2, 3);
                    else if (prgBankSize == 3)
                        map.Switch8kPrgRom((data & 0x7f) * 2, 3);
                    break;
                //Sprite CHR bank 0
                case 0x5120:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 0);
                    break;
                //Sprite CHR bank 1
                case 0x5121:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 1);
                    else if (chrBankSize == 2)
                        map.Switch2kChrRom(data * 2, 0);
                    break;
                //Sprite CHR bank 2
                case 0x5122:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 2);
                    break;
                //Sprite CHR bank 3
                case 0x5123:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 3);
                    else if (chrBankSize == 2)
                        map.Switch2kChrRom(data * 2, 1);
                    else if (chrBankSize == 1)
                        map.Switch4kChrRom(data * 4, 0);
                    break;
                //Sprite CHR bank 4
                case 0x5124:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 4);
                    break;
                //Sprite CHR bank 5
                case 0x5125:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 5);
                    else if (chrBankSize == 2)
                        map.Switch2kChrRom(data * 2, 2);
                    break;
                //Sprite CHR bank 6
                case 0x5126:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 6);
                    break;
                //Sprite CHR bank 7
                case 0x5127:
                    if (chrBankSize == 3)
                        map.Switch1kChrRom(data, 7);
                    else if (chrBankSize == 2)
                        map.Switch2kChrRom(data * 2, 3);
                    else if (chrBankSize == 1)
                        map.Switch4kChrRom(data * 4, 1);
                    else if (chrBankSize == 0)
                        map.Switch8kChrRom(data * 8);
                    break;

                //Background CHR bank 0
                case 0x5128:
                    map.Switch1kBGChrRom(data, 0);
                    map.Switch1kBGChrRom(data, 4);
                    break;
                //Background CHR bank 1
                case 0x5129:
                    if (chrBankSize == 3)
                    {
                        map.Switch1kBGChrRom(data, 1);
                        map.Switch1kBGChrRom(data, 5);
                    }
                    else if (chrBankSize == 2)
                    {
                        map.Switch2kBGChrRom(data * 2, 0);
                        map.Switch2kBGChrRom(data * 2, 2);
                    }
                    break;
                //Background CHR bank 2
                case 0x512A:
                    if (chrBankSize == 3)
                    {
                        map.Switch1kBGChrRom(data, 2);
                        map.Switch1kBGChrRom(data, 6);
                    }
                    break;
                //Background CHR bank 2
                case 0x512B:
                    if (chrBankSize == 3)
                    {
                        map.Switch1kBGChrRom(data, 3);
                        map.Switch1kBGChrRom(data, 7);
                    }
                    else if (chrBankSize == 2)
                    {
                        map.Switch2kBGChrRom(data * 2, 1);
                        map.Switch2kBGChrRom(data * 2, 3);
                    }
                    else if (chrBankSize == 1)
                    {
                        map.Switch4kBGChrRom(data * 4, 0);
                        map.Switch4kBGChrRom(data * 4, 1);
                    }
                    else if (chrBankSize == 0)
                    {
                        map.Switch8kBGChrRom(data * 8);
                    }
                    break;
                //Vertical Split Mode
                case 0x5200:
                    split_control = data;
                    break;
                //Vertical Split Scroll  
                case 0x5201:
                    split_scroll = data;
                    break;
                //Vertical Split Bank
                case 0x5202:
                    split_page = data & 0x3F;
                    break;
                //IRQ Counter
                case 0x5203: irq_line = data; map.cpu.SetIRQ(false); break;
                //IRQ Status 
                case 0x5204: irq_enable = data; map.cpu.SetIRQ(false); break;
                //Multiplier a
                case 0x5205: Multiplier_A = data; break;
                //Multiplier b
                case 0x5206: Multiplier_B = data; break;

                default:
                    if (address >= 0x5C00 && address <= 0x5FFF)
                    {
                        if (ExtendedRAMmode == 2)
                        {
                            map.ppu.MEM_PPU.NameTables[EXRAMTABLE][(ushort)(address & 0x3FF)] = data;
                        }
                        else if (ExtendedRAMmode != 3)
                        {
                            if ((IRQStatus & 0x40) == 0x40)
                            {
                                map.ppu.MEM_PPU.NameTables[EXRAMTABLE][(ushort)(address & 0x3FF)] = data;
                            }
                            else
                            {
                                map.ppu.MEM_PPU.NameTables[EXRAMTABLE][(ushort)(address & 0x3FF)] = 0;
                            }
                        }
                    }
                    else if (address >= 0x6000 && address <= 0x7FFF)
                    {
                        if ((PRGRAMprotect1 == 0x02) && (PRGRAMprotect2 == 0x01))
                            map.SRAM[map.SRAM_PAGE][address & 0x1FFF] = data;
                    }
                    break;
            }
        }
        public byte Read(ushort Address)
        {
            byte data = 0;
            switch (Address)
            {
                case 0x5015:
                    data = map.apu.Peek5015();
                    break;
                case 0x5204:
                    data = (byte)IRQStatus;
                    IRQStatus &= ~0x80;
                    map.cpu.SetIRQ(false);
                    break;
                case 0x5205:
                    data = (byte)(Multiplier_A * Multiplier_B);
                    break;
                case 0x5206:
                    data = (byte)((Multiplier_A * Multiplier_B) >> 8);
                    break;
            }
            if (Address >= 0x5C00 && Address <= 0x5FFF)
            {
                if (ExtendedRAMmode >= 2)
                {
                    data = map.ppu.MEM_PPU.NameTables[EXRAMTABLE][(ushort)(Address & 0x3FF)];
                }
            }
            return data;
        }
        public void SetUpMapperDefaults()
        {
            map.NES.APU.External = new Mmc5ExternalComponent(map.NES);
            map.Switch8kPrgRom((map.cartridge.PRG_PAGES * 4) - 2, 0);
            map.Switch8kPrgRom((map.cartridge.PRG_PAGES * 4) - 2, 1);
            map.Switch8kPrgRom((map.cartridge.PRG_PAGES * 4) - 2, 2);
            map.Switch8kPrgRom((map.cartridge.PRG_PAGES * 4) - 2, 3);
            map.Switch8kSRAM(0);
            map.IsSRAMReadOnly = true;//make it read only, this mapper will access the s-ram
            map.ppu.IsBGCHR = true;//Use defferent chr indexes for background 
            map.ppu.IsMapper5CHR = true;//change the render method
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
            map.Switch8kBGChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (map.ppu.BackgroundVisibility | map.ppu.SpriteVisibility)
            {
                if (map.ppu.ScanLine >= map.ppu.ScanlineOfEndOfVblank & map.ppu.ScanLine <= 239 + map.ppu.ScanlineOfEndOfVblank)
                {
                    irq_scanline++;
                    IRQStatus |= 0x40;
                    irq_clear = 0;
                }
            }

            if (irq_scanline == irq_line)
            {
                IRQStatus |= 0x80;
            }

            if (++irq_clear > 2)
            {
                irq_scanline = 0;
                IRQStatus &= ~0x80;
                IRQStatus &= ~0x40;

                map.cpu.SetIRQ(false);
            }

            if ((irq_enable & 0x80) == 0x80 && (IRQStatus & 0x80) == 0x80 && (IRQStatus & 0x40) == 0x40)
                map.cpu.SetIRQ(true);
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
            get { return true; }
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return true; }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.WriteByte(prgBankSize); 
            stream.WriteByte(chrBankSize); 
            stream.WriteByte(PRGRAMprotect1); 
            stream.WriteByte(PRGRAMprotect2);
            stream.WriteByte(ExtendedRAMmode);

            stream.WriteByte((byte)((IRQStatus & 0xFF000000) >> 24));
            stream.WriteByte((byte)((IRQStatus & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((IRQStatus & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((IRQStatus & 0x000000FF)));

            stream.WriteByte((byte)((irq_scanline & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_scanline & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_scanline & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_scanline & 0x000000FF)));

            stream.WriteByte((byte)((irq_line & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_line & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_line & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_line & 0x000000FF)));

            stream.WriteByte((byte)((irq_clear & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_clear & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_clear & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_clear & 0x000000FF)));

            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));

            stream.WriteByte(Multiplier_A);
            stream.WriteByte(Multiplier_B);

            stream.WriteByte((byte)((split_scroll & 0xFF000000) >> 24));
            stream.WriteByte((byte)((split_scroll & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((split_scroll & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((split_scroll & 0x000000FF)));

            stream.WriteByte((byte)((split_control & 0xFF000000) >> 24));
            stream.WriteByte((byte)((split_control & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((split_control & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((split_control & 0x000000FF)));

            stream.WriteByte((byte)((split_page & 0xFF000000) >> 24));
            stream.WriteByte((byte)((split_page & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((split_page & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((split_page & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            prgBankSize = (byte)stream.ReadByte();
            chrBankSize = (byte)stream.ReadByte();
            PRGRAMprotect1 = (byte)stream.ReadByte();
            PRGRAMprotect2 = (byte)stream.ReadByte();
            ExtendedRAMmode = (byte)stream.ReadByte();

            IRQStatus = (int)(stream.ReadByte() << 24);
            IRQStatus |= (int)(stream.ReadByte() << 16);
            IRQStatus |= (int)(stream.ReadByte() << 8);
            IRQStatus |= stream.ReadByte();
            irq_scanline = (int)(stream.ReadByte() << 24);
            irq_scanline |= (int)(stream.ReadByte() << 16);
            irq_scanline |= (int)(stream.ReadByte() << 8);
            irq_scanline |= stream.ReadByte();
            irq_line = (int)(stream.ReadByte() << 24);
            irq_line |= (int)(stream.ReadByte() << 16);
            irq_line |= (int)(stream.ReadByte() << 8);
            irq_line |= stream.ReadByte();
            irq_clear = (int)(stream.ReadByte() << 24);
            irq_clear |= (int)(stream.ReadByte() << 16);
            irq_clear |= (int)(stream.ReadByte() << 8);
            irq_clear |= stream.ReadByte();
            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();
            Multiplier_A = (byte)stream.ReadByte();
            Multiplier_B = (byte)stream.ReadByte();
            split_scroll = (int)(stream.ReadByte() << 24);
            split_scroll |= (int)(stream.ReadByte() << 16);
            split_scroll |= (int)(stream.ReadByte() << 8);
            split_scroll |= stream.ReadByte();
            split_control = (int)(stream.ReadByte() << 24);
            split_control |= (int)(stream.ReadByte() << 16);
            split_control |= (int)(stream.ReadByte() << 8);
            split_control |= stream.ReadByte();
            split_page = (int)(stream.ReadByte() << 24);
            split_page |= (int)(stream.ReadByte() << 16);
            split_page |= (int)(stream.ReadByte() << 8);
            split_page |= stream.ReadByte();
        }
    }
}
