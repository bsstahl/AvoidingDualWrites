/****** Object:  Table [dbo].[tblMessages]    Script Date: 7/19/2021 6:37:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblMessages](
	[Id] [uniqueidentifier] NOT NULL,
	[RequestId] [uniqueidentifier] NOT NULL,
	[SendToAddress] [nvarchar](255) NOT NULL,
	[MessageSubject] [nvarchar](255) NOT NULL,
	[MessageBody] [nvarchar](4000) NOT NULL,
	[Sent] [bit] NOT NULL,
 CONSTRAINT [PK_tblMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tblMessages]  WITH CHECK ADD  CONSTRAINT [FK_tblMessages_tblRequests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[tblRequests] ([Id])
GO

ALTER TABLE [dbo].[tblMessages] CHECK CONSTRAINT [FK_tblMessages_tblRequests]
GO

