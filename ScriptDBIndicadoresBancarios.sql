USE master
GO
IF EXISTS(SELECT * FROM dbo.sysdatabases WHERE name = N'BD_Indicadores')
DROP DATABASE BD_Indicadores
GO
CREATE DATABASE BD_Indicadores
GO
USE BD_Indicadores
GO
------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------CREACION TABLAS CON PKs
------------------------------------------------------------------------------------------------------------------

CREATE TABLE TB_Catalogo
(
	IdCatalogo INT NOT NULL IDENTITY(1,1),
	Prefijo CHAR(15) NOT NULL,
	Descripcion VARCHAR(35) NOT NULL

	CONSTRAINT PK_TB_Catalogo PRIMARY KEY CLUSTERED (IdCatalogo) ON [PRIMARY]
)

CREATE TABLE TB_Metadata
(
	IdMetadata INT NOT NULL IDENTITY(1,1),
	Nombre VARCHAR (35)NOT NULL,
	Descripcion VARCHAR(60) NOT NULL,
	Prefijo VARCHAR(3) NOT NULL -- SE QUEDA COMO VARCHAR PORQUE EN LOS REQUERIMIENTOS DICE QUE PUEDE CONTENER N CANTIDAD DE VALORES

	CONSTRAINT PK_TB_Metadata PRIMARY KEY CLUSTERED (IdMetadata) ON [PRIMARY]
)
GO

CREATE TABLE TB_Posvalor
(
	IdPosvalor INT not null IDENTITY(1,1),
	Valor VARCHAR (20)not null, -- SE DEJA EN VARCHAR PORQUE ESTA CONSTITUIDO EN PARTE POR ELECCION DEL USUARIO
	--EL POS VALOR SE COMPONE DE 2 DIGINOS UN GUION ( - ) Y LA PARTE FINAL A ELECCION DEL USUARIO
	--SE VA A TENER QUE VALIDAR MANUALMENTE EN LA APLICACIÓN POR LA CUESTION DEL GUION CON 2 DIGITOS, PARA MANDARLO VALIDADO YA Y QUE SE INSERTE EN LA BD
	--SIMPLEMENTE COMO UN VARCHAR NORMAL
	Descripcion VARCHAR (50)not null,
	IdMetadata INT not null 

	CONSTRAINT PK_TB_Posvalor PRIMARY KEY CLUSTERED (IdPosvalor) ON [PRIMARY]
)
GO

CREATE TABLE TB_Periodo
(
	IdPeriodo INT not null IDENTITY(1,1),
	Siglas CHAR(1) not null UNIQUE, -- PERIODO SOLO LLEVA UN DIGITO( D-M-S-T)

	CONSTRAINT PK_TB_Periodo PRIMARY KEY CLUSTERED (IdPeriodo) ON [PRIMARY]
)
GO

CREATE TABLE TB_Estado
(
	IdEstado INT not null IDENTITY(1,1),
	Sigla CHAR(4) not null UNIQUE, --TIENE 4 CARACTERES (ACTI-PASI-PATR)
	Descripcion VARCHAR(35) not null

	CONSTRAINT PK_TB_Estado PRIMARY KEY CLUSTERED (IdEstado) ON [PRIMARY]
)
GO

CREATE TABLE TB_UnidadMedida
(
	IdUnidad INT not null IDENTITY(1,1),
	Siglas CHAR(3) not null UNIQUE, --TIENE 3 CARACTERES (USD, CRC, ETC)
	Descripcion VARCHAR(20) not null

	CONSTRAINT PK_TB_UnidadMedida PRIMARY KEY CLUSTERED (IdUnidad) ON [PRIMARY]
)
GO

CREATE TABLE TB_Indicador
(
	IdIndicador INT not null IDENTITY(1,1),
	IdCatalogo INT NOT NULL,
	IdUnidad INT not null,
	IdPeriodo INT not null,
	IdEstado INT not null

	CONSTRAINT PK_TB_Indicador PRIMARY KEY CLUSTERED (IdIndicador) ON [PRIMARY]
)
GO

CREATE TABLE TB_IndicadorPosvalor
(
	IdIndiPos INT NOT NULL IDENTITY(1,1),
	IdIndicador INT not null,
	IdPosvalor INT not null,
	Orden INT not null

	CONSTRAINT PK_IndicadorPosvalor PRIMARY KEY CLUSTERED (IdindiPos) ON [PRIMARY]
)
GO
------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------CREACION FKs
------------------------------------------------------------------------------------------------------------------

---------------------------------------------------------------------FKs TB_Posvalor
ALTER TABLE TB_Posvalor WITH NOCHECK ADD
CONSTRAINT FK_Posvalor_Metadata FOREIGN KEY (IdMetadata) REFERENCES TB_Metadata (IdMetadata)
ALTER TABLE TB_Posvalor CHECK CONSTRAINT FK_Posvalor_Metadata
GO

---------------------------------------------------------------------FKs TB_Indicador
ALTER TABLE TB_Indicador WITH NOCHECK ADD
CONSTRAINT FK_Unidad_Indicador FOREIGN KEY (IdUnidad) REFERENCES TB_UnidadMedida (IdUnidad)
ALTER TABLE TB_Indicador CHECK CONSTRAINT FK_Unidad_Indicador 
GO

ALTER TABLE TB_Indicador WITH NOCHECK ADD
CONSTRAINT FK_Periodo_Indicador FOREIGN KEY (IdPeriodo) REFERENCES TB_Periodo (IdPeriodo)
ALTER TABLE TB_Indicador CHECK CONSTRAINT FK_Periodo_Indicador 
GO

ALTER TABLE TB_Indicador WITH NOCHECK ADD
CONSTRAINT FK_Estado_Indicador FOREIGN KEY (IdEstado) REFERENCES TB_Estado (IdEstado)
ALTER TABLE TB_Indicador CHECK CONSTRAINT FK_Estado_Indicador
GO

ALTER TABLE TB_Indicador WITH NOCHECK ADD
CONSTRAINT FK_Catalogo_Indicador FOREIGN KEY (IdCatalogo) REFERENCES TB_Catalogo (IdCatalogo)
ALTER TABLE TB_Indicador CHECK CONSTRAINT FK_Catalogo_Indicador
GO

---------------------------------------------------------------------FKs TB_IndicadorPosvalor
ALTER TABLE TB_IndicadorPosvalor WITH NOCHECK ADD
CONSTRAINT FK_Indicador_IndicadorPosvalor FOREIGN KEY (IdIndicador) REFERENCES TB_Indicador (IdIndicador)
ALTER TABLE TB_IndicadorPosvalor CHECK CONSTRAINT FK_Indicador_IndicadorPosvalor
GO

ALTER TABLE TB_IndicadorPosvalor WITH NOCHECK ADD
CONSTRAINT FK_Posvalor_IndicadorPosvalor FOREIGN KEY (IdPosvalor) REFERENCES TB_Posvalor (IdPosvalor)
ALTER TABLE TB_IndicadorPosvalor CHECK CONSTRAINT FK_Posvalor_IndicadorPosvalor
GO

-- VISTA

CREATE VIEW V_SelectIndicadores

AS

SELECT	TB_Indicador.IdIndicador Id, 
		TB_Catalogo.Prefijo Prefijo,
		TB_Estado.Sigla Estado,
		TB_Periodo.Siglas Periodo,
		TB_UnidadMedida.Siglas Moneda
		
		FROM TB_Indicador

INNER JOIN TB_Catalogo ON TB_Catalogo.IdCatalogo = TB_Indicador.IdCatalogo
INNER JOIN TB_Estado ON TB_Estado.IdEstado = TB_Indicador.IdEstado
INNER JOIN TB_Periodo ON TB_Periodo.IdPeriodo = TB_Indicador.IdPeriodo
INNER JOIN TB_UnidadMedida ON TB_UnidadMedida.IdUnidad = TB_Indicador.IdUnidad

GO

CREATE VIEW V_SelectMetadata

AS


SELECT	TB_IndicadorPosvalor.IdIndicador Indicador,
		TB_Metadata.Prefijo Metadata,
		TB_Posvalor.Valor Posvalor

FROM

TB_IndicadorPosvalor

INNER JOIN TB_Posvalor ON TB_Posvalor.IdPosvalor = TB_IndicadorPosvalor.IdPosvalor
INNER JOIN TB_Metadata ON TB_Posvalor.IdMetadata = TB_Metadata.IdMetadata

GO

--INSERT

INSERT INTO TB_Estado
(
	Sigla,
	Descripcion
)
VALUES
('ACTI','Activo'),--1
('PASI','Pasivo'),--2
('PATR','Patrimonio')--3
GO

INSERT INTO TB_Periodo
(
	Siglas
)
VALUES
('D'),--1
('M'),--2
('S'),--3
('T')--4
GO

INSERT INTO TB_UnidadMedida
(
	Siglas,
	Descripcion
)
VALUES

('CRC','Costa Rica'),--1
('USD','Estados Unidos'),--2
('JPY','Yen Japones')--3
go

INSERT INTO TB_Metadata
(
	Nombre,
	Prefijo,
	Descripcion
)
VALUES
('Carro','CA_','Se usara para detallar las marcas de carros'),--1
('Provinvia','PV_','Provincias del pais'),--2
('Universidad','UN_','Universidades que hay en el pais')--3
go

INSERT INTO TB_Posvalor
(
	Valor,
	Descripcion,
	IdMetadata
)
VALUES
('TYT','Toyota',1),--1
('LMN','Limon',2),--2
('UAM','Universidad Americana',3),--3
('HND','Honda',1),--4
('CTG','Cartago',2),--5
('UCR','Universidad de Costa Rica',3)--6
go

INSERT INTO TB_Catalogo
(
	Prefijo,
	Descripcion
)
VALUES
('1000000000ACT02','Universidades que poseen carro'),--1
('2000000000PAT03','Universidades en las provincias')--2
go

INSERT INTO TB_Indicador
(
	IdCatalogo,
	IdEstado,
	IdPeriodo,
	IdUnidad
)
VALUES
(1,1,2,3),--1
(2,3,4,2)--2

INSERT INTO TB_IndicadorPosvalor
(
	IdIndicador,
	IdPosvalor,
	Orden
)
VALUES
(1,6,1),
(1,1,2),
(2,3,1),
(2,5,2)
