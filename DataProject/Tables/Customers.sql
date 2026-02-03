CREATE TABLE [dbo].[Customers]
(
    [Id] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Customers PRIMARY KEY,

    [Name] NVARCHAR(200) NULL,
    [TaxId] NVARCHAR(50) NULL,
    [PhoneNumber] NVARCHAR(50) NULL,
    [EmailAddress] NVARCHAR(320) NULL
);