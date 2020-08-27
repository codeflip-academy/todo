USE [Todo]

GO

CREATE TABLE [Discounts]
(
    [ID] INT PRIMARY KEY NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    [Percentage] INT NOT NULL,
    [BillingCycles] TINYINT NOT NULL
)

GO