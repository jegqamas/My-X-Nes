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
    /// <summary>
    /// The nes.PPU memory
    /// </summary>

    public class PPUMemory
    {
        /// <summary>
        /// The nes.PPU memory
        /// </summary>
        /// <param name="NES">The nes</param>
        public PPUMemory(NES nes)
        {
            this.nes = nes;
            for (int i = 0; i < 4; i++)
                NameTables[i] = new byte[0x400];
        }
        NES nes;
        public byte[] NameTableIndexes = new byte[4];
        public byte[][] NameTables = new byte[4][];
        byte[] Palette = new byte[0x20];
        public byte[][] CRAM;
        /// <summary>
        /// The sprite-ram
        /// </summary>
        public byte[] SPR_RAM = new byte[256];
        /// <summary>
        /// Applay the cartidage mirroring
        /// </summary>
        public void ApplyMirroring()
        {
            if (nes.Memory.cartridge.Mirroring == Mirroring.Horizontal)
            {
                NameTableIndexes[0] = 0;
                NameTableIndexes[1] = 0;
                NameTableIndexes[2] = 1;
                NameTableIndexes[3] = 1;
            }
            else if (nes.Memory.cartridge.Mirroring == Mirroring.Vertical)
            {
                NameTableIndexes[0] = 0;
                NameTableIndexes[1] = 1;
                NameTableIndexes[2] = 0;
                NameTableIndexes[3] = 1;
            }
            else if (nes.Memory.cartridge.Mirroring == Mirroring.One_Screen)
            {
                if (nes.Memory.cartridge.MirroringBase == 0x2000)
                {
                    NameTableIndexes[0] = 0;
                    NameTableIndexes[1] = 0;
                    NameTableIndexes[2] = 0;
                    NameTableIndexes[3] = 0;
                }
                else if (nes.Memory.cartridge.MirroringBase == 0x2400)
                {
                    NameTableIndexes[0] = 1;
                    NameTableIndexes[1] = 1;
                    NameTableIndexes[2] = 1;
                    NameTableIndexes[3] = 1;
                }
            }
            else
            {
                NameTableIndexes[0] = 0;
                NameTableIndexes[1] = 1;
                NameTableIndexes[2] = 2;
                NameTableIndexes[3] = 3;
            }
        }
        public byte this[ushort Address]
        {
            get //Read 
            {
                /*Pattern Tables (CHR)*/
                if (Address < 0x2000)
                {
                    if (nes.Memory.cartridge.MAPPER == 9)
                        ((Mapper09)nes.Memory.MAPPER).CHRlatch(Address);
                    else if (nes.Memory.cartridge.MAPPER == 10)
                        ((Mapper10)nes.Memory.MAPPER).CHRlatch(Address);

                    if (nes.Memory.CHR_PAGE[(Address & 0x1C00) >> 10] < nes.Memory.cartridge.CHR.Length)
                        return nes.Memory.cartridge.CHR[nes.Memory.CHR_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF];
                    else
                        return CRAM[nes.Memory.CRAM_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF];
                }
                /*Name Tables*/
                else if (Address < 0x3F00)
                {
                    if (NameTableIndexes[(Address & 0x0C00) >> 10] < 4)
                        return NameTables[NameTableIndexes[(Address & 0x0C00) >> 10]][Address & 0x03FF];
                    else
                        return nes.Memory.cartridge.CHR[NameTableIndexes[(Address & 0x0C00) >> 10]][Address & 0x3FF];
                }
                /*Background and Sprite Palettes*/
                else
                {
                    Address &= 0x1F;
                    if ((Address & 0x03) == 0)
                        Address &= 0x0C;
                    return Palette[Address];
                }
            }
            set //Write
            {
                /*Pattern Tables (CHR)*/
                if (Address < 0x2000)
                {
                    if (nes.Memory.cartridge.IsVRAM)
                    {
                        nes.Memory.cartridge.CHR[nes.Memory.CHR_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF] = value;
                    }
                    else if (CRAM != null)
                    {
                        CRAM[nes.Memory.CRAM_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF] = value;
                    }
                }
                /*Name Tables*/
                else if (Address < 0x3F00)
                {
                    if (NameTableIndexes[(Address & 0x0C00) >> 10] < 4)//for mapper 19 switches
                        NameTables[NameTableIndexes[(Address & 0x0C00) >> 10]][Address & 0x03FF] = value;
                }
                /*Background and Sprite Palettes*/
                else
                {
                    Address &= 0x1F;
                    if ((Address & 0x03) == 0)
                        Address &= 0x0C;
                    Palette[Address] = (byte)(value & 0x3F);
                }
            }
        }
        public byte ReadBGCHR(ushort Address)
        {
            return nes.Memory.cartridge.CHR[nes.Memory.CHRBG_PAGE[(Address & 0x1C00) >> 10]][Address & 0x3FF];
        }
        public byte ReadCHRExtra(ushort address, int bank)
        {
            if (address < 0x1000)
                nes.Memory.Switch4kChrEXRom(bank * 4, 0);
            else
                nes.Memory.Switch4kChrEXRom(bank * 4, 1);
            return nes.Memory.cartridge.CHR[nes.Memory.CHREX_page[(address & 0x1C00) >> 10]][address & 0x3FF];
        }

        public void SaveState(System.IO.Stream stream)
        {
            for (int i = 0; i < 4; i++)
                stream.WriteByte(NameTableIndexes[i]);

            for (int i = 0; i < 4; i++)
                stream.Write(NameTables[i], 0, NameTables[i].Length);

            stream.Write(Palette, 0, Palette.Length);

            if (CRAM != null)
                for (int i = 0; i < CRAM.Length; i++)
                    stream.Write(CRAM[i], 0, CRAM[i].Length);

            stream.Write(SPR_RAM, 0, SPR_RAM.Length);
        }
        public void LoadState(System.IO.Stream stream)
        {
            for (int i = 0; i < 4; i++)
                NameTableIndexes[i] = (byte)stream.ReadByte();

            for (int i = 0; i < 4; i++)
                stream.Read(NameTables[i], 0, NameTables[i].Length);

            stream.Read(Palette, 0, Palette.Length);

            if (CRAM != null)
                for (int i = 0; i < CRAM.Length; i++)
                    stream.Read(CRAM[i], 0, CRAM[i].Length);

            stream.Read(SPR_RAM, 0, SPR_RAM.Length);
        }
    }
}