using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace EMPRESA
{
    public partial class Facturas : Form
    {
        public Facturas()
        {
            InitializeComponent();
            dgvFacturas.SelectionChanged += DgvFacturas_SelectionChanged;
            btnBuscar.Click += btnBuscar_Click;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            dgvFacturas.DataSource = null;
            string connStr = ConfigurationManager.ConnectionStrings["ConexionPrueba"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"SELECT 
                                g.NO_FACT AS NUMERO_FACTURA,
                                g.FAC_SERIE AS SERIE_FACTURA,
                                MIN(d.FECHA_FAC) AS FECHA_FAC, 
                                SUM(d.PROD_CANT * d.Valor_tot_desc_aplicado) AS TOTAL_CON_DESCUENTO
                             FROM FACT_GEN g
                             INNER JOIN FACT_DET d ON g.NO_FACT = d.NO_FACT
                             WHERE d.FECHA_FAC BETWEEN @Inicio AND @Final";
                    if (!ckbAnuladas.Checked)
                        query += " AND d.ANULADO = 0";
                    query += " GROUP BY g.NO_FACT, g.FAC_SERIE";
                    query += " ORDER BY FECHA_FAC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Inicio", dtpInicio.Value.Date);
                        cmd.Parameters.AddWithValue("@Final", dtpFin.Value.Date);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvFacturas.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar las facturas: " + ex.Message);
            }
        }
        private void DgvFacturas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFacturas.SelectedRows.Count > 0)
            {
                var noFact = dgvFacturas.SelectedRows[0].Cells["NUMERO_FACTURA"].Value.ToString();
                var facSerie = dgvFacturas.SelectedRows[0].Cells["SERIE_FACTURA"].Value.ToString();
                MostrarDetalleFactura(noFact, facSerie);
            }
        }

        private void MostrarDetalleFactura(string noFact, string facSerie)
        {
            dvgDetalles.DataSource = null;
            string connStr = ConfigurationManager.ConnectionStrings["ConexionPrueba"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"SELECT 
                                PROD_COD AS PRODUCTOS, 
                                PROD_CANT AS CANTIDAD_PRODUCTOS, 
                                Precio_original AS PRECIO
                             FROM FACT_DET
                             WHERE NO_FACT = @NoFact AND FAC_SERIE = @FacSerie";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NoFact", noFact);
                        cmd.Parameters.AddWithValue("@FacSerie", facSerie);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dvgDetalles.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar los detalles de la factura: " + ex.Message);
            }
        }
    }
}

  

