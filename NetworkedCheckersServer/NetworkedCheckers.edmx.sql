
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/06/2014 10:16:38
-- Generated from EDMX file: C:\Users\Aloysius\Documents\Visual Studio 2012\Projects\V3 NetworkedCheckers\NetworkedCheckersServer\NetworkedCheckers.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [C:\Users\Aloysius\Documents\Visual Studio 2012\Projects\V3 NetworkedCheckers\NetworkedCheckersDB.mdf];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserHighscore]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Highscores] DROP CONSTRAINT [FK_UserHighscore];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Highscores]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Highscores];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Highscores'
CREATE TABLE [dbo].[Highscores] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] nvarchar(max)  NOT NULL,
    [MoveCount] smallint  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [LoginStatus] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Highscores'
ALTER TABLE [dbo].[Highscores]
ADD CONSTRAINT [PK_Highscores]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserId] in table 'Highscores'
ALTER TABLE [dbo].[Highscores]
ADD CONSTRAINT [FK_UserHighscore]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserHighscore'
CREATE INDEX [IX_FK_UserHighscore]
ON [dbo].[Highscores]
    ([UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------