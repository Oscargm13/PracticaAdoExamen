using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PracticaAdoExamen.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;


//CREATE PROCEDURE SP_DATOS_CLIENTE_EMPRESA2
//    @nombre NVARCHAR(50)
//AS
//BEGIN
//    -- Información del cliente
//    SELECT CodigoCliente, Empresa, Contacto, Cargo, Ciudad, Telefono
//    FROM CLIENTES
//    WHERE Empresa = @nombre;

//--Pedidos relacionados
//SELECT FechaEntrega
//    FROM PEDIDOS
//    WHERE CodigoCliente IN (SELECT CodigoCliente FROM CLIENTES WHERE Empresa = @nombre);
//END
//GO
namespace PracticaAdoExamen.Repository
{
    public class RepositoryClientes
    {
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryClientes(string connectionString)
        {
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        public async Task<List<string>> GetClientes()
        {
            string sql = "SP_ALL_CLIENTES";
            List<string> clientes = new List<string>();


            using (SqlCommand com = new SqlCommand(sql, cn))
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                await cn.OpenAsync();
                using (SqlDataReader reader = await com.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string nombre = reader["EMPRESA"].ToString();
                        clientes.Add(nombre);
                    }
                }
            }

            return clientes;
        }

        public async Task<Cliente> GetDatosCliente(string nombre)
        {

            string sql = "SP_DATOS_CLIENTE_EMPRESA2";
            this.com.Parameters.AddWithValue("@nombre", nombre);
            this.com.CommandType = System.Data.CommandType.StoredProcedure;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();

            Cliente cliente = new Cliente();

            if (await this.reader.ReadAsync())
            {
                cliente.id = this.reader["CodigoCliente"].ToString();
                cliente.empresa = this.reader["EMPRESA"].ToString();
                cliente.contacto = this.reader["CONTACTO"].ToString();
                cliente.cargo = this.reader["CARGO"].ToString();
                cliente.ciudad = this.reader["CIUDAD"].ToString();
                cliente.telefono = int.Parse(this.reader["TELEFONO"].ToString());
            }

            if (await this.reader.NextResultAsync())
            {
                cliente.pedidos = new List<string>();
                while (await this.reader.ReadAsync())
                {
                    string pedido = this.reader["FechaEntrega"].ToString();
                    cliente.pedidos.Add(pedido);
                }
            }

            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
            return cliente;
        }
        public async Task<Pedido> GetDatosPedido(string fecha)
        {
            string sql = "SELECT * FROM PEDIDOS WHERE FechaEntrega=@fecha";
            using (SqlConnection cn = new SqlConnection(this.cn.ConnectionString))
            using (SqlCommand com = new SqlCommand(sql, cn))
            {
                com.CommandType = System.Data.CommandType.Text;
                com.Parameters.AddWithValue("@fecha", fecha);

                Pedido pedido = null;

                try
                {
                    await cn.OpenAsync();
                    using (SqlDataReader reader = await com.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            pedido = new Pedido
                            {
                                CodigoPedido = reader["CodigoPedido"].ToString(),
                                CodigoCliente = reader["CodigoCliente"].ToString(),
                                FechaEntrega = reader["FechaEntrega"].ToString(),
                                FormaEnvio = reader["FormaEnvio"].ToString(),
                                Importe = int.Parse(reader["IMPORTE"].ToString())
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener los datos del empleado", ex);
                }

                return pedido;
            }
        }
        public async Task EliminarPedido(int codigoPedido)
        {
            string sql = "DELETE FROM PEDIDOS WHERE CodigoPedido = @codigoPedido";
            using (SqlConnection cn = new SqlConnection(this.cn.ConnectionString))
            using (SqlCommand com = new SqlCommand(sql, cn))
            {
                com.CommandType = System.Data.CommandType.Text;
                com.Parameters.AddWithValue("@codigoPedido", codigoPedido);

                try
                {
                    await cn.OpenAsync();
                    int rowsAffected = await com.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("No se encontró ningún pedido con el código especificado.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar el pedido", ex);
                }
            }
        }

    }
}