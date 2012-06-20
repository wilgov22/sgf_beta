USE [master]
GO

-- Ou colocar raiseerror caso não exista a BD ou criar
IF NOT EXISTS (select name from dbo.sysdatabases where name = N'SGF')
BEGIN
--	raiserror('Não foi detectada a Base de Dados. Verifique por favor.', 20, -1) with log
	CREATE DATABASE [SGF] ON  PRIMARY 
	( NAME = N'SGF', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.2\MSSQL\Data\SGF.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
	 LOG ON 
	( NAME = N'SGF_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.2\MSSQL\Data\SGF_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
	 COLLATE Latin1_General_CI_AS
END
GO

--criação schema SGFADM
USE [SGF]
GO
IF EXISTS (select name from sys.schemas WHERE NAME='SGFADM')
BEGIN
	DROP SCHEMA [SGFADM]
END

-- Garantir a existencia do LOGIN de acesso à BD utilizado na WebAPP
IF  EXISTS (SELECT * FROM sys.server_principals WHERE name = N'SGF_App')
	DROP LOGIN [SGF_App]
GO

IF  EXISTS (SELECT * FROM sys.server_principals WHERE name = N'SGF_Adm')
	DROP LOGIN [SGF_Adm]
GO

--Criação de logins
CREATE LOGIN [SGF_App] WITH PASSWORD=N'SGF_App123', DEFAULT_DATABASE=[SGF], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

CREATE LOGIN [SGF_Adm] WITH PASSWORD=N'SGF_Adm123', DEFAULT_DATABASE=[SGF], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

USE [SGF]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'SGF_App')
DROP USER [SGF_App]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'SGF_Adm')
DROP USER [SGF_Adm]
GO

--Criação USER
CREATE USER [SGF_App] FOR LOGIN [SGF_App] WITH DEFAULT_SCHEMA=[SGFADM]
GO

CREATE USER [SGF_Adm] FOR LOGIN [SGF_Adm] WITH DEFAULT_SCHEMA=[SGFADM]
GO



CREATE SCHEMA [SGFADM] AUTHORIZATION [SGF_Adm]
GO



--GRANTS de SIUD para SGF_APP (user aplicacional)

GRANT SELECT ON SCHEMA::[SGFADM] TO [SGF_App]
GO
GRANT DELETE ON SCHEMA::[SGFADM] TO [SGF_App]
GO
GRANT INSERT ON SCHEMA::[SGFADM] TO [SGF_App]
GO
GRANT UPDATE ON SCHEMA::[SGFADM] TO [SGF_App]
GO

--Allow user to connect to database
GRANT CONNECT TO [SGF_Adm]
GRANT CONNECT TO [SGF_App]