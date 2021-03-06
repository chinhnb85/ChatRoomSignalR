USE [Main]
GO
/****** Object:  StoredProcedure [dbo].[sp_Account_Add]    Script Date: 4/28/2016 11:55:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_Account_Add]
@UserName varchar(50),
@Password varchar(50)
as
insert into Account(UserName,[Password]) values(@UserName,@Password)

GO
/****** Object:  Table [dbo].[Account]    Script Date: 4/28/2016 11:55:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Avatar] [varchar](250) NULL,
	[Phone] [varchar](50) NULL,
	[Address] [nvarchar](250) NULL,
	[Email] [varchar](50) NULL,
	[Description] [ntext] NULL,
	[AdminCode] [int] NULL CONSTRAINT [DF_Account_AdminCode]  DEFAULT ((0)),
	[Status] [int] NULL CONSTRAINT [DF_Account_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Category]    Script Date: 4/28/2016 11:55:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CheckIn]    Script Date: 4/28/2016 11:55:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CheckIn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Latitude] [decimal](18, 7) NULL,
	[Longitude] [decimal](18, 7) NULL,
	[Title] [nvarchar](250) NULL,
	[CheckDate] [datetime] NULL,
	[AccountId] [int] NULL,
 CONSTRAINT [PK_CheckIn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Conversation]    Script Date: 4/28/2016 11:55:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConnectionIdUser] [nvarchar](50) NULL,
	[ConnectionIdAdmin] [nvarchar](50) NULL,
	[UserGroup] [nvarchar](50) NULL,
	[Msg] [ntext] NULL,
	[MsgDate] [datetime] NULL,
	[MsgDuration] [datetime] NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[UserId] [int] NULL,
	[AdminId] [int] NULL,
 CONSTRAINT [PK_Conversation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Employee]    Script Date: 4/28/2016 11:55:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Sal] [decimal](10, 2) NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Employee_Audit]    Script Date: 4/28/2016 11:55:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee_Audit](
	[Id] [int] NULL,
	[Name] [nvarchar](50) NULL,
	[Sal] [decimal](10, 2) NULL,
	[Audit_Action] [nvarchar](50) NULL,
	[Audit_Timestamp] [datetime] NULL
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[CheckIn]  WITH CHECK ADD  CONSTRAINT [FK_CheckIn_Account] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([Id])
GO
ALTER TABLE [dbo].[CheckIn] CHECK CONSTRAINT [FK_CheckIn_Account]
GO
