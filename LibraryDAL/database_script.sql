CREATE DATABASE LIBDATABASE ON PRIMARY (NAME = 'LibDB', FILENAME = 'LibDB.mdf')

USE LIBDATABASE

CREATE TABLE [dbo].[Books] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [BookName]         NVARCHAR (150) NOT NULL,
    [Authors]          NVARCHAR (150) NOT NULL,
    [YearOfPublishing] INT            NOT NULL,
    [PreviewImg] VARBINARY (MAX) NULL, 
    [AttachedFile] VARBINARY(MAX) NULL, 
    CONSTRAINT [PK__Books__3214EC2708455360] PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE TABLE [dbo].[Users] (
    [ID]             INT             IDENTITY (1, 1) NOT NULL,
    [Username]       VARCHAR (24)    NOT NULL,
    [PassHash]       VARCHAR (32)    NOT NULL,
    [FirstName]      NVARCHAR (50)   NOT NULL,
    [LastName]       NVARCHAR (50)   NOT NULL,
    [DateOfBirth]    DATE            NOT NULL,
    [ProfilePicture] VARBINARY (MAX) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_Username] UNIQUE NONCLUSTERED ([Username] ASC)
);

CREATE APPLICATION ROLE lib_app WITH PASSWORD = 'DFa[7wzaVA'

DENY BACKUP DATABASE, BACKUP LOG, CREATE PROCEDURE, CREATE TABLE TO lib_app CASCADE
GRANT Insert, Select, Update, Delete ON Books TO lib_app
GRANT Insert, Select, Update, Delete ON Users TO lib_app