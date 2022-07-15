/*********************************************************************\
*This file is part of My X Nes                                        *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2010 Ala Hadid                                         *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My X Nes is free software: you can redistribute it and/or modify     *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My X Nes is distributed in the hope that it will be useful,          *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with My X Nes.  If not, see <http://www.gnu.org/licenses/>.    *
\*********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using MyNes.Nes;
using System.Xml.Serialization;
using System.Xml;

namespace RomsManager
{
    public partial class Form1 : Form
    {
        MXN_ROMS_COLLECTION RomsHolder = new MXN_ROMS_COLLECTION();
        Thread MainThread;
        public Form1()
        {
            InitializeComponent();
            if (!File.Exists(".\\CACHE.XML"))
                RefreshRoms();
            else
                button5_Click(this, null);
        }
        #region Delegates
        delegate void MainDelegate();
        delegate void SetValueDelegate(int Max);
        delegate void SetTextValueDelegate(string text);
        delegate void AddItemDelegate(object item);
        void DisableItems()
        {
            if (!this.InvokeRequired)
            {
                DisableItems1();
            }
            else
            {
                this.Invoke(new MainDelegate(DisableItems1));
            }
        }
        void DisableItems1()
        { groupBox_Roms.Enabled = false; }
        void BarSetMax(int Max)
        {
            if (!this.InvokeRequired)
            {
                BarSetMax1(Max);
            }
            else
            {
                this.Invoke(new SetValueDelegate(BarSetMax1), new object[] { Max });
            }
        }
        void BarSetMax1(int Max)
        { progressBar1.Maximum = Max; }
        void BarSetValue(int value)
        {
            if (!this.InvokeRequired)
            {
                BarSetValue1(value);
            }
            else
            {
                this.Invoke(new SetValueDelegate(BarSetValue1), new object[] { value });
            }
        }
        void BarSetValue1(int value)
        { progressBar1.Value = value; }
        void EnableItems()
        {
            if (!this.InvokeRequired)
            {
                EnableItems1();
            }
            else
            {
                this.Invoke(new MainDelegate(EnableItems1));
            }
        }
        void EnableItems1()
        { groupBox_Roms.Enabled = true; }
        void SetStatus(string text)
        {
            if (!this.InvokeRequired)
            {
                SetStatus1(text);
            }
            else
            {
                this.Invoke(new SetTextValueDelegate(SetStatus1), new object[] { text });
            }
        }
        void SetStatus1(string text)
        { label1.Text = text; }
        void AddListboxItem(object item)
        {
            if (!this.InvokeRequired)
            {
                AddListboxItem1(item);
            }
            else
            {
                this.Invoke(new SetTextValueDelegate(AddListboxItem1), new object[] { item });
            }
        }
        void AddListboxItem1(object item)
        {
            listBox1.Items.Add(item);
        }
        void ClearListBox()
        {
            if (!this.InvokeRequired)
            {
                ClearListBox1();
            }
            else
            {
                this.Invoke(new MainDelegate(ClearListBox1));
            }
        }
        void ClearListBox1()
        { listBox1.Items.Clear(); }
        #endregion
        void RefreshRoms()
        {
            MainThread = new Thread(new ThreadStart(LOADROMS));
            MainThread.Start();
        }
        void LOADROMS()
        {
            DisableItems();
            RomsHolder.ROMS.Clear();
            if (Directory.Exists(".\\ROMS"))
            {
                string[] files = Directory.GetFiles(".\\ROMS");
                string[] imgexts = new string[] { ".jpg", ".bmp", ".png", ".gif" };
                string[] pictures = new string[0];
                if (Directory.Exists(".\\PICTURES"))
                    pictures = Directory.GetFiles(".\\PICTURES");
                BarSetMax(files.Length);
                int i = 0;
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".nes")
                    {
                        Cartridge cart = new Cartridge();
                        if (cart.Load(file, true) == LoadRomStatus.LoadSuccessed)
                        {
                            MXN_ROM ro = new MXN_ROM();
                            ro.Name = Path.GetFileNameWithoutExtension(file);
                            ro.Path = file;
                            //search for the snap shot
                            foreach (string pic in pictures)
                            {
                                if (Path.GetFileNameWithoutExtension(pic).Length >= ro.Name.Length)
                                {
                                    if (Path.GetFileNameWithoutExtension(pic).Substring(0, ro.Name.Length).ToLower() == ro.Name.ToLower())
                                    {
                                        foreach (string ext in imgexts)
                                        {
                                            if (Path.GetExtension(pic).ToLower() == ext)
                                            {
                                                ro.SnapShotPath = ".\\PICTURES\\" + Path.GetFileName(pic);
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                            //rom info
                            ro.Mapper = cart.MAPPER;
                            ro.IsSRAM = cart.IsBatteryBacked;
                            ro.System = cart.IsPAL ? "PAL" : "NTSC";
                            RomsHolder.ROMS.Add(ro);
                        }

                    }
                    SetStatus("Loading roms " + i.ToString() + " of " + files.Length.ToString());
                    BarSetValue(i);
                    i++;
                }
            }
            SetStatus("Ready, " + RomsHolder.ROMS.Count.ToString() + " compatible rom(s) found.");
            AddRomsToListBox();
        }

        void AddRomsToListBox()
        {
            ClearListBox();
            foreach (MXN_ROM ro in RomsHolder.ROMS)
            {
                AddListboxItem(ro.Name);
            }
            EnableItems();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure ?\nUnsaved work will be LOST !!",
                "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
                RefreshRoms();
        }
        //Build Cach
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Stream stream = new FileStream(".\\CACHE.XML", FileMode.Create, FileAccess.Write);

                XmlSerializer ser = new XmlSerializer(typeof(MXN_ROMS_COLLECTION));
                ser.Serialize(stream, RomsHolder);
                stream.Close();

                MessageBox.Show("CACH SAVED SUCCESSFULLY !!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { }
        }
        //Reload Cach
        private void button5_Click(object sender, EventArgs e)
        {
            if (File.Exists(".\\CACHE.XML"))
            {
                try
                {
                    Stream stream = new FileStream(".\\CACHE.XML", FileMode.Open, FileAccess.Read);

                    XmlSerializer ser = new XmlSerializer(typeof(MXN_ROMS_COLLECTION));
                    RomsHolder = (MXN_ROMS_COLLECTION)ser.Deserialize(stream);
                    stream.Close();

                    AddRomsToListBox();

                    BarSetMax(100);
                    BarSetValue(100);
                    SetStatus("Roms Loaded from CACHE.XML file, " +
                        RomsHolder.ROMS.Count.ToString() + " compatible rom(s) found.");
                }
                catch
                {
                    MessageBox.Show("CAN'T LOAD CACHE.XML FILE !!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("NO CACHE.XML FILE FOUND !!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure ?\nRom(s) will be deleted only from the list.",
               "Delete " + listBox1.SelectedItems.Count.ToString() + " roms.", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
               == System.Windows.Forms.DialogResult.Yes)
            {
                for (int i = 0; i < listBox1.SelectedItems.Count; i++)
                {
                    RomsHolder.ROMS.RemoveAt(listBox1.SelectedIndices[0]);
                    listBox1.Items.Remove(listBox1.SelectedItems[0]);
                    i = -1;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = (listBox1.SelectedItems.Count == 1);
            if (listBox1.SelectedItems.Count == 1)
            {
                //Load name
                textBox1.Text = (string)listBox1.Items[listBox1.SelectedIndices[0]];
                listBox1.Select();
                if (File.Exists(RomsHolder.ROMS[listBox1.SelectedIndices[0]].SnapShotPath))
                    pictureBox1.Image = Image.FromFile(RomsHolder.ROMS[listBox1.SelectedIndices[0]].SnapShotPath);
                else
                    pictureBox1.Image = null;
            }
            else
                pictureBox1.Image = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox1.Enabled & listBox1.SelectedItems.Count == 1 & textBox1.Text.Length > 0)
            {
                listBox1.Items[listBox1.SelectedIndices[0]] = textBox1.Text;
                RomsHolder.ROMS[listBox1.SelectedIndices[0]].Name = textBox1.Text;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                button6_Click(sender, null);
        }
        //Move up
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0 & listBox1.SelectedItems.Count == 1)
            {
                MXN_ROM SelectedRom = RomsHolder.ROMS[listBox1.SelectedIndex];
                int index = listBox1.SelectedIndex;
                //Remove selected
                RomsHolder.ROMS.RemoveAt(index);
                listBox1.Items.RemoveAt(index);
                //Decrement index
                index--;
                //Insert at the new index
                RomsHolder.ROMS.Insert(index, SelectedRom);
                listBox1.Items.Insert(index, SelectedRom.Name);
                listBox1.SelectedIndex = index;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < listBox1.Items.Count - 1 & listBox1.SelectedItems.Count == 1)
            {
                MXN_ROM SelectedRom = RomsHolder.ROMS[listBox1.SelectedIndex];
                int index = listBox1.SelectedIndex;
                //Remove selected
                RomsHolder.ROMS.RemoveAt(index);
                listBox1.Items.RemoveAt(index);
                //Increment index
                index++;
                //Insert at the new index
                RomsHolder.ROMS.Insert(index, SelectedRom);
                listBox1.Items.Insert(index, SelectedRom.Name);
                listBox1.SelectedIndex = index;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 1)
            {
                if (MessageBox.Show("Are you sure ?",
                 "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                 == System.Windows.Forms.DialogResult.Yes)
                {
                    RomsHolder.ROMS[listBox1.SelectedIndex].SnapShotPath = "";
                    pictureBox1.Image = null;
                }
            }
        }
    }
}
