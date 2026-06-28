using System;
using System.Data.SqlClient;

namespace Estacionamento.Data
{
    public static class Conexao
    {
        private static readonly string _connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=Estacionamento;Integrated Security=True;";

        public static SqlConnection Abrir()
        {
            try
            {
                SqlConnection conexao = new SqlConnection(_connectionString);
                conexao.Open();
                return conexao;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Erro ao conectar ao banco de dados: {ex.Message}");
            }
        }

        // Testa se a conexão está funcionando
        public static bool Testar()
        {
            try
            {
                using (SqlConnection conexao = Abrir())
                {
                    return conexao.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
