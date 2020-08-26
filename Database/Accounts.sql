USE [ToDo]

GO

CREATE TABLE [Accounts]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [FullName] VARCHAR(50),
    [PictureUrl] VARCHAR(255),
    [Email] VARCHAR(50) NOT NULL,
    [CustomerID] VARCHAR(9) NOT NULL,
    [PaymentMethodID] VARCHAR(50),
    [SubscriptionID] VARCHAR(50),
)

GO