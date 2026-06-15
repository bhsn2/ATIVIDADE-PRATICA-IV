using System;
using Estacionamento.Enums;

namespace Estacionamento.Models
{
    public abstract class Veiculo
    {
        public int Id { get; set; }

        private string _placa;
        public string Placa
        {
            get => _placa;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Placa não pode ser vazia.");
                _placa = value.ToUpper().Trim();
            }
        }

        private string _modelo;
        public string Modelo
        {
            get => _modelo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Modelo não pode ser vazio.");
                _modelo = value.Trim();
            }
        }

        private string _cor;
        public string Cor
        {
            get => _cor;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Cor não pode ser vazia.");
                _cor = value.Trim();
            }
        }

        public abstract TipoVeiculo Tipo { get; }
        public abstract decimal ValorHora { get; }
        public abstract decimal TaxaAdicional { get; }

        public abstract decimal CalcularValor(int horas);

        protected Veiculo(string placa, string modelo, string cor)
        {
            Placa = placa;
            Modelo = modelo;
            Cor = cor;
        }

        protected Veiculo() { }

        public override string ToString()
        {
            return $"[{Tipo}] {Placa} - {Modelo} ({Cor})";
        }
    }
}