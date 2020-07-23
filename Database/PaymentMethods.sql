USE [ToDo]

GO

Create Table [PaymentMethods](
    TokenID VARCHAR(50) PRIMARY KEY,
    AccountID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (AccountID) REFERENCES Accounts (ID)
)

Go