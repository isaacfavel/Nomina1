using Newtonsoft.Json;
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

namespace Nomina.Formulario
{
    public partial class Form2 : Form
    {
        private void cargarDatos() {
            string archivo = "datos.json";
            if (File.Exists(archivo))
            {
                try { 
                    string jsonString=File.ReadAllText(archivo);
                    Datos datos=JsonConvert.DeserializeObject<Datos>(jsonString);
                    if (datos != null)
                    {
                        txtRegular.Text = datos.regular.ToString();
                        txtOver.Text=datos.overtime.ToString();
                        txtDouble.Text=datos.doble.ToString();

                    }
                }catch(Exception ex) { 
                    MessageBox.Show("No se pudieron cargar los datos"+ex.Message,"Sistema",MessageBoxButtons.OK,MessageBoxIcon.Information);
                
                }
            }
        }
        public Form2()
        {
            InitializeComponent();
            cargarDatos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                double regular = double.Parse(txtRegular.Text);
                double overtime = double.Parse(txtOver.Text);
                double doble = double.Parse(txtDouble.Text);


                var datos = new Datos
                {
                    regular = regular,
                    overtime = overtime,
                    doble = doble
                };

                string archivo = "datos.json";

                string jsonString = JsonConvert.SerializeObject(datos, Formatting.Indented);
                
                File.WriteAllText(archivo, jsonString);
                MessageBox.Show("Datos guardados correctamente","Exito",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Close();
                    }
            catch(Exception ex)
            {
                MessageBox.Show("Los datos no se guardaron", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
