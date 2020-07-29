USE [Todo]

GO

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

INSERT INTO Plans
    ([ID], [Name], [MaxContributors], [MaxLists], [CanNotifyViaEmail], [CanAddDueDates])
VALUES
    (1, 'Free', 0, 100, 0, 0)

GO

INSERT INTO Plans
    ([ID], [Name], [MaxContributors], [MaxLists], [CanNotifyViaEmail], [CanAddDueDates])
VALUES
    (2, 'Basic', 5, 500, 1, 1)

GO

INSERT INTO Plans
    ([ID], [Name], [MaxContributors], [MaxLists], [CanNotifyViaEmail], [CanAddDueDates])
VALUES
    (3, 'Premium', -1, -1, 1, 1)

GO

CREATE TABLE [Accounts]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [FullName] VARCHAR(50),
    [PictureUrl] VARCHAR(255),
    [Email] VARCHAR(50) NOT NULL,
    [PlanID] INT NOT NULL,
    [PaymentID] VARCHAR(9) NOT NULL,
    FOREIGN KEY ([PlanID]) REFERENCES Plans([ID])
)

GO

GO

CREATE TABLE [PaymentMethods](
    TokenID VARCHAR(50) PRIMARY KEY,
    AccountID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (AccountID) REFERENCES Accounts (ID)
)

GO

CREATE TABLE [TodoLists]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [ListTitle] VARCHAR(50) NOT NULL,
    [Completed] BIT NOT NULL DEFAULT(0),
    [Contributors] VARCHAR(max) NOT NULL
)

GO

CREATE TABLE [TodoListItems]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [Notes] VARCHAR(200),
    [Completed] BIT NOT NULL DEFAULT(0),
    [Name] VARCHAR(50),
    [DueDate] DATETIME,
    [ListID] UNIQUEIDENTIFIER,

    FOREIGN KEY (ListID) REFERENCES TodoLists (ID) ON DELETE CASCADE
)

GO

Create TABLE [TodoListLayouts]
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    ListId UNIQUEIDENTIFIER,
    Layout VARCHAR(max) NOT NULL,
    FOREIGN KEY (ListId)
    REFERENCES TodoLists (ID) ON DELETE CASCADE
)

GO

CREATE TABLE [SubItems]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [ListItemID] UNIQUEIDENTIFIER,
    [Name] VARCHAR(50),
    [Completed] BIT NOT NULL DEFAULT(0)

        FOREIGN KEY (ListItemID) REFERENCES TodoListItems (ID) ON DELETE CASCADE
)

GO

CREATE TABLE [SubItemLayouts]
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    ItemId UNIQUEIDENTIFIER,
    Layout VARCHAR(max) NOT NULL,
    FOREIGN KEY (ItemId) REFERENCES TodoListItems (ID) ON DELETE CASCADE,
    UNIQUE (ItemId)
)

GO

CREATE TABLE [AccountsLists]
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    AccountID UNIQUEIDENTIFIER,
    ListID UNIQUEIDENTIFIER,
    Role TINYINT NOT NULL,
    FOREIGN KEY (AccountID) REFERENCES Accounts (ID),
)

GO

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
