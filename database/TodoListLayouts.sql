USE ToDO

GO

Create TABLE [TodoListLayouts](
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    ListId UNIQUEIDENTIFIER,
    Layout VARCHAR(max) NOT NULL,
    FOREIGN KEY (ListId)
    REFERENCES TodoLists (ID) ON DELETE CASCADE
)

GO