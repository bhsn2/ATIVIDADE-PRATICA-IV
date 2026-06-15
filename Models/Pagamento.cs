using System;
using Estacionamento.Enums;

namespace Estacionamento.Models
{
    public class Pagamento
    {
        public int Id { get; set; }
        public int TicketId { get; set; }

        private decimal _valor;
        public decimal Valor
        {
            get => _valor;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Valor do pagamento não pode ser negativo.");
                _valor = value;
            }
        }

        public TipoPagamento TipoPagamento { get; set; }
        public DateTime DataHora { get; set; }

        public Pagamento() { }

        public Pagamento(int ticketId, decimal valor, TipoPagamento tipo)
        {
            if (valor < 0)
                throw new ArgumentException("Valor do pagamento não pode ser negativo.");

            TicketId = ticketId;
            Valor = valor;
            TipoPagamento = tipo;
            DataHora = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Pagamento #{Id} | Ticket: {TicketId} | R$ {Valor:F2} | " +
                   $"Tipo: {TipoPagamento} | Data: {DataHora:dd/MM/yyyy HH:mm}";
        }
    }
}