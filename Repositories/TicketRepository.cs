using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Estacionamento.Data;
using Estacionamento.Enums;
using Estacionamento.Models;

namespace Estacionamento.Repositories
{
    public class TicketRepository
    {
        private readonly VeiculoRepository _veiculoRepo = new VeiculoRepository();

        public int RegistrarEntrada(Ticket ticket)
        {
            try
            {
                if (TemTicketAberto(ticket.VeiculoId))
                    throw new Exception("Este veículo já possui um ticket aberto no estacionamento.");

                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"INSERT INTO Tickets (VeiculoId, Vaga, HoraEntrada, Status) 
                                   VALUES (@VeiculoId, @Vaga, @HoraEntrada, @Status);
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@VeiculoId", ticket.VeiculoId);
                        cmd.Parameters.AddWithValue("@Vaga", ticket.Vaga);
                        cmd.Parameters.AddWithValue("@HoraEntrada", ticket.HoraEntrada);
                        cmd.Parameters.AddWithValue("@Status", (int)StatusTicket.Aberto);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        ticket.Id = id;
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao registrar entrada: {ex.Message}");
            }
        }

        public void RegistrarSaida(int ticketId, decimal valorTotal)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"UPDATE Tickets 
                                   SET HoraSaida = @HoraSaida, ValorTotal = @ValorTotal, Status = @Status 
                                   WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@HoraSaida", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ValorTotal", valorTotal);
                        cmd.Parameters.AddWithValue("@Status", (int)StatusTicket.Fechado);
                        cmd.Parameters.AddWithValue("@Id", ticketId);

                        int linhas = cmd.ExecuteNonQuery();
                        if (linhas == 0)
                            throw new Exception("Ticket não encontrado.");
                    }
                }
            }
            catch (Exception ex) when (!ex.Message.Contains("não encontrado"))
            {
                throw new Exception($"Erro ao registrar saída: {ex.Message}");
            }
        }

        public int RegistrarPagamento(Pagamento pagamento)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"INSERT INTO Pagamentos (TicketId, Valor, TipoPagamento, DataHora) 
                                   VALUES (@TicketId, @Valor, @Tipo, @DataHora);
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TicketId", pagamento.TicketId);
                        cmd.Parameters.AddWithValue("@Valor", pagamento.Valor);
                        cmd.Parameters.AddWithValue("@Tipo", (int)pagamento.TipoPagamento);
                        cmd.Parameters.AddWithValue("@DataHora", pagamento.DataHora);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        pagamento.Id = id;
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao registrar pagamento: {ex.Message}");
            }
        }

        public Ticket BuscarTicketAbertoPorPlaca(string placa)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"SELECT t.Id, t.VeiculoId, t.Vaga, t.HoraEntrada, t.HoraSaida, 
                                          t.ValorTotal, t.Status
                                   FROM Tickets t
                                   INNER JOIN Veiculos v ON t.VeiculoId = v.Id
                                   WHERE v.Placa = @Placa AND t.Status = 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Placa", placa.ToUpper().Trim());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CriarTicketDoReader(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar ticket: {ex.Message}");
            }
        }

        public List<Ticket> ListarTicketsAbertos()
        {
            List<Ticket> tickets = new List<Ticket>();

            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = @"SELECT t.Id, t.VeiculoId, t.Vaga, t.HoraEntrada, t.HoraSaida, 
                                          t.ValorTotal, t.Status
                                   FROM Tickets t
                                   WHERE t.Status = 0
                                   ORDER BY t.HoraEntrada";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tickets.Add(CriarTicketDoReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar tickets: {ex.Message}");
            }

            return tickets;
        }

        public bool TemTicketAberto(int veiculoId)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = "SELECT COUNT(*) FROM Tickets WHERE VeiculoId = @Id AND Status = 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", veiculoId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar ticket: {ex.Message}");
            }
        }

        public bool VagaOcupada(string vaga)
        {
            try
            {
                using (SqlConnection conn = Conexao.Abrir())
                {
                    string sql = "SELECT COUNT(*) FROM Tickets WHERE Vaga = @Vaga AND Status = 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Vaga", vaga.ToUpper().Trim());
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar vaga: {ex.Message}");
            }
        }

        private Ticket CriarTicketDoReader(SqlDataReader reader)
        {
            Ticket ticket = new Ticket();
            ticket.Id = reader.GetInt32(0);
            ticket.VeiculoId = reader.GetInt32(1);
            ticket.Vaga = reader.GetString(2);
            ticket.HoraEntrada = reader.GetDateTime(3);
            ticket.HoraSaida = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
            ticket.ValorTotal = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5);
            ticket.Status = (StatusTicket)reader.GetInt32(6);

            ticket.Veiculo = _veiculoRepo.BuscarPorId(ticket.VeiculoId);

            return ticket;
        }
    }
}