using System;
using Estacionamento.Enums;

namespace Estacionamento.Models
{
    public class Caminhao : Veiculo
    {
        public override TipoVeiculo Tipo => TipoVeiculo.Caminhao;
        public override decimal ValorHora => 18.00m;
        public override decimal TaxaAdicional => 5.00m;

        public Caminhao(string placa, string modelo, string cor)
            : base(placa, modelo, cor) { }

        public Caminhao() : base() { }

        public override decimal CalcularValor(int horas)
        {
            return (horas * ValorHora) + TaxaAdicional;
        }
    }
}