using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Estacionamento.Data;
using Estacionamento.Enums;
using Estacionamento.Models;

namespace Estacionamento.Repositories
{
    public class VeiculoRepository
    {
        public int Inserir(Veiculo veiculo)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"INSERT INTO Veiculos (Placa, Modelo, Cor, TipoVeiculo) 
                                   VALUES (@Placa, @Modelo, @Cor, @Tipo);
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Placa", veiculo.Placa);
                        cmd.Parameters.AddWithValue("@Modelo", veiculo.Modelo);
                        cmd.Parameters.AddWithValue("@Cor", veiculo.Cor);
                        cmd.Parameters.AddWithValue("@Tipo", (int)veiculo.Tipo);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        veiculo.Id = id;
                        return id;
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Já existe um veículo com essa placa.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir veículo: {ex.Message}");
            }
        }

        public Veiculo BuscarPorPlaca(string placa)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = "SELECT Id, Placa, Modelo, Cor, TipoVeiculo FROM Veiculos WHERE Placa = @Placa";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Placa", placa.ToUpper().Trim());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CriarVeiculoDoReader(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar veículo: {ex.Message}");
            }
        }
        public Veiculo BuscarPorId(int id)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = "SELECT Id, Placa, Modelo, Cor, TipoVeiculo FROM Veiculos WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CriarVeiculoDoReader(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar veículo: {ex.Message}");
            }
        }

        public List<Veiculo> ListarTodos()
        {
            List<Veiculo> veiculos = new List<Veiculo>();

            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = "SELECT Id, Placa, Modelo, Cor, TipoVeiculo FROM Veiculos ORDER BY Placa";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            veiculos.Add(CriarVeiculoDoReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar veículos: {ex.Message}");
            }

            return veiculos;
        }

        private Veiculo CriarVeiculoDoReader(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string placa = reader.GetString(1);
            string modelo = reader.GetString(2);
            string cor = reader.GetString(3);
            TipoVeiculo tipo = (TipoVeiculo)reader.GetInt32(4);

            Veiculo veiculo;

            switch (tipo)
            {
                case TipoVeiculo.Moto:
                    veiculo = new Moto(placa, modelo, cor);
                    break;
                case TipoVeiculo.Carro:
                    veiculo = new Carro(placa, modelo, cor);
                    break;
                case TipoVeiculo.Caminhao:
                    veiculo = new Caminhao(placa, modelo, cor);
                    break;
                default:
                    throw new Exception($"Tipo de veículo desconhecido: {tipo}");
            }

            veiculo.Id = id;
            return veiculo;
        }
    }
}