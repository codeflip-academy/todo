use [ToDo]

GO

Create Table[Downgrades]
(
    AccountID UNIQUEIDENTIFIER UNIQUE NOT NULL,
    BillingCycleEnd DATETIME NOT NULL,
    PlanID INT NOT NULL,
)