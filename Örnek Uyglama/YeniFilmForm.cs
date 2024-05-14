using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Örnek_Uyglama
{
    public partial class YeniFilmForm : Form
    {
        string baglanti = "Server=localhost;Database=film_arsiv;Uid=root;Pwd=;";
        string yeniAd = null;
        public YeniFilmForm()
        {
            InitializeComponent();
        }

        private void YeniFilmForm_Load(object sender, EventArgs e)
        {
            CmbDoldur();

        }

        private void CmbDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT DISTINCT tur FROM filmler;";
                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbTur.DataSource = dt;

                cmbTur.DisplayMember = "tur";
                cmbTur.ValueMember = "tur";


            }
        }

        private void pbPoster_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";


            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                string kaynakYol = openFileDialog.FileName;
                yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakYol);
                string hedefYol = Path.Combine(Environment.CurrentDirectory,"poster",yeniAd);

                File.Copy(kaynakYol, hedefYol);

                pbPoster.ImageLocation = hedefYol;
                pbPoster.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();

                string sorgu = "INSERT INTO filmler VALUES(NULL,@filmad,@yonetmen,@yil,@tur,@sure,@poster,@puan,@odul);";
                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                cmd.Parameters.AddWithValue("@filmad",txtFilmAd.Text);
                cmd.Parameters.AddWithValue("@yonetmen", txtYonetmen.Text);
                cmd.Parameters.AddWithValue("@yil",Convert.ToInt32(txtYil.Text));
                cmd.Parameters.AddWithValue("@tur",cmbTur.SelectedValue);
                cmd.Parameters.AddWithValue("@sure",txtSure.Text);
                cmd.Parameters.AddWithValue("@poster",yeniAd);
                cmd.Parameters.AddWithValue("@puan",txtPuan.Text);
                cmd.Parameters.AddWithValue("@odul",cbOdul.Checked);

                if (cmd.ExecuteNonQuery() >0)
                {
                    MessageBox.Show("Kayıt Eklendi");
                    this.Close();

                }

            }
        }
    }
}
