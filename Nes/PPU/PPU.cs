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
using System.IO;
using MyNes.Nes.Output.Video;

namespace MyNes.Nes
{
    /// <summary>
    /// Picture Processing Unit class
    /// </summary>

    public class PPU
    {
        /// <summary>
        /// Picture Processing Unit class
        /// </summary>
        /// <param name="nes">The nes</param>
        public PPU(NES nes)
        {
            this.nes = nes;
            CONSOLE.WriteLine(this, "PPU Initialized ok.", DebugStatus.Cool);
        }
        NES nes;

        public IGraphicDevice VIDEO;
        public TVFORMAT TV = TVFORMAT.NTSC;

        bool ExecuteNMIOnVBlank = false;
        public bool SpriteSize = false;//(false=8x8, true=8x16)
        public int PatternTableAddressBackground = 0;
        public int PatternTableAddress8x8Sprites = 0;
        int VRAMAddressIncrement = 1;
        bool MonochromeMode = false;
        bool BackgroundClipping = false;
        bool SpriteClipping = false;
        public bool BackgroundVisibility = false;
        bool BBackgroundVisibility = false;
        public bool SpriteVisibility = false;
        byte ColorEmphasis = 0;
        public byte SpriteRamAddress = 0;
        public ushort VRAM_ADDRESS = 0;
        public ushort VRAM_TEMP = 0;
        byte VRAM_READBUFFER = 0;
        bool PPU_TOGGLE = true;
        bool VBlank = false;
        bool Sprite0Hit = false;
        bool OddFrame = false;
        public bool IsMapper5CHR = false;
        public bool IsBGCHR = false;//Used for mapper 5, this will make the BG rendered using the bg chr
        public byte EXRAMMODE = 0;//Only for mapper 5
        int[] ScreenBuffer = new int[240 * 256];
        public byte[] piorityBuff = new byte[256];

        public int ScanLine = -1;
        public int ScanCycle = 0;
        int TileCycle = 8;//?

        byte TileX = 0;
        byte SpriteCrossed = 0;
        bool SpritesRendered = false;
        public bool FrameDone = false;
        public int ScanlinesPerFrame = 261;

        //int ScanlineOfEndOfVblank = 21;
        public int ScanlineOfEndOfVblank = 22;
        //Draw
        int nameTableAddress = 0x2000;
        byte HScroll = 0;
        int VScroll = 0;
        public int[] PALETTE =//PAL palette
        { 
        0x808080, 0xbb, 0x3700bf, 0x8400a6, 0xbb006a, 0xb7001e, 0xb30000, 0x912600, 
        0x7b2b00, 0x3e00, 0x480d, 0x3c22, 0x2f66, 0, 0x50505, 0x50505, 
        0xc8c8c8, 0x59ff, 0x443cff, 0xb733cc, 0xff33aa, 0xff375e, 0xff371a, 0xd54b00,
        0xc46200, 0x3c7b00, 0x1e8415, 0x9566, 0x84c4, 0x111111, 0x90909, 0x90909, 
        0xffffff, 0x95ff, 0x6f84ff, 0xd56fff, 0xff77cc, 0xff6f99, 0xff7b59, 0xff915f,
        0xffa233, 0xa6bf00, 0x51d96a, 0x4dd5ae, 0xd9ff, 0x666666, 0xd0d0d, 0xd0d0d, 
        0xffffff, 0x84bfff, 0xbbbbff, 0xd0bbff, 0xffbfea, 0xffbfcc, 0xffc4b7, 0xffccae,
        0xffd9a2, 0xcce199, 0xaeeeb7, 0xaaf7ee, 0xb3eeff, 0xdddddd, 0x111111, 0x111111
        };
        //For rendering
        public byte tiledata1;
        public byte tiledata2;
        public uint tilepage;
        public int tilenumber;
        public int tileDataOffset;
        public int virtualScanline;
        public int virtualColumn;
        public byte PaletteUpperBits;
        public RenderLineState rstate;
        public bool vflip;
        public bool hflip;
        //For save state
        byte reg2000 = 0;
        byte reg2001 = 0;
        //helper
        public bool isMMC3IRQ = false;

        public void RunPPU()
        {
            ScanCycle++;
            TileCycle++;
            if (ScanCycle == 341)
            {
                ScanCycle = 0;
                TileCycle = 8;
                SpritesRendered = false;
                ScanLine++;
                //some mappers tick each scanline and not pause at vblank
                if (nes.Memory.MAPPER.ScanlineTimerNotPauseAtVBLANK)
                {
                    nes.Memory.MAPPER.TickScanlineTimer();
                }

                //Render the actual data to be displayed on the screen
                if (ScanLine >= ScanlineOfEndOfVblank & ScanLine <= 239 + ScanlineOfEndOfVblank)
                {
                    rstate = RenderLineState.ClearScanline;
                    //Clean up the line from before
                    //Draw background color
                    byte B = nes.PPUMemory[0x3F00];
                    if (B >= 63)
                        B = 63;
                    for (int i = 0; i < 256; i++)
                    {
                        ScreenBuffer[(ScanLine - ScanlineOfEndOfVblank) * 256 + i] = PALETTE[B];
                        piorityBuff[i] = 0;
                    }
                   
                }
                //Tick mapper timer (scanline timer)
                if (ScanLine >= ScanlineOfEndOfVblank & ScanLine <= 240 + ScanlineOfEndOfVblank)
                {
                    if (!nes.Memory.MAPPER.ScanlineTimerNotPauseAtVBLANK)
                    {
                        if (BackgroundVisibility | SpriteVisibility)
                            nes.Memory.MAPPER.TickScanlineTimer();
                    }
                }
                //do nothing for 1 scanline
                if (ScanLine == ScanlinesPerFrame)
                {
                    ScanLine = -1;
                    FrameDone = true;
                    //Odd frame ? only in ntsc
                    if (TV == TVFORMAT.NTSC)
                    {
                        OddFrame = !OddFrame;
                        if (!OddFrame & BackgroundVisibility)
                        {
                            if (BBackgroundVisibility)
                                ScanCycle += 1;
                        }
                        BBackgroundVisibility = BackgroundVisibility;
                    }
                    //render into screen
                    VIDEO.RenderFrame(ScreenBuffer);
                }
            }
            #region Render on the screen
            else if (ScanLine >= ScanlineOfEndOfVblank & ScanLine <= 239 + ScanlineOfEndOfVblank)
            {
                if (ScanCycle <= 256)
                {
                    if (TileCycle >= 8)//Render BG tile each 8 cycles
                    {
                        if (BackgroundVisibility)
                        {
                            if (rstate != RenderLineState.RenderBackground)
                            {
                                rstate = RenderLineState.RenderBackground;
                                hflip = true;
                                vflip = false;
                            }
                            RenderNextBackgroundTile();
                        }
                        TileCycle -= 8;
                    }
                }
                else if (!SpritesRendered)
                {
                    //if (SpriteVisibility)
                    {
                        rstate = RenderLineState.RenderSprite;
                        RenderSprites();
                    }
                    if (BackgroundClipping)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            ScreenBuffer[(ScanLine - ScanlineOfEndOfVblank) * 256 + i] = 0;
                        }
                    }
                    SpritesRendered = true;
                }
            }
            #endregion
            #region Start of vblank
            if (ScanLine == 0 & ScanCycle == 339)
            {
                if (ExecuteNMIOnVBlank)
                {
                    nes.CPU.NMIRequest = true;
                }
                VBlank = true;
            }
            #endregion
            #region End of vblank
            else if (ScanLine == ScanlineOfEndOfVblank)
            {
                //Update Vscroll
                if (ScanCycle == 305)
                {
                    if (SpriteVisibility | BackgroundVisibility)
                        VRAM_ADDRESS = VRAM_TEMP;

                    VScroll = (byte)(((VRAM_TEMP & 0x03E0) >> 2) | ((VRAM_TEMP & 0x7000) >> 12));
                    if (VScroll >= 240)
                        VScroll -= 255;
                }
                if (ScanCycle == 339)
                {
                    //clear buffer
                    VIDEO.Begin();
                    //Clear flags
                    Sprite0Hit = false;
                    VBlank = false;
                    SpriteCrossed = 0;
                }
            }
            #endregion
        }
        void RenderNextBackgroundTile()
        {
            //Update Hscroll and name table
            nameTableAddress = ((VRAM_ADDRESS & 0x0800) +
            (VRAM_TEMP & 0x0400) + 0x2000);

            HScroll = (byte)(((VRAM_TEMP & 0x001F) << 3) | TileX);

            bool vScrollSide = ((HScroll / 8) + (ScanCycle / 8)) > 31;
            int virtualRow = ((ScanLine - ScanlineOfEndOfVblank) + (VScroll));
            int ScanlineOfRender = (ScanLine - ScanlineOfEndOfVblank);
            int nameTableBase = nameTableAddress;
            int startColumn = 0;
            int endColumn = 0;
            if (!vScrollSide)
            {
                if (virtualRow >= 240)
                {
                    if (nameTableAddress == 0x2000)
                        nameTableBase = 0x2800;
                    else if (nameTableAddress == 0x2400)
                        nameTableBase = 0x2C00;
                    else if (nameTableAddress == 0x2800)
                        nameTableBase = 0x2000;
                    else if (nameTableAddress == 0x2C00)
                        nameTableBase = 0x2400;
                    virtualRow -= 240;
                }
                startColumn = (HScroll / 8) + (ScanCycle / 8);
            }
            else
            {
                if (virtualRow >= 240)
                {
                    if (nameTableAddress == 0x2000)
                        nameTableBase = 0x2C00;
                    else if (nameTableAddress == 0x2400)
                        nameTableBase = 0x2800;
                    else if (nameTableAddress == 0x2800)
                        nameTableBase = 0x2400;
                    else if (nameTableAddress == 0x2C00)
                        nameTableBase = 0x2000;
                    virtualRow -= 240;
                }
                else
                {
                    if (nameTableAddress == 0x2000)
                        nameTableBase = 0x2400;
                    else if (nameTableAddress == 0x2400)
                        nameTableBase = 0x2000;
                    else if (nameTableAddress == 0x2800)
                        nameTableBase = 0x2C00;
                    else if (nameTableAddress == 0x2C00)
                        nameTableBase = 0x2800;
                }
                startColumn = ((HScroll / 8) + (ScanCycle / 8) - 32);
            }

            if ((HScroll % 8) != 0)
            {
                endColumn = startColumn + 2;
                if (endColumn > 32)
                    endColumn = 32;
            }
            else
            {
                endColumn = startColumn + 1;
            }

            for (int currentTileColumn = startColumn; currentTileColumn < endColumn;
                currentTileColumn++)
            {
                //Starting tile row is currentScanline / 8
                //The offset in the tile is currentScanline % 8

                //Step #1, get the tile number
                tilenumber = nes.PPUMemory[(ushort)(nameTableBase + ((virtualRow / 8) * 32) + currentTileColumn)];

                //Step #2, get the offset for the tile in the tile data
                tileDataOffset = PatternTableAddressBackground + (tilenumber * 16);

                //Step #3, get the tile data from chr rom
                if (!IsMapper5CHR)
                {
                    tiledata1 = nes.PPUMemory[(ushort)(tileDataOffset + (virtualRow % 8))];
                    tiledata2 = nes.PPUMemory[(ushort)(tileDataOffset + (virtualRow % 8) + 8)];
                    tilepage = nes.Memory.CHR_PAGE[((tileDataOffset + (virtualRow % 8)) & 0x1C00) >> 10];
                }
                else
                {
                    if (EXRAMMODE == 1)
                    {
                        int row = virtualRow;
                        if (row < 0)
                            row *= -1;
                        int EXtilenumber = nes.PPUMemory.NameTables[2][(ushort)(((row / 8) * 32) + currentTileColumn)] & 0x3F;
                        tiledata1 = nes.PPUMemory.ReadCHRExtra((ushort)(tileDataOffset + (virtualRow % 8)), EXtilenumber);
                        tiledata2 = nes.PPUMemory.ReadCHRExtra((ushort)(tileDataOffset + (virtualRow % 8) + 8), EXtilenumber);
                    }
                    else
                    {
                        if (IsBGCHR & SpriteSize)//For mapper 5
                        {
                            tiledata1 = nes.PPUMemory.ReadBGCHR((ushort)(tileDataOffset + (virtualRow % 8)));
                            tiledata2 = nes.PPUMemory.ReadBGCHR((ushort)(tileDataOffset + (virtualRow % 8) + 8));
                            tilepage = nes.Memory.CHRBG_PAGE[((tileDataOffset + (virtualRow % 8)) & 0x1C00) >> 10];
                        }
                        else
                        {
                            tiledata1 = nes.PPUMemory[(ushort)(tileDataOffset + (virtualRow % 8))];
                            tiledata2 = nes.PPUMemory[(ushort)(tileDataOffset + (virtualRow % 8) + 8)];
                            tilepage = nes.Memory.CHR_PAGE[((tileDataOffset + (virtualRow % 8)) & 0x1C00) >> 10];
                        }
                    }
                }

                //Step #4, get the attribute byte for the block of tiles we're in
                //this will put us in the correct section in the palette table
                if (!IsMapper5CHR)
                {
                    PaletteUpperBits = nes.PPUMemory[(ushort)((nameTableBase +
                        0x3c0 + (((virtualRow / 8) / 4) * 8) + (currentTileColumn / 4)))];
                    PaletteUpperBits = (byte)(PaletteUpperBits >> ((4 * (((virtualRow / 8) % 4) / 2)) +
                        (2 * ((currentTileColumn % 4) / 2))));
                    PaletteUpperBits = (byte)((PaletteUpperBits & 0x3) << 2);
                }
                else
                {
                    if (EXRAMMODE == 1)
                    {
                        int row = virtualRow;
                        if (row < 0)
                            row *= -1;
                        PaletteUpperBits = (byte)((nes.PPUMemory.NameTables[2][(ushort)(((row / 8) * 32) + currentTileColumn)] & 0xC0) >> 4);
                    }
                    else
                    {
                        PaletteUpperBits = nes.PPUMemory[(ushort)((nameTableBase +
                         0x3c0 + (((virtualRow / 8) / 4) * 8) + (currentTileColumn / 4)))];
                        PaletteUpperBits = (byte)(PaletteUpperBits >> ((4 * (((virtualRow / 8) % 4) / 2)) +
                            (2 * ((currentTileColumn % 4) / 2))));
                        PaletteUpperBits = (byte)((PaletteUpperBits & 0x3) << 2);
                    }
                }
                //Step #5, render the line inside the tile to the offscreen buffer
                int startTilePixel = 0;
                int endTilePixel = 0;
                if (!vScrollSide)
                {
                    if (currentTileColumn == startColumn)
                    {
                        startTilePixel = HScroll % 8;
                        endTilePixel = 8;
                    }
                    else
                    {
                        startTilePixel = 0;
                        endTilePixel = 8;
                    }
                }
                else
                {
                    if (currentTileColumn == 32 + (HScroll / 8))
                    {
                        startTilePixel = 0;
                        endTilePixel = HScroll % 8;
                    }
                    else
                    {
                        startTilePixel = 0;
                        endTilePixel = 8;
                    }
                }
                for (int i = startTilePixel; i < endTilePixel; i++)
                {
                    virtualColumn = 7 - i;
                    virtualScanline = virtualRow % 8;
                    int pixelColor = PaletteUpperBits + (((tiledata2 & (1 << (7 - i))) >> (7 - i)) << 1) +
                        ((tiledata1 & (1 << (7 - i))) >> (7 - i));

                    if ((pixelColor % 4) != 0)
                    {
                        if (!vScrollSide)
                        {
                            int tmpX = (8 * currentTileColumn) - HScroll + i;
                            piorityBuff[tmpX] = 1;
                            ScreenBuffer[ScanlineOfRender * 256 + tmpX] = PALETTE[nes.PPUMemory[(ushort)(0x3F00 + pixelColor)]];
                        }
                        else
                        {
                            int tmpX = (8 * currentTileColumn) + (256 - HScroll) + i;
                            if (tmpX < 256)
                            {
                                piorityBuff[tmpX] = 1;
                                ScreenBuffer[ScanlineOfRender * 256 + tmpX] = PALETTE[nes.PPUMemory[(ushort)(0x3F00 + pixelColor)]];
                            }
                        }
                    }
                    else
                    {
                        if (!vScrollSide)
                        {
                            VIDEO.BlankPixel((8 * currentTileColumn) - HScroll + i, ScanlineOfRender);
                        }
                        else
                        {
                            if (((8 * currentTileColumn) + (256 - HScroll) + i) < 256)
                            {
                                VIDEO.BlankPixel((8 * currentTileColumn) + (256 - HScroll) + i, ScanlineOfRender);
                            }
                        }
                    }
                }
            }
        }
        void RenderSprites()
        {
            int tmpX;
            bool drawPixel;
            int _SpriteSize = SpriteSize ? 16 : 8;
            virtualScanline = 0;
            //1: loop through SPR-RAM
            for (tilenumber = 0; tilenumber < 256; tilenumber += 4)
            {
                int PixelColor = 0;
                byte YCoordinate = (byte)(nes.PPUMemory.SPR_RAM[tilenumber] + 1);
                //2: if the sprite falls on the current scanline, draw it
                if ((YCoordinate <= (ScanLine - ScanlineOfEndOfVblank)) &&
                    ((YCoordinate + _SpriteSize) > (ScanLine - ScanlineOfEndOfVblank)))
                {
                    if ((SpriteVisibility | BackgroundVisibility))
                        SpriteCrossed++;
                    //3: Draw the sprites differently if they are 8x8 or 8x16
                    if (!SpriteSize)//8x8
                    {
                        //4: calculate which line of the sprite is currently being drawn
                        //Line to draw is: currentScanline - Y coord + 1
                        if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x80) != 0x80)
                        {
                            vflip = false;
                            virtualScanline = (ScanLine - ScanlineOfEndOfVblank) - YCoordinate;
                        }
                        else
                        {
                            vflip = true;
                            virtualScanline = YCoordinate + 7 - (ScanLine - ScanlineOfEndOfVblank);
                        }
                        //5: calculate the offset to the sprite's data in
                        //our chr rom data 
                        tileDataOffset = PatternTableAddress8x8Sprites + nes.PPUMemory.SPR_RAM[tilenumber + 1] * 16;
                        //6: extract our tile data
                        tiledata1 = nes.PPUMemory[(ushort)(tileDataOffset + virtualScanline)];
                        tiledata2 = nes.PPUMemory[(ushort)((tileDataOffset + virtualScanline) + 8)];
                        tilepage = nes.Memory.CHR_PAGE[((tileDataOffset + virtualScanline) & 0x1C00) >> 10];
                        //7: get the palette attribute data
                        byte PaletteUpperBits = (byte)((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x3) << 2);
                        int virtualColumn = 0;
                        //8: render the line inside the tile into the screen direcly
                        for (int j = 0; j < 8; j++)
                        {
                            if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x40) == 0x40)
                            {
                                //hflip = false;
                                virtualColumn = j;
                            }
                            else
                            {
                                //hflip = true;
                                virtualColumn = 7 - j;
                            }
                            PixelColor = PaletteUpperBits + (((tiledata2 & (1 << (virtualColumn))) >> (virtualColumn)) << 1) +
                            ((tiledata1 & (1 << (virtualColumn))) >> (virtualColumn));
                            tmpX = nes.PPUMemory.SPR_RAM[tilenumber + 3] + j;
                            if (tmpX < 256)
                            {
                                drawPixel = false;
                                //Sprite 0 hit
                                if ((PixelColor % 4) != 0)
                                {
                                    if (tilenumber == 0 & piorityBuff[tmpX] == 1)
                                    {
                                        Sprite0Hit = true;
                                    }
                                }
                                if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x20) == 0x20)
                                {
                                    if (piorityBuff[tmpX] == 0)
                                        drawPixel = true;
                                }
                                else
                                {
                                    if (piorityBuff[tmpX] < 2)
                                        drawPixel = true;
                                }
                                if ((PixelColor % 4) != 0)
                                    piorityBuff[tmpX] = 2;
                                if (drawPixel)
                                {
                                    if ((PixelColor % 4) != 0)
                                    {
                                        if (SpriteVisibility)
                                            ScreenBuffer[(ScanLine - ScanlineOfEndOfVblank) * 256 + tmpX] =
                                              PALETTE[nes.PPUMemory[(ushort)(0x3F10 + PixelColor)]];

                                    }
                                    else
                                    {
                                        VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                    }
                                }
                                else if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x20) == 0)
                                {
                                    VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                }
                            }
                        }
                    }
                    else//8x16
                    {
                        //4: get the sprite id
                        byte SpriteId = nes.PPUMemory.SPR_RAM[tilenumber + 1];
                        if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x80) != 0x80)
                        {
                            vflip = false;
                            virtualScanline = (ScanLine - ScanlineOfEndOfVblank) - YCoordinate;
                        }
                        else
                        {
                            vflip = true;
                            virtualScanline = YCoordinate + 15 - (ScanLine - ScanlineOfEndOfVblank);
                        }
                        //5: We draw the sprite like two halves, so getting past the 
                        //first 8 puts us into the next tile
                        //If the ID is even, the tile is in 0x0000, odd 0x1000
                        tileDataOffset = 0;
                        if (virtualScanline < 8)
                        {
                            //Draw the top tile
                            if ((SpriteId % 2) == 0)
                                tileDataOffset = 0x0000 + (SpriteId) * 16;
                            else
                                tileDataOffset = 0x1000 + (SpriteId - 1) * 16;
                        }
                        else
                        {
                            //Draw the bottom tile
                            virtualScanline -= 8;
                            if ((SpriteId % 2) == 0)
                                tileDataOffset = 0x0000 + (SpriteId + 1) * 16;
                            else
                                tileDataOffset = 0x1000 + (SpriteId) * 16;
                        }
                        //6: extract our tile data
                        tiledata1 = nes.PPUMemory[(ushort)(tileDataOffset + virtualScanline)];
                        tiledata2 = nes.PPUMemory[(ushort)((tileDataOffset + virtualScanline) + 8)];
                        tilepage = nes.Memory.CHR_PAGE[(((tileDataOffset + virtualScanline) + 8) & 0x1C00) >> 10];
                        //7: get the palette attribute data
                        PaletteUpperBits = (byte)((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x3) << 2);
                        //8, render the line inside the tile to the screen
                        for (int j = 0; j < 8; j++)
                        {
                            if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x40) == 0x40)
                            {
                                hflip = false;
                                virtualColumn = j;
                            }
                            else
                            {
                                hflip = true;
                                virtualColumn = 7 - j;
                            }
                            PixelColor = PaletteUpperBits + (((tiledata2 & (1 << (virtualColumn))) >> (virtualColumn)) << 1) +
                                    ((tiledata1 & (1 << (virtualColumn))) >> (virtualColumn));
                            tmpX = nes.PPUMemory.SPR_RAM[tilenumber + 3] + j;
                            if (tmpX < 256)
                            {
                                drawPixel = false;
                                //Sprite 0 hit
                                if ((PixelColor % 4) != 0)
                                {
                                    if (tilenumber == 0 & piorityBuff[tmpX] == 1)
                                    {
                                        Sprite0Hit = true;
                                    }
                                }
                                if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x20) == 0x20)
                                {
                                    if (piorityBuff[tmpX] == 0)
                                        drawPixel = true;
                                }
                                else
                                {
                                    if (piorityBuff[tmpX] < 2)
                                        drawPixel = true;
                                }
                                if ((PixelColor % 4) != 0)
                                    piorityBuff[tmpX] = 2;
                                if (drawPixel)
                                {
                                    if ((PixelColor % 4) != 0)
                                    {
                                        if (SpriteVisibility)
                                            ScreenBuffer[(ScanLine - ScanlineOfEndOfVblank) * 256 + tmpX] =
                                            PALETTE[nes.PPUMemory[(ushort)(0x3F10 + PixelColor)]];
                                    }
                                    else
                                    {
                                        VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                    }
                                }
                                else if ((nes.PPUMemory.SPR_RAM[tilenumber + 2] & 0x20) == 0)
                                {
                                    VIDEO.BlankPixel(tmpX, (ScanLine - ScanlineOfEndOfVblank));
                                }
                            }
                        }
                    }
                }
                else
                {
                    SpriteRamAddress = 0;
                }
            }
        }
        /// <summary>
        /// Set the nes pallete and tv system
        /// </summary>
        /// <param name="FORMAT">Tv system</param>
        /// <param name="PlFormat">The palette format</param>
        public void SetPallete(TVFORMAT FORMAT, PaletteFormat PlFormat)
        {
            TV = FORMAT;
            switch (FORMAT)
            {
                case TVFORMAT.NTSC:
                    //Setup timing
                    nes.FramePeriod = 0.01666666666666667;
                    ScanlinesPerFrame = 261;
                    ScanlineOfEndOfVblank = 21;
                    try
                    {
                        nes.Memory.apu.ChannelDpm.IsPAL = false;
                    }
                    catch { }
                    if (PlFormat.UseInternalPalette)
                    {
                        switch (PlFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.NTSC:
                                PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.PAL:
                                PALETTE = Paletter.PALPalette;
                                break;
                        }

                    }
                    else
                    {
                        PALETTE = Paletter.LOADEDPalette;
                    }
                    break;
                case TVFORMAT.PAL:
                    //Setup timing
                    nes.FramePeriod = 0.020;
                    ScanlinesPerFrame = 311;
                    ScanlineOfEndOfVblank = 71;
                    try
                    {
                        nes.Memory.apu.ChannelDpm.IsPAL = true;
                    }
                    catch { }
                    if (PlFormat.UseInternalPalette)
                    {
                        switch (PlFormat.UseInternalPaletteMode)
                        {
                            case UseInternalPaletteMode.Auto:
                                PALETTE = Paletter.PALPalette;
                                break;
                            case UseInternalPaletteMode.NTSC:
                                PALETTE = Paletter.NTSCPalette;
                                break;
                            case UseInternalPaletteMode.PAL:
                                PALETTE = Paletter.PALPalette;
                                break;
                        }

                    }
                    else
                    {
                        PALETTE = Paletter.LOADEDPalette;
                    }
                    break;
            }
        }
        #region Registers
        public void Write2000(byte value)
        {
            VRAM_TEMP = (ushort)((VRAM_TEMP & 0xF3FF) | ((value & 0x3) << 10));
            VRAMAddressIncrement = ((value & 0x04) != 0) ? 32 : 1;//Bit2
            PatternTableAddress8x8Sprites = ((value & 0x8) == 0x8) ? 0x1000 : 0;//Bit3
            PatternTableAddressBackground = ((value & 0x10) == 0x10) ? 0x1000 : 0;//Bit4
            SpriteSize = (value & 0x20) == 0x20;//Bit5
            ExecuteNMIOnVBlank = (value & 0x80) == 0x80;//Bit 7

            reg2000 = value;
        }
        public void Write2001(byte value)
        {
            MonochromeMode = (value & 0x01) != 0;//Bit 0
            BackgroundClipping = (value & 0x02) == 0;//Bit 1
            SpriteClipping = (value & 0x04) == 0;//Bit 2
            BackgroundVisibility = (value & 0x8) != 0;//Bit 3
            SpriteVisibility = (value & 0x10) != 0;//Bit 4
            ColorEmphasis = (byte)((value & 0xE0) << 1);//Bit 5 - 7

            reg2001 = value;
        }
        public byte Read2002()
        {
            byte status = 0;
            // VBlank
            if (VBlank)
                status |= 0x80;

            //Sprite 0 Hit
            if (Sprite0Hit & (BackgroundVisibility & SpriteVisibility))
                status |= 0x40;

            //More than 8 sprites in 1 scanline
            if ((SpriteCrossed > 8) /*& (SpriteVisibility | BackgroundVisibility)*/)
                status |= 0x20;

            //If it should ignore any write into 2007
            if ((ScanLine < ScanlineOfEndOfVblank) | !(BackgroundVisibility & SpriteVisibility))
                status |= 0x10;

            VBlank = false;
            PPU_TOGGLE = true;
            return status;
        }
        public byte DumpRead2002()
        {
            byte status = 0;
            // VBlank
            if (VBlank)
                status |= 0x80;

            //Sprite 0 Hit
            if (Sprite0Hit & (BackgroundVisibility & SpriteVisibility))
                status |= 0x40;

            //More than 8 sprites in 1 scanline
            if ((SpriteCrossed > 8) & (SpriteVisibility | BackgroundVisibility))
                status |= 0x20;

            //If it should ignore any write into 2007
            if ((ScanLine < ScanlineOfEndOfVblank) | !(BackgroundVisibility & SpriteVisibility))
                status |= 0x10;

            return status;
        }
        public void Write2003(byte value)
        {
            SpriteRamAddress = value;
        }
        public void Write2004(byte value)
        {
            nes.PPUMemory.SPR_RAM[SpriteRamAddress] = value;
            SpriteRamAddress++;
        }
        public byte Read2004()
        {
            return nes.PPUMemory.SPR_RAM[SpriteRamAddress];
        }
        public void Write2005(byte value)
        {
            if (PPU_TOGGLE)
            {
                TileX = (byte)(value & 0x07);
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0xFFE0) | ((value & 0xF8) >> 3));
            }
            else
            {
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0x0C1F) | ((value & 7) << 12) | ((value & 0xF8) << 2));
            }
            PPU_TOGGLE = !PPU_TOGGLE;
        }
        public void Write2006(byte value)
        {
            if (PPU_TOGGLE)
            {
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0x00FF) | ((value & 0x3F) << 8));
            }
            else
            {
                VRAM_TEMP = (ushort)((VRAM_TEMP & 0xFF00) | value);
                if (isMMC3IRQ)
                    if (((VRAM_TEMP & 0x1000) == 0x1000) && ((VRAM_ADDRESS & 0x1000) == 0))
                        nes.Mapper.TickScanlineTimer();
             
                VRAM_ADDRESS = VRAM_TEMP;

                //Update reload bits
                VScroll = (((VRAM_TEMP & 0x03E0) >> 2) | ((VRAM_TEMP & 0x7000) >> 12));

                if (ScanLine < ScanlinesPerFrame)
                    VScroll -= (ScanLine - ScanlineOfEndOfVblank);
            }
            PPU_TOGGLE = !PPU_TOGGLE;
        }
        public void Write2007(byte value)
        {
            nes.PPUMemory[(ushort)(VRAM_ADDRESS & 0x3FFF)] = value;

            ushort previuos = VRAM_ADDRESS;

            VRAM_ADDRESS += (ushort)VRAMAddressIncrement;

            if (isMMC3IRQ)
                if (((VRAM_ADDRESS & 0x1000) == 0x1000) && ((previuos & 0x1000) == 0))
                    nes.Mapper.TickScanlineTimer();
        }
        public byte Read2007()
        {
            byte returnedValue = VRAM_READBUFFER;
            if ((ushort)(VRAM_ADDRESS & 0x3FFF) < 0x3F00)
                VRAM_READBUFFER = nes.PPUMemory[(ushort)(VRAM_ADDRESS & 0x3FFF)];
            else
                returnedValue = nes.PPUMemory[(ushort)(VRAM_ADDRESS & 0x3FFF)];

            ushort previuos = VRAM_ADDRESS;

            VRAM_ADDRESS += (ushort)VRAMAddressIncrement;

            if (isMMC3IRQ)
                if (((VRAM_ADDRESS & 0x1000) == 0x1000) && ((previuos & 0x1000) == 0))
                    nes.Mapper.TickScanlineTimer();

            return returnedValue;
        }
        public void Write4014(byte value)
        {
            ushort a = (ushort)(value << 8);
            for (int i = 0; i < 0x100; i++)
            {
                nes.PPUMemory.SPR_RAM[(byte)(0xFF & (SpriteRamAddress + i))] =
                  nes.Memory[(ushort)(a + i)];
            }
            nes.CPU.ADDCycles = 512;
        }
        #endregion
        public PPUMemory MEM_PPU
        { get { return nes.PPUMemory; } }

        public void SaveState(System.IO.Stream stream)
        {
            //0x2000
            stream.WriteByte(reg2000);
            //0x2001
            stream.WriteByte(reg2001);
            //0x2002 and others
            byte status = 0;
            if (VBlank)
                status |= 0x01;
            if (Sprite0Hit)
                status |= 0x02;
            if (PPU_TOGGLE)
                status |= 0x04;
            if (OddFrame)
                status |= 0x10;
            stream.WriteByte(status);
            //SpriteCrossed
            stream.WriteByte(SpriteCrossed);
            //0x2003
            stream.WriteByte(SpriteRamAddress);
            //vram address
            stream.WriteByte((byte)((VRAM_ADDRESS & 0xFF00) >> 8));
            stream.WriteByte((byte)((VRAM_ADDRESS & 0x00FF)));
            stream.WriteByte((byte)((VRAM_TEMP & 0xFF00) >> 8));
            stream.WriteByte((byte)((VRAM_TEMP & 0x00FF)));
            stream.WriteByte(VRAM_READBUFFER);
            //piority buffer
            stream.Write(piorityBuff, 0, piorityBuff.Length);
            //scrolls
            stream.WriteByte(TileX);
            stream.WriteByte(HScroll);
            stream.WriteByte((byte)((VScroll & 0xFF000000) >> 24));
            stream.WriteByte((byte)((VScroll & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((VScroll & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((VScroll & 0x000000FF)));
            stream.WriteByte((byte)((nameTableAddress & 0xFF000000) >> 24));
            stream.WriteByte((byte)((nameTableAddress & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((nameTableAddress & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((nameTableAddress & 0x000000FF)));
            //counters
            stream.WriteByte((byte)((ScanLine & 0xFF000000) >> 24));
            stream.WriteByte((byte)((ScanLine & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((ScanLine & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((ScanLine & 0x000000FF)));
            stream.WriteByte((byte)((ScanCycle & 0xFF000000) >> 24));
            stream.WriteByte((byte)((ScanCycle & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((ScanCycle & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((ScanCycle & 0x000000FF)));
            stream.WriteByte((byte)((TileCycle & 0xFF000000) >> 24));
            stream.WriteByte((byte)((TileCycle & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((TileCycle & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((TileCycle & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            //0x2000
            reg2000 = (byte)stream.ReadByte();
            VRAMAddressIncrement = ((reg2000 & 0x04) != 0) ? 32 : 1;
            PatternTableAddress8x8Sprites = ((reg2000 & 0x8) == 0x8) ? 0x1000 : 0;
            PatternTableAddressBackground = ((reg2000 & 0x10) == 0x10) ? 0x1000 : 0;
            SpriteSize = (reg2000 & 0x20) == 0x20;
            ExecuteNMIOnVBlank = (reg2000 & 0x80) == 0x80;
            //0x2001
            reg2001 = (byte)stream.ReadByte();
            MonochromeMode = (reg2001 & 0x01) != 0;
            BackgroundClipping = (reg2001 & 0x02) == 0;
            SpriteClipping = (reg2001 & 0x04) == 0;
            BackgroundVisibility = (reg2001 & 0x8) != 0;
            SpriteVisibility = (reg2001 & 0x10) != 0;
            ColorEmphasis = (byte)((reg2001 & 0xE0) << 1);
            //0x2002 and others
            byte status = (byte)stream.ReadByte();
            VBlank = ((status & 0x1) == 0x1);
            Sprite0Hit = ((status & 0x2) == 0x2);
            PPU_TOGGLE = ((status & 0x4) == 0x4);
            OddFrame = ((status & 0x10) == 0x10);
            //SpriteCrossed
            SpriteCrossed = (byte)stream.ReadByte();
            //0x2003
            SpriteRamAddress = (byte)stream.ReadByte();
            //vram address
            VRAM_ADDRESS = (ushort)(stream.ReadByte() << 8);
            VRAM_ADDRESS = (ushort)((VRAM_ADDRESS & 0xFF00) | stream.ReadByte());
            VRAM_TEMP = (ushort)(stream.ReadByte() << 8);
            VRAM_TEMP = (ushort)((VRAM_TEMP & 0xFF00) | stream.ReadByte());
            VRAM_READBUFFER = (byte)stream.ReadByte();
            //piority buffer
            stream.Read(piorityBuff, 0, piorityBuff.Length);
            //scrolls
            TileX = (byte)stream.ReadByte();
            HScroll = (byte)stream.ReadByte();
            VScroll = (int)(stream.ReadByte() << 24);
            VScroll |= (int)(stream.ReadByte() << 16);
            VScroll |= (int)(stream.ReadByte() << 8);
            VScroll |= stream.ReadByte();
            nameTableAddress = (int)(stream.ReadByte() << 24);
            nameTableAddress |= (int)(stream.ReadByte() << 16);
            nameTableAddress |= (int)(stream.ReadByte() << 8);
            nameTableAddress |= stream.ReadByte();

            ScanLine = (int)(stream.ReadByte() << 24);
            ScanLine |= (int)(stream.ReadByte() << 16);
            ScanLine |= (int)(stream.ReadByte() << 8);
            ScanLine |= stream.ReadByte();
            ScanCycle = (int)(stream.ReadByte() << 24);
            ScanCycle |= (int)(stream.ReadByte() << 16);
            ScanCycle |= (int)(stream.ReadByte() << 8);
            ScanCycle |= stream.ReadByte();
            TileCycle = (int)(stream.ReadByte() << 24);
            TileCycle |= (int)(stream.ReadByte() << 16);
            TileCycle |= (int)(stream.ReadByte() << 8);
            TileCycle |= stream.ReadByte();
        }
    }
    public enum RenderLineState
    {
        ClearScanline,
        RenderSprite,
        RenderBackground,
        RenderClipping,
    }
}
