USE [Todo]

GO

CREATE TABLE [Downgrades]
(
    AccountID UNIQUEIDENTIFIER UNIQUE NOT NULL,
    BillingCycleEnd DATETIME NOT NULL,
    PlanID INT NOT NULL,
)

GO