CREATE DATABASE Estacionamento;
GO

USE Estacionamento;
GO

CREATE TABLE Veiculos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Placa VARCHAR(10) NOT NULL UNIQUE,
    Modelo VARCHAR(50) NOT NULL,
    Cor VARCHAR(30) NOT NULL,
    TipoVeiculo INT NOT NULL
);

CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VeiculoId INT NOT NULL,
    Vaga VARCHAR(10) NOT NULL,
    HoraEntrada DATETIME NOT NULL,
    HoraSaida DATETIME NULL,
    ValorTotal DECIMAL(10,2) NULL,
    Status INT NOT NULL DEFAULT 0,
    FOREIGN KEY (VeiculoId) REFERENCES Veiculos(Id)
);

CREATE TABLE Pagamentos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT NOT NULL,
    Valor DECIMAL(10,2) NOT NULL,
    TipoPagamento INT NOT NULL,
    DataHora DATETIME NOT NULL,
    FOREIGN KEY (TicketId) REFERENCES Tickets(Id)
);

CREATE INDEX IX_Veiculos_Placa ON Veiculos(Placa);
CREATE INDEX IX_Tickets_Status ON Tickets(Status);
GO




CREATE VIEW vw_TicketsAbertos AS
SELECT
    t.Id AS TicketId,
    v.Placa,
    v.Modelo,
    v.TipoVeiculo,
    t.Vaga,
    t.HoraEntrada,
    DATEDIFF(MINUTE, t.HoraEntrada, GETDATE()) AS MinutosEstacionado
FROM Tickets t
INNER JOIN Veiculos v ON t.VeiculoId = v.Id
WHERE t.Status = 0;
GO




CREATE PROCEDURE sp_FaturamentoDiario
    @Data DATE
AS
BEGIN
    SELECT
        COUNT(*) AS TotalTickets,
        SUM(p.Valor) AS FaturamentoTotal,
        SUM(CASE WHEN p.TipoPagamento = 0 THEN p.Valor ELSE 0 END) AS TotalDinheiro,
        SUM(CASE WHEN p.TipoPagamento = 1 THEN p.Valor ELSE 0 END) AS TotalCartao
    FROM Pagamentos p
    WHERE CAST(p.DataHora AS DATE) = @Data;
END;
GO