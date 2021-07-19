/****** Object:  Table [dbo].[tblRequests]    Script Date: 7/19/2021 6:37:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblRequests](
	[Id] [uniqueidentifier] NOT NULL,
	[CustomerEmail] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2000) NOT NULL,
 CONSTRAINT [PK_tblRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

