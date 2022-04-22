USE HealthPetBD
GO 

-- Se crea la tabla que almacena la información de una categoría.
IF  EXISTS (SELECT * FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') 
    AND type in (N'U'))
DROP TABLE [dbo].[Categories]
GO

CREATE TABLE dbo.Categories
(
    CategoryId INT IDENTITY(1, 1) NOT NULL,
    Description VARCHAR(200) NOT NULL
)
ALTER TABLE dbo.Categories ADD CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED
(
   CategoryId ASC 
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,  IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Se crea la tabla que almacena la información de un tipo de mascota.
IF  EXISTS (SELECT * FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[dbo].[PetTypes]') 
    AND type in (N'U'))
DROP TABLE [dbo].[PetTypes]
GO

CREATE TABLE dbo.PetTypes
(
    PetTypeId INT IDENTITY(1,1) NOT NULL,
    Description VARCHAR(200) NOT NULL,  
)
ALTER TABLE dbo.PetTypes ADD CONSTRAINT [PK_PetTypes] PRIMARY KEY CLUSTERED
(
   PetTypeId ASC 
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,  IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Se crea la tabla que almacena la información de una mascota.
IF  EXISTS (SELECT * FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[dbo].[Pets]') 
    AND type in (N'U'))
DROP TABLE [dbo].[Pets]
GO

CREATE TABLE dbo.Pets
(
    PetId INT IDENTITY(1,1) NOT NULL,
    Name VARCHAR(200) NOT NULL,  
    PetTypeId INT NOT NULL, 
    Age TINYINT NOT NULL,
    Breed VARCHAR(150) NOT NULL
)
ALTER TABLE dbo.Pets ADD CONSTRAINT [PK_Pets] PRIMARY KEY CLUSTERED
(
   PetId ASC 
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,  IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Pets]  WITH CHECK ADD  CONSTRAINT [FK_PetTypeId_Pet] FOREIGN KEY([PetTypeId])
REFERENCES [dbo].[PetTypes] ([PetTypeId])

-- Se crea la tabla que almacena la información de un dueño.
IF  EXISTS (SELECT * FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[dbo].[Owners]') 
    AND type in (N'U'))
DROP TABLE [dbo].[Owners]
GO

CREATE TABLE dbo.Owners
(
    OwnerId INT IDENTITY(1,1) NOT NULL,
    Name VARCHAR(100) NOT NULL,  
    LastName VARCHAR(200) NOT NULL,
    ID VARCHAR(50) NOT NULL,
    Phone VARCHAR(25) NOT NULL,
    Email VARCHAR(100) NOT NULL   
)
ALTER TABLE dbo.Owners ADD CONSTRAINT [PK_Owners] PRIMARY KEY CLUSTERED
(
   OwnerId ASC 
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,  IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Se crea la tabla que almacena la información de una relación entre un dueño y varias mascotas.
IF  EXISTS (SELECT * FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[dbo].[OwnerPets]') 
    AND type in (N'U'))
DROP TABLE [dbo].[OwnerPets]
GO

CREATE TABLE dbo.OwnerPets
(
    PetId INT NOT NULL,
    OwnerId INT NOT NULL,  
    ID_Owner VARCHAR(50) NOT NULL 
)
ALTER TABLE dbo.OwnerPets ADD CONSTRAINT [PK_OwnerPets] PRIMARY KEY CLUSTERED
(
   PetId, OwnerId ASC 
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,  IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OwnerPets]  WITH CHECK ADD  CONSTRAINT [FK_OwnerPets_OwnerId] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[Owners] ([OwnerId])
GO
ALTER TABLE [dbo].[OwnerPets]  WITH CHECK ADD  CONSTRAINT [Fk_OwnerPets_PetId] FOREIGN KEY([PetId])
REFERENCES [dbo].[Pets] ([PetId])

-- Se crea la tabla que almacena la información de una cita.
IF  EXISTS (SELECT * FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') 
    AND type in (N'U'))
DROP TABLE [dbo].[Appointments]
GO

CREATE TABLE dbo.Appointments
(
    AppointmentId INT IDENTITY(1,1) NOT NULL,
    OwnerId INT NOT NULL,
    PetId INT NOT NULL,
    CategoryId INT NOT NULL,  
    Hour VARCHAR(5) NOT NULL,
    Date DATETIME NOT NULL,
    State VARCHAR(25) NOT NULL,
)
ALTER TABLE dbo.Appointments ADD CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED
(
   AppointmentId ASC 
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,  IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_OwnerId] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[Owners] ([OwnerId])
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_PetId] FOREIGN KEY([PetId])
REFERENCES [dbo].[Pets] ([PetId])
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [Fk_Appointments_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])

-- Se crea la tabla que almacena la información de un horario.
IF  EXISTS (SELECT * FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[dbo].[Schedules]') 
    AND type in (N'U'))
DROP TABLE [dbo].[Schedules]
GO

CREATE TABLE dbo.Schedules
(
    ScheduleId INT IDENTITY(1, 1) NOT NULL,
    Schedule VARCHAR(5) NOT NULL
)
ALTER TABLE dbo.Schedules ADD CONSTRAINT [PK_Schedules] PRIMARY KEY CLUSTERED
(
   ScheduleId ASC 
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,  IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
