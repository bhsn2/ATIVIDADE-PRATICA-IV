using System;
using Estacionamento.Enums;

namespace Estacionamento.Models
{
    public class Moto : Veiculo
    {
        public override TipoVeiculo Tipo => TipoVeiculo.Moto;
        public override decimal ValorHora => 5.00m;
        public override decimal TaxaAdicional => 0.00m;

        public Moto(string placa, string modelo, string cor)
            : base(placa, modelo, cor) { }

        public Moto() : base() { }

        public override decimal CalcularValor(int horas)
        {
            return horas * ValorHora;
        }
    }
}