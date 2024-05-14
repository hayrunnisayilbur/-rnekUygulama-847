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
    public partial class Form1 : Form
    {
        string baglanti = "Server=localhost;Database=film_arsiv;Uid=root;Pwd=;";

        public Form1()
        {
            InitializeComponent();
        }


        public void DgwDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT * FROM filmler;";
                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgwFilmler.DataSource = dt;

                dgwFilmler.Columns["yonetmen"].Visible = false;
                dgwFilmler.Columns["yil"].Visible = false;
                dgwFilmler.Columns["poster"].Visible = false;
                dgwFilmler.Columns["film_odul"].Visible = false;


            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(!Directory.Exists("poster")) {
                Directory.CreateDirectory("poster");
            }

            DgwDoldur();
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

        private void dgwFilmler_SelectionChanged(object sender, EventArgs e)
        {
            if(dgwFilmler.SelectedRows.Count > 0)
            {
                txtFilmAd.Text = dgwFilmler.SelectedRows[0].Cells["film_ad"].Value.ToString();
                txtYonetmen.Text = dgwFilmler.SelectedRows[0].Cells["yonetmen"].Value.ToString();
                cmbTur.SelectedValue = dgwFilmler.SelectedRows[0].Cells["tur"].Value.ToString();
                txtYil.Text = dgwFilmler.SelectedRows[0].Cells["yil"].Value.ToString();
                txtSure.Text = dgwFilmler.SelectedRows[0].Cells["sure"].Value.ToString();
                txtPuan.Text = dgwFilmler.SelectedRows[0].Cells["imdb_puan"].Value.ToString();
                cbOdul.Checked = Convert.ToBoolean(dgwFilmler.SelectedRows[0].Cells["film_odul"].Value);

                string posterYol = Path.Combine(Environment.CurrentDirectory,"poster", dgwFilmler.SelectedRows[0].Cells["poster"].Value.ToString());
                if(File.Exists(posterYol))
                {
                    pbPoster.ImageLocation = posterYol;
                    pbPoster.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    pbPoster.ImageLocation= null;
                }

            }
        }

        private void btnYeniFilmForm_Click(object sender, EventArgs e)
        {
            YeniFilmForm yeniFilmForm = new YeniFilmForm();
            yeniFilmForm.ShowDialog();
            DgwDoldur();
            
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dgwFilmler.SelectedRows.Count > 0)
            {
                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    baglan.Open();
                    string sorgu = "DELETE FROM filmler WHERE film_id=@satirid";
                    MySqlCommand cmd = new MySqlCommand(sorgu,baglan);
                    cmd.Parameters.AddWithValue("@satirid", dgwFilmler.SelectedRows[0].Cells["film_id"].Value.ToString());

                    //Resim silme resimyol
                    string posterYol = Path.Combine(Environment.CurrentDirectory, "poster", dgwFilmler.SelectedRows[0].Cells["poster"].Value.ToString());

                    string filmAd = dgwFilmler.SelectedRows[0].Cells["film_ad"].Value.ToString();
                    if (DialogResult.Yes == MessageBox.Show("Film Adı:"+filmAd, "Film Sil",MessageBoxButtons.YesNo,MessageBoxIcon.Error))
                    {
                        cmd.ExecuteNonQuery();
                        DgwDoldur();

                        //resim silme işlemi
                        if(File.Exists(posterYol))
                        {
                            File.Delete(posterYol);
                        }

                    }
                }
            }
        }
    }
}
