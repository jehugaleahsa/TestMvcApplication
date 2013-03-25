USE [TestDatabase]
GO

/****** Object:  Table [dbo].[Customer]    Script Date: 3/13/2013 1:02:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Customer](
	[CustomerId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[BirthDate] [datetime] NOT NULL,
	[Height] [int] NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF_Customer_CustomerId]  DEFAULT (newid()) FOR [CustomerId]
GO

