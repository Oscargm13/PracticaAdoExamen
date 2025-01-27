using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DEPARTAMENTOSEMPLEADOSADO.Helppers;
using PracticaAdoExamen;
using PracticaAdoExamen.Models;
using PracticaAdoExamen.Repository;

namespace Test
{
    public partial class FormPractica : Form
    {
        public FormPractica()
        {
            InitializeComponent();
            LoadClientes();
        }

        public async void LoadClientes()
        {
            RepositoryClientes repository = new RepositoryClientes(HelpperConfiguration.GetConnectionString());
            List<string> clientes = await repository.GetClientes();
            this.cmbclientes.DataSource = clientes;
        }

        private async void cmbclientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepositoryClientes repository = new RepositoryClientes(HelpperConfiguration.GetConnectionString());
            string nombre = this.cmbclientes.SelectedItem.ToString();


            Cliente cliente = await repository.GetDatosCliente(nombre);

            this.txtcontacto.Text = cliente.contacto;
            this.txtcargo.Text = cliente.cargo;
            this.txtciudad.Text = cliente.ciudad;
            this.txttelefono.Text = cliente.telefono.ToString();
            this.lstpedidos.Items.Clear();
            foreach (string pedido in cliente.pedidos)
            {
                this.lstpedidos.Items.Add(pedido);
            }
        }

        private async void lstpedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepositoryClientes repository = new RepositoryClientes(HelpperConfiguration.GetConnectionString());
            if (this.lstpedidos.SelectedItem != null)
            {
                string fecha = this.lstpedidos.SelectedItem.ToString();

                Pedido pedido = await repository.GetDatosPedido(fecha);

                if (pedido != null)
                {
                    this.txtcodigopedido.Text = pedido.CodigoPedido.ToString();
                    this.txtfechaentrega.Text = pedido.FechaEntrega.ToString();
                    this.txtformaenvio.Text = pedido.FormaEnvio.ToString();
                    this.txtimporte.Text = pedido.Importe.ToString();
                }
                else
                {
                    MessageBox.Show("No se encontró información sobre el empleado seleccionado.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }
        }

        private async void btneliminarpedido_Click(object sender, EventArgs e)
        {
            if (this.lstpedidos.SelectedItem != null)
            {
                string fecha = this.lstpedidos.SelectedItem.ToString();
                    try
                    {
                        RepositoryClientes repository = new RepositoryClientes(HelpperConfiguration.GetConnectionString());
                        Pedido pedido = await repository.GetDatosPedido(fecha);

                        if (pedido != null)
                        {
                            await repository.EliminarPedido(int.Parse(pedido.CodigoPedido));

                            this.lstpedidos.Items.Remove(this.lstpedidos.SelectedItem);

                            this.txtcodigopedido.Clear();
                            this.txtfechaentrega.Clear();
                            this.txtformaenvio.Clear();
                            this.txtimporte.Clear();

                            MessageBox.Show("Pedido eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el pedido seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar el pedido: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
           
        }

    }
}
