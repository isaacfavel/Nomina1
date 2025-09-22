using Newtonsoft.Json;
using Nomina.Formulario;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace Nomina
{
    public partial class Form1 : Form
    {
        List<string> columnas;
        public Form1()
        {
            InitializeComponent();
            columnas = new List<string>();
            columnas.Add("EE");
            columnas.Add("Nombre");
            columnas.Add("Apellido");
            columnas.Add("Rg. Hrs");
            columnas.Add("OT. Hrs.");
            columnas.Add("D Hrs.");
        }

        private void importarExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdExcel.ShowDialog() == DialogResult.OK)
            {
                string archivo = ofdExcel.FileName;
                //MessageBox.Show("Archivo seleccionado: " + archivo);
                CargarExcel(archivo);
                multiplicarDatos();

            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tmrReloj_Tick(object sender, EventArgs e)
        {
            ttsHora.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void CargarExcel(string path)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Isaac Romero Favela");

            using (var package = new ExcelPackage(new System.IO.FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    MessageBox.Show("El archivo de Excel no contiene hojas de trabajo.");
                    return; //En caso que no haya hojas
                }

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                DataTable dt = new DataTable();

                // Leer los encabezados de columna
                foreach (var col in columnas)
                {
                    dt.Columns.Add(col);
                }

                // Leer las filas de datos
                int rowCount = 0;
                for (int i = 3; i < worksheet.Dimension.End.Row; i++)
                {
                    if (worksheet.Cells[i, 1].Text == "")
                        break;
                    else
                        rowCount = rowCount + 1;
                }

                rowCount = rowCount + 3;
                for (int i = 3; i < rowCount; i++)
                {
                    DataRow row = dt.NewRow();
                    int p = 1;
                    for (int j = 1; j < dt.Columns.Count + 1; j++)
                    {
                        if (worksheet.Cells[i, p].Text == "")
                            break;
                        row[j - 1] = worksheet.Cells[i, p].Text;
                        if (j == 2) {
                            string[] partes = SepararNombre(worksheet.Cells[i, p].Text);
                            row[1] = partes[1];
                            row[2] = partes[0];
                            j++;

                        }
                        p++;
                    }
                    dt.Rows.Add(row);
                }

                //Mostrar los datos en el DataGridView
                dgvInformacion.Rows.Clear();
                dgvInformacion.Rows.Add(dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        dgvInformacion.Rows[i].Cells[j].Value = dt.Rows[i][j].ToString();
                    }
                }

            }



        }

        private string[] SepararNombre(string nombreCompleto)
        {

            string[] partes = nombreCompleto.Split(',');
            if (partes.Length >= 2)
            {
                string apellido = partes[0];
                string nombre = partes[1];
                partes[1] = nombre.Trim();
            }
            return partes;
        }

        private void datosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.ShowDialog();
        }

        public void multiplicarDatos()
        {
            string archivo = "datos.json";
            if (File.Exists(archivo))
            {
                try
                {
                    string jsonString = File.ReadAllText(archivo);
                    Datos datos = JsonConvert.DeserializeObject<Datos>(jsonString);
                    if (datos != null)
                    {
                        double regular = Double.Parse(datos.regular.ToString());
                        double Over = Double.Parse(datos.overtime.ToString());
                        double doble = Double.Parse(datos.doble.ToString());

                        for (int i = 0; i < dgvInformacion.RowCount; i++)
                        {
                            double valor = Convert.ToDouble(dgvInformacion[3, i].Value);
                            double valor1 = Convert.ToDouble(dgvInformacion[4, i].Value);
                            double valor2 = Convert.ToDouble(dgvInformacion[5, i].Value);
                            dgvInformacion[6, i].Value = valor * regular;
                            dgvInformacion[7, i].Value = valor1 * Over;
                            dgvInformacion[8, i].Value = valor2 * doble;
                        }

                        for (int i = 0; i < dgvInformacion.RowCount; i++)
                        {
                            double valor = Convert.ToDouble(dgvInformacion[6, i].Value);
                            double valor1 = Convert.ToDouble(dgvInformacion[7, i].Value);
                            double valor2 = Convert.ToDouble(dgvInformacion[8, i].Value);
                            dgvInformacion[9, i].Value = valor + valor1 + valor2;
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudieron cargar los datos" + ex.Message, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
        }


    }
}