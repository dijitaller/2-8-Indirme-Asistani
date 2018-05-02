using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Net.NetworkInformation;

namespace AKKHelper
{
    public partial class Form1 : Form
    {

        string[] ayar;
        string[] ayar1;
        int index = 0;
        int counter = 0;
        string versiyon = "1.8.2";
        string progBaslangic;
        const int ERROR_CANCELLED = 1223;
        long baslangicDownloadData, baslangicUploadData, kapanisDownloadData, kapanisUploadData, ilkDown, ilkUp, sonDown, sonUp;

        public Form1()
        {
            InitializeComponent();
            // İnternet varsa, versiyonu kontrol edelim. 
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] raw = wc.DownloadData("https://www.dijitaller.com/2-8version.txt");

                string guncelversiyon = System.Text.Encoding.UTF8.GetString(raw);

                if (versiyon != guncelversiyon)
                {
                    DialogResult r = MessageBox.Show("Asistan için yeni bir güncelleme mevcut. Güncellemeyi indirmek ister misiniz?", "Yeni Bir Güncelleştirme Mevcut!", MessageBoxButtons.YesNo);
                    if (r == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://www.dijitaller.com/2-8-asistan/");
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Bilgisayarınız internete bağlı olmadığı için programın güncelliğini kontrol edemedik! Eski versiyonu kullanmak bazı hatalara sebep olabilir.", "Bilgi");
            }

            // Program versiyonunu hakkında sayfasına yazdık.
            lblVersiyon.Text = versiyon;

            

            //Bağlı olan network adaptörünü çek
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (ni.Name == "Wi-Fi" && ni.OperationalStatus == OperationalStatus.Up)
                    {
                        index = counter;
                        break;
                    }
                }

                counter++;
            }

            // İlk dataları çek
            ilkDown = dataKullaniminiCek(index)[0];
            ilkUp = dataKullaniminiCek(index)[1];

            int saat = DateTime.Now.Hour;
            int dakika = DateTime.Now.Minute;

            progBaslangic = saat.ToString("D2") + ":" + dakika.ToString("D2");

            notifyIcon1.Visible = true;
            notifyIcon1.Text = "D/U İstatistik İçin Tıkla";


            lblGuncelleme.Text = "Aralık: " + progBaslangic;

            // Saat ve dakika comboxlarını oluştur.
            for (int i = 0; i < 24; i++)
            {
                if (i < 10)
                {
                    cmbSaat.Items.Add("0" + i);
                    cmbUyanisSaat.Items.Add("0" + i);

                    cmbIntAcSaat.Items.Add("0" + i);
                    cmbIntKapSaat.Items.Add("0" + i);
                    
                }
                else
                {
                    cmbSaat.Items.Add(i);
                    cmbUyanisSaat.Items.Add(i);

                    cmbIntAcSaat.Items.Add(i);
                    cmbIntKapSaat.Items.Add(i);
                }

            }

            for (int i = 0; i < 60; i++)
            {
                if (i < 10)
                {
                    cmbDakika.Items.Add("0" + i);
                    cmbUyanisDakika.Items.Add("0" + i);

                    cmbIntAcDakika.Items.Add("0" + i);
                    cmbIntKapDakika.Items.Add("0" + i);
                }
                else
                {
                    cmbDakika.Items.Add(i);
                    cmbUyanisDakika.Items.Add(i);

                    cmbIntAcDakika.Items.Add(i);
                    cmbIntKapDakika.Items.Add(i);
                }

            }

            // Saatimiz çalışmaya başladı.
            timer1.Start();

            // Yeni sürümde ayar dosyası değişikliğini yapıyoruz.
            string[] ayarTemp1 = File.ReadAllLines(@"Kaynaklar\ayarlar.txt");


            if (ayarTemp1.Length > 3)
            {

                if (ayarTemp1.Length > 9)
                {
                    StreamWriter sw = new StreamWriter(@"Kaynaklar\ayarlar.txt");
                    sw.Write(ayarTemp1[0] + "|" + ayarTemp1[1] + "|" + ayarTemp1[2] + "|" + ayarTemp1[3] + "|" + ayarTemp1[4] + "|" + ayarTemp1[5] + "|" + ayarTemp1[6] + "|" + ayarTemp1[7] + "|" + ayarTemp1[8] + "|" + ayarTemp1[9]);
                    sw.Close();
                }
                else if (ayarTemp1.Length < 10)
                {
                    StreamWriter sw = new StreamWriter(@"Kaynaklar\ayarlar.txt");
                    sw.Write(ayarTemp1[0] + "|" + ayarTemp1[1] + "|" + ayarTemp1[2] + "|" + ayarTemp1[3] + "|" + ayarTemp1[4] + "|" + ayarTemp1[5] + "|" + ayarTemp1[6] + "|" + ayarTemp1[7] + "|0-3-0|0");
                    sw.Close();
                }

            }

            // Tüm ayarları çekiyoruz ve guiye şekil veriyoruz
            ayar1 = File.ReadAllLines(@"Kaynaklar\ayarlar.txt");
            ayar = ayar1[0].Split('|');

            if (ayar[8].Split('-')[0] == "0")
            {
                radioButton5.Checked = true;
            }
            else if (ayar[8].Split('-')[0] == "1")
            {
                radioButton6.Checked = true;
                panelUyanisAyar.Visible = true;
                panelUyanisAyar.Enabled = false;
                cmbUyanisSaat.SelectedIndex = Convert.ToInt32(ayar[8].Split('-')[1]);
                cmbUyanisDakika.SelectedIndex = Convert.ToInt32(ayar[8].Split('-')[2]);
            }

            if (ayar[9] == "0")
            {
                rdbWinUpdateAcik.Checked = true;
            }
            else if (ayar[9] == "1")
            {
                rdbWinUpdateKapali.Checked = true;
            }
            else if (ayar[9] == "2")
            {
                rdbWinUpdateOzel.Checked = true;
            }


            // Uyanma ayarlı mı değil mi?
            if (ayar[0] == "1")
            {
                btnSleep.Visible = true;
                label2.Text = "Uyanma AKTİF! Gün ayarını yapmak için önce \"Uyanma Modunu\" kapatın.";
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                panelInfo.BackColor = Color.Green;
                btnKaydet.Text = "Uyanma Modunu Kapat";
                btnKaydet.BackColor = Color.Maroon;
                btnKaydet.FlatAppearance.BorderColor = Color.DarkRed;
            }

            // Ayarlara göre günleri seç.
            chkPazartesi.Checked = Convert.ToBoolean(ayar[1]);
            chkSali.Checked = Convert.ToBoolean(ayar[2]);
            chkCarsamba.Checked = Convert.ToBoolean(ayar[3]);
            chkPersembe.Checked = Convert.ToBoolean(ayar[4]);
            chkCuma.Checked = Convert.ToBoolean(ayar[5]);
            chkCumartesi.Checked = Convert.ToBoolean(ayar[6]);
            chkPazar.Checked = Convert.ToBoolean(ayar[7]);

            // Varsayılan genel ayarlar.
            radioButton1.Checked = true;
            radioButton3.Checked = true;

            // Ekli programları listele.
            string[] programlar = File.ReadAllLines(@"Kaynaklar\programlar.txt");
            for (int i = 0; i < programlar.Length; i++)
            {
                string[] programBilgi = programlar[i].Split('¡');
                ListViewItem lvi = new ListViewItem();
                lvi.Text = programBilgi[0];
                lvi.SubItems.Add(programBilgi[1]);
                listView1.Items.Add(lvi);
            }

            // Yapılan indirmeleri ve yüklemeleri listele.
            try
            {
                string[] indirmeler = File.ReadAllLines(@"Kaynaklar\verikullanimi.txt");
                for (int i = 0; i < indirmeler.Length; i++)
                {
                    string[] indirmeBilgi = indirmeler[i].Split('=');
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = indirmeBilgi[0];
                    lvi.SubItems.Add(indirmeBilgi[1]);
                    lvi.SubItems.Add(indirmeBilgi[2]);
                    lvi.SubItems.Add(indirmeBilgi[3]);
                    listViewData.Items.Insert(0, lvi);
                }
            }
            catch (Exception)
            {
                File.CreateText(@"Kaynaklar\verikullanimi.txt").Close();

            }



        }

        // Ben Yatıyorum Sen Hallet Butonu
        private void btnSleep_Click(object sender, EventArgs e)
        {
                Application.SetSuspendState(PowerState.Suspend, false, false);
        }

        // Uyanma Modunu Aktif Etme Butonu
        private void btnKaydet_Click(object sender, EventArgs e)
        {

            ayar1 = File.ReadAllLines(@"Kaynaklar\ayarlar.txt");
            ayar = ayar1[0].Split('|');

            if (btnKaydet.Text == "Uyanma Modunu Aktif Et")
            {

                if (chkPazartesi.Checked == false && chkSali.Checked == false && chkCarsamba.Checked == false && chkPersembe.Checked == false && chkCuma.Checked == false && chkCumartesi.Checked == false && chkPazar.Checked == false)
                {
                    MessageBox.Show("Lütfen gün seçimi yapınız!", "HATA!");
                }
                else
                {
                    // Bat dosyası oluşturuluyor
                    string[] lines = File.ReadAllLines(@"Kaynaklar\pcac.bat");

                    StreamWriter sw = new StreamWriter(@"Kaynaklar\pcac2.bat");
                    for (int i = 0; i < 24; i++)
                    {
                        sw.WriteLine(lines[i]);

                    }

                    if (radioButton5.Checked)
                    {
                        lines[24] = @"<StartBoundary>2017-04-29T02:00:00</StartBoundary>";

                    }
                    else if (radioButton6.Checked)
                    {
                        lines[24] = @"<StartBoundary>2017-04-29T" + cmbUyanisSaat.SelectedItem.ToString() + ":" + cmbUyanisDakika.SelectedItem.ToString() + ":00</StartBoundary>";
                    }

                    sw.WriteLine(lines[24]);
                    sw.WriteLine(lines[25]);
                    sw.WriteLine(lines[26]);
                    sw.WriteLine(lines[27]);

                    for (int i = 1; i < 8; i++)
                    {
                        ayar[i] = "false";
                    }


                    if (chkPazartesi.Checked == true)
                    {
                        sw.WriteLine(@"<Monday/>");
                        ayar[1] = "true";
                    }

                    if (chkSali.Checked == true)
                    {
                        sw.WriteLine(@"<Tuesday/>");
                        ayar[2] = "true";
                    }
                    if (chkCarsamba.Checked == true)
                    {
                        sw.WriteLine(@"<Wednesday/>");
                        ayar[3] = "true";
                    }
                    if (chkPersembe.Checked == true)
                    {
                        sw.WriteLine(@"<Thursday/>");
                        ayar[4] = "true";
                    }
                    if (chkCuma.Checked == true)
                    {
                        sw.WriteLine(@"<Friday/>");
                        ayar[5] = "true";
                    }
                    if (chkCumartesi.Checked == true)
                    {
                        sw.WriteLine(@"<Saturday/>");
                        ayar[6] = "true";
                    }
                    if (chkPazar.Checked == true)
                    {
                        sw.WriteLine(@"<Sunday/>");
                        ayar[7] = "true";
                    }

                    for (int i = 28; i < lines.Length; i++)
                    {
                        sw.WriteLine(lines[i]);
                    }

                    sw.Close();



                    // Oluşturulan dosyayı çalıştır
                    try
                    {
                        batCalistir("pcac2.bat");
                        ayar[0] = "1";
                        if (!radioButton5.Checked)
                        {
                            ayar[8] = "1" + "-" + cmbUyanisSaat.SelectedIndex + "-" + cmbUyanisDakika.SelectedIndex;
                        }
                        else if (radioButton5.Checked)
                        {
                            ayar[8] = "0-3-0";
                        }

                        StreamWriter sw1 = new StreamWriter(@"Kaynaklar\ayarlar.txt");
                        sw1.Write(ayar[0] + "|" + ayar[1] + "|" + ayar[2] + "|" + ayar[3] + "|" + ayar[4] + "|" + ayar[5] + "|" + ayar[6] + "|" + ayar[7] + "|" + ayar[8] + "|" + ayar[9]);
                        sw1.Close();
                        Array.Clear(ayar, 0, ayar.Length);

                        // GUI düzenlemesini yap
                        label2.Text = "Uyanma AKTİF! Gün ayarını yapmak için önce \"Uyanma Modunu\" kapatın.";
                        panelAyar.Enabled = false;
                        btnKaydet.Text = "Uyanma Modunu Kapat";
                        btnKaydet.BackColor = Color.Maroon;
                        btnKaydet.FlatAppearance.BorderColor = Color.DarkRed;
                        panelInfo.BackColor = Color.Green;
                        btnSleep.Visible = true;
                        groupBox3.Enabled = false;
                        groupBox4.Enabled = false;
                        MessageBox.Show("Belirttiğiniz günlerde bilgisayarınız otomatik olarak açılacaktır. Günleriniz başarıyla kaydedildi!", "Kaydeldi!");
                    }
                    catch (Win32Exception ex)
                    {
                        if (ex.NativeErrorCode == ERROR_CANCELLED)
                            MessageBox.Show("Yönetici izni vermediğiniz için programı ayarlayamadık. Tekrardan kaydet butonuna basıp, açılan pencerede yönetici olarak çalıştırmaya izin vermelisiniz.", "HATA!");
                        else
                            throw;
                    }

                }


            }
            else if (btnKaydet.Text == "Uyanma Modunu Kapat")
            {
                try
                {
                    // Zamanlanmış görevi kaldır
                    batCalistir("pcackaldir.bat");

                    // GUI Düzenlemesini yap
                    panelAyar.Enabled = true;
                    groupBox2.Enabled = true;
                    panelInfo.BackColor = ColorTranslator.FromHtml("#a92037");
                    btnKaydet.Text = "Uyanma Modunu Aktif Et";
                    btnKaydet.BackColor = Color.ForestGreen;
                    btnKaydet.FlatAppearance.BorderColor = Color.DarkGreen;
                    label2.Text = "Uyanma modu aktif değil. Yukarıdan günleri seçerek \"Uyanma Modunu Aktif Et\" butonuna basınız.";
                    btnSleep.Visible = false;
                    groupBox3.Enabled = true;
                    groupBox4.Enabled = true;
                    panelUyanisAyar.Enabled = true;

                    //Ayarları çek
                    ayar1 = File.ReadAllLines(@"Kaynaklar\ayarlar.txt");
                    ayar = ayar1[0].Split('|');
                    ayar[0] = "0";
                    ayar[8] = "0-3-0";

                    // Ayarları kaydet
                    StreamWriter sw1 = new StreamWriter(@"Kaynaklar\ayarlar.txt");
                    sw1.Write(ayar[0] + "|" + ayar[1] + "|" + ayar[2] + "|" + ayar[3] + "|" + ayar[4] + "|" + ayar[5] + "|" + ayar[6] + "|" + ayar[7] + "|" + ayar[8] + "|" + ayar[9]);
                    sw1.Close();
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == ERROR_CANCELLED)
                        MessageBox.Show("Yönetici izni vermediğiniz için programı uyanma modunu kapatamadık. Tekrardan 'Günleri Düzenle' butonuna basıp, açılan pencerede yönetici olarak çalıştırmaya izin vermelisiniz.", "HATA!");
                    else
                        throw;
                }

            }


        }

        // Program Ekle Butonu
        private void btnProgramEkle_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            DialogResult r = ofd.ShowDialog();

            if (r == DialogResult.OK)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = ofd.SafeFileName;
                lvi.SubItems.Add(ofd.FileName);
                listView1.Items.Add(lvi);

                StreamWriter sw = new StreamWriter(@"Kaynaklar\programlar.txt", true);
                sw.WriteLine(ofd.SafeFileName + "¡" + ofd.FileName);
                sw.Close();

            }
            else
            {

            }
        }

        // Programı Kaldır Seçeneği
        private void kALDIRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2)
            {
                while (listView1.SelectedIndices.Count > 0)
                {
                    listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
                }
                StreamWriter sw = new StreamWriter(@"Kaynaklar\programlar.txt");
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    sw.WriteLine(listView1.Items[i].SubItems[0].Text + "¡" + listView1.Items[i].SubItems[1].Text);
                }
                sw.Close();
            }
            else if (tabControl1.SelectedTab == tabPage7)
            {
                while (listViewData.SelectedIndices.Count > 0)
                {
                    listViewData.Items.RemoveAt(listViewData.SelectedIndices[0]);
                }
                StreamWriter sw = new StreamWriter(@"Kaynaklar\verikullanimi.txt");
                for (int i = listViewData.Items.Count - 1; i >= 0; i--)
                {
                    sw.WriteLine(listViewData.Items[i].SubItems[0].Text + "=" + listViewData.Items[i].SubItems[1].Text + "=" + listViewData.Items[i].SubItems[2].Text + "=" + listViewData.Items[i].SubItems[3].Text);
                }
                sw.Close();
            }


        }

        // Zamanlanmış görevleri kontrol et.
        private void timer1_Tick(object sender, EventArgs e)
        {
            int saat = DateTime.Now.Hour;
            int dakika = DateTime.Now.Minute;

            sonDown = dataKullaniminiCek(index)[0];
            sonUp = dataKullaniminiCek(index)[1];

            lblDownInfo.Text = "Download: " + Convert.ToInt32(sonDown-ilkDown) + "MB" ;
            lblUpInfo.Text = "Upload: " + Convert.ToInt32(sonUp - ilkUp) + "MB";
            lblGuncelleme.Text = "Aralık: " + progBaslangic + " - " + saat.ToString("D2") + ":" + dakika.ToString("D2");

            

            if (radioButton3.Checked) // Uyanma modu aktifse
            {
                //Uyanma saati 02:00'sa
                if (radioButton5.Checked)
                {

                    // Programları 02:05'de çalıştır.
                    if (saat == 2 && dakika == 5)
                    {
                        
                        baslangicDownloadData = dataKullaniminiCek(index)[0];
                        baslangicUploadData = dataKullaniminiCek(index)[1];

                        string[] programlar = File.ReadAllLines(@"Kaynaklar\programlar.txt");
                        for (int i = 0; i < programlar.Length; i++)
                        {
                            string[] programBilgi = programlar[i].Split('¡');
                            System.Diagnostics.Process.Start(programBilgi[1]);

                        }

                        if (ayar[9] == "2")
                        {
                            batCalistir("w-on.bat");
                        }

                    }
                }
                else if (radioButton6.Checked) // Uyanma saati varsayılan DEĞİLSE
                {
                    int cmbSaat = cmbUyanisSaat.SelectedIndex;
                    int cmbDakika = cmbUyanisDakika.SelectedIndex;

                    if (cmbDakika >= 55)
                    {
                        cmbSaat = cmbSaat + 1;
                    }
                    cmbDakika = (cmbDakika + 5) % 60;

                    if (saat == cmbSaat && dakika == cmbDakika)
                    {
                       
                        baslangicDownloadData = dataKullaniminiCek(index)[0];
                        baslangicUploadData = dataKullaniminiCek(index)[1];

                        string[] programlar = File.ReadAllLines(@"Kaynaklar\programlar.txt");
                        for (int i = 0; i < programlar.Length; i++)
                        {
                            string[] programBilgi = programlar[i].Split('¡');
                            System.Diagnostics.Process.Start(programBilgi[1]);

                        }

                    }
                }


            }
            else if (radioButton4.Checked) // İnternet modu aktifse
            {
                if(rdbIntAcDef.Checked) // interneti açma saati 02:00'sa
                {
                    if (saat == 2 && dakika == 1)
                    {
                        internetiAc();

                        
                        baslangicDownloadData = dataKullaniminiCek(index)[0];
                        baslangicUploadData = dataKullaniminiCek(index)[1];

                        string[] programlar = File.ReadAllLines(@"Kaynaklar\programlar.txt");
                        for (int i = 0; i < programlar.Length; i++)
                        {
                            string[] programBilgi = programlar[i].Split('¡');
                            System.Diagnostics.Process.Start(programBilgi[1]);

                        }

                        if (ayar[9] == "2")
                        {
                            batCalistir("w-on.bat");
                        }

                    }
                }
                else if (rdbIntAcMan.Checked) // interneti açma saati varsayılan değilse
                {
                    if (saat == cmbIntAcSaat.SelectedIndex && dakika == cmbIntAcDakika.SelectedIndex)
                    {
                        internetiAc();

                        int cmbSaat = cmbIntAcSaat.SelectedIndex;
                        int cmbDakika = cmbIntAcDakika.SelectedIndex;

                        if (cmbDakika >= 55)
                        {
                            cmbSaat = cmbSaat + 1;
                        }
                        // cmbDakika = (cmbDakika + 5) % 60;

                        if (saat == cmbSaat && dakika == cmbDakika)
                        {
                            
                            baslangicDownloadData = dataKullaniminiCek(index)[0];
                            baslangicUploadData = dataKullaniminiCek(index)[1];

                            string[] programlar = File.ReadAllLines(@"Kaynaklar\programlar.txt");
                            for (int i = 0; i < programlar.Length; i++)
                            {
                                string[] programBilgi = programlar[i].Split('¡');
                                System.Diagnostics.Process.Start(programBilgi[1]);

                            }

                            if (ayar[9] == "2")
                            {
                                batCalistir("w-on.bat");
                            }

                        }
                    }
                }
            }

            if (radioButton3.Checked) // kapanış saati uyanma modu 
            {
                // Bilgisayar ne zaman kapansın?
                if (radioButton1.Checked == true)
                {
                    if (saat == 7 && dakika == 55)
                    {
                        string aralik = null;
                       
                        kapanisDownloadData = dataKullaniminiCek(index)[0];
                        kapanisUploadData = dataKullaniminiCek(index)[1];

                        int DownloadKullanim = Convert.ToInt32(kapanisDownloadData - baslangicDownloadData);
                        int UploadKullanim = Convert.ToInt32(kapanisUploadData - baslangicUploadData);

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = DateTime.Now.ToShortDateString();

                        if (radioButton5.Checked)
                        {
                            aralik = "02:05 - 07:55";
                        }
                        else if (radioButton6.Checked)
                        {
                            aralik = cmbUyanisSaat.SelectedItem.ToString() + ":" + cmbUyanisDakika.SelectedItem.ToString() + " - 07:55";
                        }
                        lvi.SubItems.Add(aralik);
                        lvi.SubItems.Add(DownloadKullanim + "");
                        lvi.SubItems.Add(UploadKullanim + "");
                        listViewData.Items.Insert(0, lvi);

                        StreamWriter sw = new StreamWriter(@"Kaynaklar\verikullanimi.txt", true);
                        sw.WriteLine(DateTime.Now.ToShortDateString() + "=" + aralik + "=" + DownloadKullanim + "=" + UploadKullanim);
                        sw.Close();

                        if (ayar[9] == "2")
                        {
                            batCalistir("w-off.bat");
                        }

                        System.Diagnostics.Process.Start("Shutdown", "-s -t 10");
                    }
                }
                else if (radioButton2.Checked == true)
                {
                    if (saat == Convert.ToInt32(cmbSaat.SelectedItem.ToString()) && dakika == Convert.ToInt32(cmbDakika.SelectedItem.ToString()))
                    {
                        string aralik = null;

                        
                        kapanisDownloadData = dataKullaniminiCek(index)[0];
                        kapanisUploadData = dataKullaniminiCek(index)[1];
                        int DownloadKullanim = Convert.ToInt32(kapanisDownloadData - baslangicDownloadData);
                        int UploadKullanim = Convert.ToInt32(kapanisUploadData - baslangicUploadData);

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = DateTime.Now.ToShortDateString();
                        if (radioButton5.Checked)
                        {
                            aralik = "02:05 - " + cmbSaat.SelectedItem.ToString() + ":" + cmbDakika.SelectedItem.ToString();
                        }
                        else if (radioButton6.Checked)
                        {
                            aralik = cmbUyanisSaat.SelectedItem.ToString() + ":" + cmbUyanisDakika.SelectedItem.ToString() + " - " + cmbSaat.SelectedItem.ToString() + ":" + cmbDakika.SelectedItem.ToString();
                        }
                        lvi.SubItems.Add(aralik);
                        lvi.SubItems.Add(DownloadKullanim + "");
                        lvi.SubItems.Add(UploadKullanim + "");
                        listViewData.Items.Insert(0, lvi);

                        StreamWriter sw = new StreamWriter(@"Kaynaklar\verikullanimi.txt", true);
                        sw.WriteLine(DateTime.Now.ToShortDateString() + "=" + aralik + "=" + DownloadKullanim + "=" + UploadKullanim);
                        sw.Close();

                        if (ayar[9] == "2")
                        {
                            batCalistir("w-off.bat");
                        }

                        System.Diagnostics.Process.Start("Shutdown", "-s -t 10");
                    }
                }
            }
            else if (radioButton4.Checked)  { // kapanış saati internet modu


                // Bilgisayar ne zaman kapansın?
                if (rdbIntSaatDef.Checked == true)
                {
                    if (saat == 7 && dakika == 55)
                    {
                        string aralik = null;
                        
                        kapanisDownloadData = dataKullaniminiCek(index)[0];
                        kapanisUploadData = dataKullaniminiCek(index)[1];

                        int DownloadKullanim = Convert.ToInt32(kapanisDownloadData - baslangicDownloadData);
                        int UploadKullanim = Convert.ToInt32(kapanisUploadData - baslangicUploadData);

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = DateTime.Now.ToShortDateString();

                        if (rdbIntAcDef.Checked)
                        {
                            aralik = "02:05 - 07:55";
                        }
                        else if (rdbIntAcMan.Checked)
                        {
                            aralik = cmbIntAcSaat.SelectedItem.ToString() + ":" + cmbIntAcDakika.SelectedItem.ToString() + " - 07:55";
                        }
                        lvi.SubItems.Add(aralik);
                        lvi.SubItems.Add(DownloadKullanim + "");
                        lvi.SubItems.Add(UploadKullanim + "");
                        listViewData.Items.Insert(0, lvi);

                        StreamWriter sw = new StreamWriter(@"Kaynaklar\verikullanimi.txt", true);
                        sw.WriteLine(DateTime.Now.ToShortDateString() + "=" + aralik + "=" + DownloadKullanim + "=" + UploadKullanim);
                        sw.Close();

                        if (ayar[9] == "2")
                        {
                            batCalistir("w-off.bat");
                        }

                        System.Diagnostics.Process.Start("Shutdown", "-s -t 10");
                    }
                }
                else if (rdbIntSaatMan.Checked == true)
                {
                    if (saat == Convert.ToInt32(cmbIntKapSaat.SelectedItem.ToString()) && dakika == Convert.ToInt32(cmbIntKapDakika.SelectedItem.ToString()))
                    {
                        string aralik = null;

                        
                        kapanisDownloadData = dataKullaniminiCek(index)[0];
                        kapanisUploadData = dataKullaniminiCek(index)[1];
                        int DownloadKullanim = Convert.ToInt32(kapanisDownloadData - baslangicDownloadData);
                        int UploadKullanim = Convert.ToInt32(kapanisUploadData - baslangicUploadData);


                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = DateTime.Now.ToShortDateString();
                        if (rdbIntAcDef.Checked)
                        {
                            aralik = "02:05 - " + cmbIntKapSaat.SelectedItem.ToString() + ":" + cmbIntKapDakika.SelectedItem.ToString();
                        }
                        else if (rdbIntSaatMan.Checked)
                        {
                            aralik = cmbIntAcSaat.SelectedItem.ToString() + ":" + cmbIntAcDakika.SelectedItem.ToString() + " - " + cmbIntKapSaat.SelectedItem.ToString() + ":" + cmbIntKapDakika.SelectedItem.ToString();
                        }
                        lvi.SubItems.Add(aralik);
                        lvi.SubItems.Add(DownloadKullanim + "");
                        lvi.SubItems.Add(UploadKullanim + "");
                        listViewData.Items.Insert(0, lvi);

                        StreamWriter sw = new StreamWriter(@"Kaynaklar\verikullanimi.txt", true);
                        sw.WriteLine(DateTime.Now.ToShortDateString() + "=" + aralik + "=" + DownloadKullanim + "=" + UploadKullanim);
                        sw.Close();

                        if (ayar[9] == "2")
                        {
                            batCalistir("w-off.bat");
                        }

                      System.Diagnostics.Process.Start("Shutdown", "-s -t 10");
                    }
                }

            }

        }

        // Kapanış saati "ben seçeceğim" olarak seçiliyse
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            panelSaat.Visible = true;
            panelSaat.Enabled = true;
            cmbSaat.SelectedIndex = 9;
            cmbDakika.SelectedIndex = 5;
            lblSaatBilgisi.Text = "Saat/Dakika değişikliği otomatik kaydedilecektir.";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            panelSaat.Visible = false;
            panelSaat.Enabled = false;
        }

        // İnterneti kes yada aç butonu
        private void button1_Click(object sender, EventArgs e)
        {
            if (btnKillNet.Text == "Ben Yatıyorum Sen Hallet")
            {

                internetiKapat();

            }
            else
            {
                internetiAc();
            }
        }

        public void internetiKapat()
        {
            btnKillNet.BackColor = Color.Green;
            btnKillNet.Text = "Uykum Kaçtı Neti Aç";

            btnSleep.Text = "Uykum Kaçtı Neti Aç";
            btnSleep.BackColor = Color.Green;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "ipconfig";
            info.Arguments = "/release"; // or /release if you want to disconnect
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(info);
            p.WaitForExit();
        }
        public void internetiAc()
        {
            btnKillNet.BackColor = Color.DodgerBlue;
            btnKillNet.Text = "Ben Yatıyorum Sen Hallet!";

            btnSleep.Text = "Ben Yatıyorum Sen Hallet!";
            btnSleep.BackColor = Color.DodgerBlue;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "ipconfig";
            info.Arguments = "/renew"; // or /release if you want to disconnect
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(info);
            p.WaitForExit();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            btnSleep.Visible = false;
            panelAyar.Enabled = false;
            btnKaydet.Visible = false;
            btnKillNet.Visible = true;

            if (tabControl1.SelectedIndex != 1)
            {
                tabControl1.SelectedIndex = 1;
            }

            label41.BackColor = Color.Green;
            label41.Text = "İnternet modu aktif! Başlangıç saatinde programlarınız çalıştırılacaktır.";
        }

        private void btnLogSil_Click(object sender, EventArgs e)
        {
            listViewData.Items.Clear();
            System.IO.File.WriteAllText(@"Kaynaklar\verikullanimi.txt", string.Empty);
        }

        // Uyanış 02:00 seçiliyse
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            panelUyanisAyar.Visible = false;
        }

        // Uyanış farklı bir saate ayarlıysa
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {


            panelUyanisAyar.Visible = true;


            if (ayar[8].Split('-')[0] == "0")
            {
                int sistemSaat = DateTime.Now.Hour;
                int sistemDakika = DateTime.Now.Minute;

                if (sistemDakika >= 55)
                {
                    sistemDakika = (sistemDakika + 5) % 60;
                    sistemSaat++;
                }
                else
                {
                    sistemDakika += 5;
                }

                cmbUyanisSaat.SelectedIndex = sistemSaat;
                cmbUyanisDakika.SelectedIndex = sistemDakika;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex != 0)
            {
                tabControl1.SelectedIndex = 0;
            }


            if (ayar[0] == "0")
            {
                btnSleep.Visible = false;
            }
            panelAyar.Enabled = true;
            btnKaydet.Visible = true;
            btnKillNet.Visible = false;

            label41.BackColor = Color.Maroon;
            label41.Text = "Uyanma modu aktif değil! Çalışma modunu değiştirmelisiniz.";
        }

        private void rdbIntAcMan_CheckedChanged(object sender, EventArgs e)
        {
            pnlIntAc.Visible = true;

            int sistemSaat = DateTime.Now.Hour;
            int sistemDakika = DateTime.Now.Minute;

            if (sistemDakika >= 55)
            {
                sistemDakika = (sistemDakika + 5) % 60;
                sistemSaat++;
            }
            else
            {
                sistemDakika += 5;
            }

            cmbIntAcSaat.SelectedIndex = sistemSaat;
            cmbIntAcDakika.SelectedIndex = sistemDakika;
        }

        private void rdbIntAcDef_CheckedChanged(object sender, EventArgs e)
        {
            pnlIntAc.Visible = false;
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
            this.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                notifyIcon1.BalloonTipTitle = "2-8 Asistan Veri Kullanımı";
                notifyIcon1.BalloonTipText = "" + lblGuncelleme.Text + "\n" + lblDownInfo.Text + "\n" + lblUpInfo.Text;
                notifyIcon1.ShowBalloonTip(1000);
                notifyIcon1.Text = "2-8 Asistan";
            }
           
        }

        private void rdbIntSaatDef_CheckedChanged(object sender, EventArgs e)
        {
            pnlIntKapat.Visible = false;
        }

        private void rdbIntSaatMan_CheckedChanged(object sender, EventArgs e)
        {
            pnlIntKapat.Visible = true;
            cmbIntKapSaat.SelectedIndex = 9;
            cmbIntKapDakika.SelectedIndex = 5;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.dijitaller.com/");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (txtİndirmeHizi.Text.Trim() == "" && txtDosyaBoyutu.Text.Trim() == "")
            {
                MessageBox.Show("İndirilecek dosya boyutu ve ortlama indirme hızınızı doldurmanız gerekmekte!", "Hata!");
            }
            else
            {
                try
                {

                    double hiz = Double.Parse(txtİndirmeHizi.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                    double dosyaboyutu = Double.Parse(txtDosyaBoyutu.Text.Replace(",", "."), CultureInfo.InvariantCulture);


                    double hesap = (((dosyaboyutu * 1000) * 1024) / (1000 * hiz)) / 60;
                    int hesap2 = (int)hesap;

                    if (hesap2 < 60)
                    {
                        MessageBox.Show("İndirmeniz " + hesap2 + " dakika sürecektir.", "İndirme Saati");
                    }
                    else if (hesap2 >= 1140)
                    {
                        int gun = hesap2 % (24 * 60);
                        int gun2 = hesap2 / (24 * 60);
                        int saat = (hesap2 - (gun2 * 60 * 24)) / 60;
                        int dakika = ((hesap2 - saat) % 60);
                        MessageBox.Show("İndirmeniz " + gun2 + " gün " + saat + " saat " + dakika + " dakika sürecektir.", "İndirme Saati");
                    }
                    else
                    {
                        int dakika = hesap2 % 60;
                        int saat = (hesap2 - dakika) / 60;
                        MessageBox.Show("İndirmeniz " + saat + " saat " + dakika + " dakika sürecektir.", "İndirme Saati");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Değerleri doğru girdiğinizden emin olun!", "Hata!");
                    txtDosyaBoyutu.Clear();
                    txtİndirmeHizi.Clear();
                }
            }
        }

        private void btnWindowsUpdateKaydet_Click(object sender, EventArgs e)
        {
            if (rdbWinUpdateAcik.Checked)
            {
                ayar[9] = "0";
                batCalistir("w-on.bat");
            }
            else if (rdbWinUpdateKapali.Checked)
            {
                ayar[9] = "1";
                batCalistir("w-off.bat");
            }
            else if (rdbWinUpdateOzel.Checked)
            {
                ayar[9] = "2";
                batCalistir("w-off.bat");
            }

            StreamWriter sw1 = new StreamWriter(@"Kaynaklar\ayarlar.txt");
            sw1.Write(ayar[0] + "|" + ayar[1] + "|" + ayar[2] + "|" + ayar[3] + "|" + ayar[4] + "|" + ayar[5] + "|" + ayar[6] + "|" + ayar[7] + "|" + ayar[8] + "|" + ayar[9]);
            sw1.Close();

            MessageBox.Show("Windows update ayarlarınız başarıyla kaydedildi!", "Kaydedildi!");
        }

        public void batCalistir(String DosyaBilgi)
        {
            ProcessStartInfo info = new ProcessStartInfo(@"Kaynaklar\" + DosyaBilgi + "");
            info.UseShellExecute = true;
            info.Verb = "runas";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info);
        }

        public long[] dataKullaniminiCek (int index)
        {
            long[] datalar = new long[2];

            IPv4InterfaceStatistics interfaceStats = NetworkInterface.GetAllNetworkInterfaces()[index].GetIPv4Statistics();
            datalar[0] = interfaceStats.BytesReceived / 1048576;
            datalar[1] = interfaceStats.BytesSent / 1048576;

            return datalar;
        }

    }
}