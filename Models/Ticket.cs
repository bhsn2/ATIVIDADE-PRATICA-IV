using System;

namespace Estacionamento.Models
{
    public enum StatusTicket
    {
        Aberto = 0,
        Fechado = 1
    }
    public class Ticket
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public Veiculo Veiculo { get; set; }
        public string Vaga { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSaida { get; set; }
        public decimal? ValorTotal { get; set; }
        public StatusTicket Status { get; set; }

        public Ticket() { }

        public Ticket(Veiculo veiculo, string vaga)
        {
            if (veiculo == null)
                throw new ArgumentException("Veículo não pode ser nulo.");
            if (string.IsNullOrWhiteSpace(vaga))
                throw new ArgumentException("Vaga não pode ser vazia.");

            VeiculoId = veiculo.Id;
            Veiculo = veiculo;
            Vaga = vaga.ToUpper().Trim();
            HoraEntrada = DateTime.Now;
            Status = StatusTicket.Aberto;
        }

        public decimal CalcularValorSaida()
        {
            HoraSaida = DateTime.Now;
            TimeSpan permanencia = HoraSaida.Value - HoraEntrada;
            int minutosTotal = (int)permanencia.TotalMinutes;

            if (minutosTotal <= 15)
            {
                ValorTotal = 0;
                return 0;
            }

            int horas = minutosTotal / 60;
            int minutosRestantes = minutosTotal % 60;

            if (minutosRestantes > 30)
                horas++;
            else if (minutosRestantes > 0 && horas == 0)
                horas = 1;

            if (horas == 0)
                horas = 1;

            ValorTotal = Veiculo.CalcularValor(horas);
            return ValorTotal.Value;
        }

        public string TempoFormatado()
        {
            DateTime fim = HoraSaida ?? DateTime.Now;
            TimeSpan tempo = fim - HoraEntrada;
            return $"{(int)tempo.TotalHours}h {tempo.Minutes}min";
        }

        public override string ToString()
        {
            return $"Ticket #{Id} | Vaga: {Vaga} | Veículo: {Veiculo?.Placa ?? "N/A"} | " +
                   $"Entrada: {HoraEntrada:dd/MM/yyyy HH:mm} | Status: {Status}";
        }
    }
}