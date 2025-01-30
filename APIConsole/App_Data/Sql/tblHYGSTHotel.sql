USE [StaticData]
GO

/****** Object:  Table [dbo].[tblHYGSTHotel]    Script Date: 30-01-2025 14:17:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblHYGSTHotel](
	[hotel_id] [nvarchar](50) NOT NULL,
	[name] [nvarchar](500) NULL,
	[country] [nvarchar](50) NULL,
	[city] [nvarchar](50) NULL,
	[city_Id] [nvarchar](50) NULL,
	[region] [nvarchar](50) NULL,
	[phone] [nvarchar](50) NULL,
	[email] [nvarchar](50) NULL,
	[website] [nvarchar](50) NULL,
	[checkIn] [nvarchar](20) NULL,
	[checkOut] [nvarchar](20) NULL,
	[longitude] [nvarchar](20) NULL,
	[latitude] [nvarchar](20) NULL,
	[address] [nvarchar](500) NULL,
	[postcode] [nvarchar](20) NULL,
	[descriptions] [nvarchar](max) NULL,
	[images] [nvarchar](max) NULL,
	[last_updated] [datetime] NULL,
	[is_updated] [bit] NULL,
 CONSTRAINT [PK_tblHYGSTHotel] PRIMARY KEY CLUSTERED 
(
	[hotel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


