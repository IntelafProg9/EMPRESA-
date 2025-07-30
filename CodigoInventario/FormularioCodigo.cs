using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodigoInventario
{
    public partial class FormularioCodigo : Form
    {
        public FormularioCodigo()
        {
            InitializeComponent();
            btnBuscar.Click += btnBuscar_Click;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            lstSerie.Items.Clear();

            string codigo = txtCodigo.Text.Trim();
            if (string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Tienes que ingresar un código.");
                return;
            }
            string connStr = "Data Source=192.168.220.241;Initial Catalog=INTELAF BODEGA;User ID=sa;Password=Intelaf1;";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT 
                            p.CODIGO_PROVEEDOR,
                            p.CODIGO,
                            sa.SN
                         FROM PROVAINT p
                         INNER JOIN SN_APROV sa ON p.PROVPDCOD = sa.PROVPDCOD
                         WHERE p.CODIGO = @Codigo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        bool existe = false;
                        while (reader.Read())
                        {
                            existe = true;
                            string item = $"{reader["CODIGO"]} -- {reader["CODIGO_PROVEEDOR"]} -- {reader["SN"]}";
                            lstSerie.Items.Add(item);
                        }
                        if (!existe)
                        {
                            MessageBox.Show("El código que ingreso no existe.");
                        }
                    }
                }
            }
        }
    }
}