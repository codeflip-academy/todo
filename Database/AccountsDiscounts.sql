USE [Todo]

GO

CREATE TABLE [AccountsDiscounts]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    [AccountID] UNIQUEIDENTIFIER NOT NULL,
    [DiscountID] INT NOT NULL,
    [AppliedToSubscription] BIT NOT NULL DEFAULT 0,
    FOREIGN KEY ([AccountID]) REFERENCES Accounts(ID),
    FOREIGN KEY ([DiscountID]) REFERENCES Discounts(ID)
)

GO