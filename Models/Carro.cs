using System;
using Estacionamento.Enums;

namespace Estacionamento.Models
{
    public class Carro : Veiculo
    {
        public override TipoVeiculo Tipo => TipoVeiculo.Carro;
        public override decimal ValorHora => 10.00m;
        public override decimal TaxaAdicional => 0.00m;

        public Carro(string placa, string modelo, string cor)
            : base(placa, modelo, cor) { }

        public Carro() : base() { }

        public override decimal CalcularValor(int horas)
        {
            return horas * ValorHora;
        }
    }
}