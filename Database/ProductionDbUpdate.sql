-- DELETE TABLES

-- Downgrades.sql
DROP TABLE Downgrades
GO

-- PaymentMethods.sql
DROP TABLE PaymentMethods
GO

-- AccountsPlans.sql
DROP TABLE AccountsPlans
GO

-- AccountsLists.sql
DROP TABLE AccountsLists
GO

-- AccountsDiscounts.sql
DROP TABLE AccountsDiscounts
GO

-- Discounts.sql
DROP TABLE Discounts
GO

-- SubItemLayouts
DROP TABLE SubItemLayouts
GO

-- SubItems
DROP TABLE SubItems
GO

-- TodoListLayouts.sql
DROP TABLE TodoListLayouts
GO

-- TodoListItems
DROP TABLE TodoListItems
GO

-- TodoLists.sql
DROP TABLE TodoLists
GO

-- Account.sql
DROP TABLE Accounts
GO

-- Plans.sql
DROP TABLE Plans
GO

-- CREATE TABLES

-- Plans.sql
CREATE TABLE [Plans]
(
    ID INT PRIMARY KEY NOT NULL,
    [Name] VARCHAR(7) NOT NULL,
    [MaxContributors] INT NOT NULL,
    [MaxLists] INT NOT NULL,
    [CanAddDueDates] BIT NOT NULL,
    [CanNotifyViaEmail] BIT NOT NULL,
)
GO

-- PlanData.sql
INSERT INTO Plans
    ([ID], [Name], [MaxContributors], [MaxLists], [CanNotifyViaEmail], [CanAddDueDates])
VALUES
    (1, 'Free', 0, 5, 0, 0)
GO

INSERT INTO Plans
    ([ID], [Name], [MaxContributors], [MaxLists], [CanNotifyViaEmail], [CanAddDueDates])
VALUES
    (2, 'Basic', 5, 10, 1, 1)
GO

INSERT INTO Plans
    ([ID], [Name], [MaxContributors], [MaxLists], [CanNotifyViaEmail], [CanAddDueDates])
VALUES
    (3, 'Premium', -1, -1, 1, 1)
GO

-- Account.sql
CREATE TABLE [Accounts]
(

    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [FullName] NVARCHAR(50),
    [PictureUrl] NVARCHAR(255),
    [Email] NVARCHAR(50) NOT NULL,
    [CustomerID] VARCHAR(9) NOT NULL,
    [PaymentMethodID] VARCHAR(50),
    [SubscriptionID] VARCHAR(50),
    [PaymentMethodDeletedPlan] VARCHAR(50),
    [EmailDueDate] BIT,
    [EmailListCompleted] BIT,
    [EmailItemCompleted] BIT,
    [EmailInvitation] BIT
)
GO

-- TodoLists.sql
CREATE TABLE [TodoLists]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [ListTitle] VARCHAR(50) NOT NULL,
    [Completed] BIT NOT NULL DEFAULT(0),
    [Contributors] VARCHAR(max) NOT NULL
)
GO

-- TodoListItems.sql
CREATE TABLE [TodoListItems]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [Notes] NVARCHAR(200),
    [Completed] BIT NOT NULL DEFAULT(0),
    [Name] NVARCHAR(50),
    [DueDate] DATETIME,
    [ListID] UNIQUEIDENTIFIER,
    [HasSubItems] BIT NOT NULL DEFAULT(0),
    [Important] BIT,
    FOREIGN KEY (ListID) REFERENCES TodoLists (ID) ON DELETE CASCADE
)
GO

-- TodoListLayouts.sql
CREATE TABLE [TodoListLayouts]
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    ListId UNIQUEIDENTIFIER,
    Layout VARCHAR(max) NOT NULL,
    FOREIGN KEY (ListId)
    REFERENCES TodoLists (ID) ON DELETE CASCADE
)
GO

-- SubItems.sql
CREATE TABLE [SubItems]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [ListItemID] UNIQUEIDENTIFIER,
    [Name] VARCHAR(50),
    [Completed] BIT NOT NULL DEFAULT(0),
    FOREIGN KEY (ListItemID) REFERENCES TodoListItems (ID) ON DELETE CASCADE
)
GO

-- SubItemLayouts.sql
CREATE TABLE [SubItemLayouts]
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    ItemId UNIQUEIDENTIFIER,
    Layout VARCHAR(max) NOT NULL,
    FOREIGN KEY (ItemId) REFERENCES TodoListItems (ID) ON DELETE CASCADE,
    UNIQUE (ItemId)
)
GO

-- Discounts.sql
CREATE TABLE [Discounts]
(
    [ID] INT PRIMARY KEY NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    [Percentage] INT NOT NULL,
    [BillingCycles] TINYINT NOT NULL
)
GO

-- DiscountsData.sql
INSERT INTO [Discounts]
    ([ID], [Name], [Percentage], [BillingCycles])
VALUES
    (1, 'FreeMonth', 100, 1)
GO

INSERT INTO Discounts
    ([ID], [Name], [Percentage], [BillingCycles])
VALUES
    (2, '20PercentOff', 20, 1)
GO

-- AccountsDiscounts.sql
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

-- AccountsLists.sql
CREATE TABLE [AccountsLists]
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    AccountID UNIQUEIDENTIFIER,
    ListID UNIQUEIDENTIFIER,
    Role TINYINT NOT NULL,
    FOREIGN KEY (AccountID) REFERENCES Accounts (ID),
)
GO

-- AccountsPlans.sql
CREATE TABLE [AccountsPlans]
(
    ID UNIQUEIDENTIFIER NOT NULL,
    AccountID UNIQUEIDENTIFIER NOT NULL,
    PlanID INT NOT NULL,
    ListCount INT NOT NULL,
    FOREIGN KEY (AccountID) REFERENCES Accounts (ID),
    FOREIGN KEY (PlanID) REFERENCES Plans (ID)
)
GO

-- PaymentMethods.sql
CREATE TABLE [PaymentMethods]
(
    TokenID VARCHAR(50) PRIMARY KEY,
    AccountID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (AccountID) REFERENCES Accounts (ID)
)
GO

-- Downgrades.sql
CREATE TABLE [Downgrades]
(
    AccountID UNIQUEIDENTIFIER UNIQUE NOT NULL,
    BillingCycleEnd DATETIME NOT NULL,
    PlanID INT NOT NULL,
)
GO