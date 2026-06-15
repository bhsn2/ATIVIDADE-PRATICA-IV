//Bruno Henrique | Maverson Souza – 6º Período
using System;
using System.Collections.Generic;
using Estacionamento.Data;
using Estacionamento.Enums;
using Estacionamento.Models;
using Estacionamento.Repositories;

namespace Estacionamento
{
    class Program
    {
        static VeiculoRepository veiculoRepo = new VeiculoRepository();
        static TicketRepository ticketRepo = new TicketRepository();

        static void Main(string[] args)
        {
            int opcao = -1;

            do
            {
                ExibirMenu();
                string entrada = Console.ReadLine();

                if (!int.TryParse(entrada, out opcao) || opcao < 0 || opcao > 6)
                {
                    Console.WriteLine("\nOpção inválida! Tente novamente.");
                    continue;
                }

                try
                {
                    switch (opcao)
                    {
                        case 1: CadastrarVeiculo(); break;
                        case 2: RegistrarEntrada(); break;
                        case 3: RegistrarSaida(); break;
                        case 4: ListarVeiculosEstacionados(); break;
                        case 5: ConsultarTicket(); break;
                        case 6: ListarVeiculos(); break;
                        case 0: Console.WriteLine("\nSistema encerrado."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                }

            } while (opcao != 0);
        }

        static void ExibirMenu()
        {
            Console.WriteLine("\nSISTEMA DE ESTACIONAMENTO:\n");
            Console.WriteLine("  1 - Cadastrar veículo");
            Console.WriteLine("  2 - Registrar entrada");
            Console.WriteLine("  3 - Registrar saída + pagamento");
            Console.WriteLine("  4 - Listar veículos estacionados");
            Console.WriteLine("  5 - Consultar ticket por placa");
            Console.WriteLine("  6 - Listar veículos cadastrados");
            Console.WriteLine("  0 - Sair");
            Console.Write("\nEscolha uma opção: ");
        }

        //OPÇÃO 1
        static void CadastrarVeiculo()
        {
            Console.WriteLine("\n--- CADASTRAR VEÍCULO ---");

            string placa = LerTexto("Placa: ");

            Veiculo existente = veiculoRepo.BuscarPorPlaca(placa);
            if (existente != null)
            {
                Console.WriteLine($"Veículo já cadastrado: {existente}");
                return;
            }

            string modelo = LerTexto("Modelo: ");
            string cor = LerTexto("Cor: ");

            Console.WriteLine("Tipo do veículo:");
            Console.WriteLine("  1 - Moto  (R$ 5,00/h)");
            Console.WriteLine("  2 - Carro (R$ 10,00/h)");
            Console.WriteLine("  3 - Caminhão (R$ 18,00/h + taxa)");
            int tipo = LerInt("Escolha: ", 1, 3);

            Veiculo veiculo;
            switch (tipo)
            {
                case 1: veiculo = new Moto(placa, modelo, cor); break;
                case 2: veiculo = new Carro(placa, modelo, cor); break;
                case 3: veiculo = new Caminhao(placa, modelo, cor); break;
                default: throw new Exception("Tipo inválido.");
            }

            int id = veiculoRepo.Inserir(veiculo);
            Console.WriteLine($"\nVeículo cadastrado com sucesso! ID: {id}");
            Console.WriteLine($"  {veiculo}");
        }

        //OPÇÃO 2
        static void RegistrarEntrada()
        {
            Console.WriteLine("\n--- REGISTRAR ENTRADA ---");

            string placa = LerTexto("Placa do veículo: ");

            Veiculo veiculo = veiculoRepo.BuscarPorPlaca(placa);
            if (veiculo == null)
            {
                Console.WriteLine("Veículo não cadastrado. Deseja cadastrar agora? (S/N)");
                string resp = Console.ReadLine();
                if (resp == "S")
                {
                    CadastrarVeiculo();
                    veiculo = veiculoRepo.BuscarPorPlaca(placa);
                    if (veiculo == null) return;
                }
                else return;
            }

            if (ticketRepo.TemTicketAberto(veiculo.Id))
            {
                Console.WriteLine("Este veículo já está no estacionamento.");
                return;
            }

            string vaga = LerTexto("Número da vaga: ");

            if (ticketRepo.VagaOcupada(vaga))
            {
                Console.WriteLine($"A vaga '{vaga}' já está ocupada.");
                return;
            }

            Ticket ticket = new Ticket(veiculo, vaga);
            int ticketId = ticketRepo.RegistrarEntrada(ticket);

            Console.WriteLine($"\n ENTRADA REGISTRADA!");
            Console.WriteLine($"  Ticket: #{ticketId}");
            Console.WriteLine($"  Veículo: {veiculo}");
            Console.WriteLine($"  Vaga: {vaga}");
            Console.WriteLine($"  Entrada: {ticket.HoraEntrada:dd/MM/yyyy HH:mm:ss}");
        }

        //OPÇÃO 3
        static void RegistrarSaida()
        {
            Console.WriteLine("\n--- REGISTRAR SAÍDA + PAGAMENTO ---");

            string placa = LerTexto("Placa do veículo: ");

            Ticket ticket = ticketRepo.BuscarTicketAbertoPorPlaca(placa);
            if (ticket == null)
            {
                Console.WriteLine("Nenhum ticket aberto encontrado para esta placa.");
                return;
            }

            decimal valor = ticket.CalcularValorSaida();

            Console.WriteLine($"\n--- RESUMO DA SAÍDA ---");
            Console.WriteLine($"  Ticket: #{ticket.Id}");
            Console.WriteLine($"  Veículo: {ticket.Veiculo}");
            Console.WriteLine($"  Vaga: {ticket.Vaga}");
            Console.WriteLine($"  Entrada: {ticket.HoraEntrada:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"  Saída: {ticket.HoraSaida:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"  Permanência: {ticket.TempoFormatado()}");

            if (valor == 0)
            {
                Console.WriteLine($" TOLERÂNCIA - Sem cobranças.");
                ticketRepo.RegistrarSaida(ticket.Id, 0);
                Console.WriteLine("\n Saída registrada com sucesso.");
                return;
            }

            if (ticket.Veiculo.TaxaAdicional > 0)
                Console.WriteLine($"  Taxa adicional ({ticket.Veiculo.Tipo}): R$ {ticket.Veiculo.TaxaAdicional:F2}");

            Console.WriteLine($" VALOR TOTAL: R$ {valor:F2}");

            // Pagamento
            Console.WriteLine("\nForma de pagamento:");
            Console.WriteLine("  1 - Dinheiro");
            Console.WriteLine("  2 - Cartão");
            int formaPgto = LerInt("Escolha: ", 1, 2);

            TipoPagamento tipoPgto = formaPgto == 1 ? TipoPagamento.Dinheiro : TipoPagamento.Cartao;

            if (tipoPgto == TipoPagamento.Dinheiro)
            {
                decimal valorPago = LerDecimal($"Valor pago (mínimo R$ {valor:F2}): ", valor);
                decimal troco = valorPago - valor;

                if (troco > 0)
                    Console.WriteLine($"  Troco: R$ {troco:F2}");
            }

            Pagamento pagamento = new Pagamento(ticket.Id, valor, tipoPgto);
            ticketRepo.RegistrarPagamento(pagamento);
            ticketRepo.RegistrarSaida(ticket.Id, valor);

            Console.WriteLine($"\n✅ Pagamento registrado! Ticket #{ticket.Id} fechado.");
        }

        //OPÇÃO 4
        static void ListarVeiculosEstacionados()
        {
            Console.WriteLine("\n--- VEÍCULOS ESTACIONADOS ---");

            List<Ticket> tickets = ticketRepo.ListarTicketsAbertos();

            if (tickets.Count == 0)
            {
                Console.WriteLine("Nenhum veículo estacionado no momento.");
                return;
            }

            Console.WriteLine($"Total: {tickets.Count} veículo(s)\n");

            foreach (Ticket t in tickets)
            {
                Console.WriteLine($"  Ticket #{t.Id} | Vaga: {t.Vaga} | {t.Veiculo} | " +
                                  $"Entrada: {t.HoraEntrada:HH:mm} | Tempo: {t.TempoFormatado()}");
            }
        }

        //OPÇÃO 5
        static void ConsultarTicket()
        {
            Console.WriteLine("\n--- CONSULTAR TICKET ---");

            string placa = LerTexto("Placa do veículo: ");

            Ticket ticket = ticketRepo.BuscarTicketAbertoPorPlaca(placa);
            if (ticket == null)
            {
                Console.WriteLine("Nenhum ticket aberto para esta placa.");
                return;
            }

            Console.WriteLine($"\n  Ticket: #{ticket.Id}");
            Console.WriteLine($"  Veículo: {ticket.Veiculo}");
            Console.WriteLine($"  Vaga: {ticket.Vaga}");
            Console.WriteLine($"  Entrada: {ticket.HoraEntrada:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"  Tempo atual: {ticket.TempoFormatado()}");
            Console.WriteLine($"  Valor/hora: R$ {ticket.Veiculo.ValorHora:F2}");

            if (ticket.Veiculo.TaxaAdicional > 0)
                Console.WriteLine($"  Taxa adicional: R$ {ticket.Veiculo.TaxaAdicional:F2}");
        }

        //OPÇÃO 6
        static void ListarVeiculos()
        {
            Console.WriteLine("\n--- VEÍCULOS CADASTRADOS ---");

            List<Veiculo> veiculos = veiculoRepo.ListarTodos();

            if (veiculos.Count == 0)
            {
                Console.WriteLine("Nenhum veículo cadastrado.");
                return;
            }

            foreach (Veiculo v in veiculos)
            {
                Console.WriteLine($"  ID: {v.Id} | {v}");
            }
        }

        //métodos
        static string LerTexto(string mensagem)
        {
            string valor;
            do
            {
                Console.Write(mensagem);
                valor = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(valor))
                    Console.WriteLine("Erro: Campo obrigatório.");
            } while (string.IsNullOrWhiteSpace(valor));
            return valor.Trim();
        }

        static int LerInt(string mensagem, int min, int max)
        {
            int valor;
            bool valido;
            do
            {
                Console.Write(mensagem);
                valido = int.TryParse(Console.ReadLine(), out valor);
                if (!valido || valor < min || valor > max)
                {
                    Console.WriteLine($"Erro: Digite um número entre {min} e {max}.");
                    valido = false;
                }
            } while (!valido);
            return valor;
        }

        static decimal LerDecimal(string mensagem, decimal minimo)
        {
            decimal valor;
            bool valido;
            do
            {
                Console.Write(mensagem);
                valido = decimal.TryParse(Console.ReadLine(), out valor);
                if (!valido || valor < minimo)
                {
                    Console.WriteLine($"Erro: Valor mínimo é R$ {minimo:F2}.");
                    valido = false;
                }
            } while (!valido);
            return valor;
        }
    }
}