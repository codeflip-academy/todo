USE [ToDo]

GO

CREATE TABLE [Accounts]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [FullName] NVARCHAR(50),
    [PictureUrl] NVARCHAR(255),
    [Email] NVARCHAR(50) NOT NULL,
    [CustomerID] VARCHAR(9) NOT NULL,
    [PaymentMethodID] VARCHAR(50),
    [SubscriptionID] VARCHAR(50),
)

GO