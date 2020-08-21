USE Todo

GO

-- Free month
INSERT INTO Discounts
    ([ID], [Name], [Percentage], [BillingCycles])
VALUES
    (1, 'FreeMonth', 100, 1)

-- 20% Off
INSERT INTO Discounts
    ([ID], [Name], [Percentage], [BillingCycles])
VALUES
    (2, '20PercentOff', 20, 1)

GO