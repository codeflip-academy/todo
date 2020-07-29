USE [ToDo]

GO

Create Table [Accounts]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [FullName] VARCHAR(50),
    [PictureUrl] VARCHAR(255),
    [Email] VARCHAR(50) NOT NULL,
    [PlanID] INT NOT NULL,
    [PaymentID] VARCHAR(9) NOT NULL,
    [SubscriptionID] VARCHAR(50),
    FOREIGN KEY ([PlanID]) REFERENCES Plans([ID])
)

GO