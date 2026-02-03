CREATE TABLE [dbo].[Addresses]
(
    [Id] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Addresses PRIMARY KEY,

    [PostCode] NVARCHAR(20) NOT NULL,
    [City] NVARCHAR(100) NOT NULL,
    [Street] NVARCHAR(150) NOT NULL,
    [StreetNumber] NVARCHAR(20) NOT NULL,
    [ApartmentNumber] NVARCHAR(20) NULL,

    CONSTRAINT FK_Addresses_Customers
        FOREIGN KEY ([Id])
        REFERENCES [dbo].[Customers] ([Id])
        ON DELETE CASCADE
);
