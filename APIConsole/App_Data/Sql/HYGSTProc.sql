USE [StaticData]
GO
/****** Object:  StoredProcedure [dbo].[HYGSTProc]    Script Date: 30-01-2025 17:35:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[HYGSTProc]
@flag int=NULL,
@jsonData NVarChar(max)=NULL,
@hotel_id NVARCHAR(50)=NULL,
@images NVARCHAR(max)=NULL,
@address NVARCHAR(1000)=NULL,
@postcode NVARCHAR(40)=NULL,
@longitude NVARCHAR(40)=NULL,
@latitude NVARCHAR(40)=NULL,
@phone NVARCHAR(100)=NULL,
@email NVARCHAR(100)=NULL,
@website NVARCHAR(250)=NULL,
@descriptions NVARCHAR(max)=NULL,
@checkIn NVARCHAR(40)=NULL,
@checkOut NVARCHAR(40)=NULL,
@isTest int=NULL
AS  
BEGIN
IF (@flag = 1)  
BEGIN 

IF (ISJSON(@jsonData) = 1)  
BEGIN   
PRINT 'Imported JSON is Valid'  

INSERT INTO tblHYGSTHotel (hotel_id,last_updated,name,country,region,city,city_Id)
SELECT * FROM OpenJson(@jsonData)
WITH (hotel_id NVARCHAR(50) '$.hotel_id',
last_updated NVARCHAR(50) '$.last_updated',
name NVARCHAR(500) '$.name',
country NVARCHAR(50) '$.country',
region NVARCHAR(50) '$.region',
city NVARCHAR(50) '$.city',
city_Id NVARCHAR(50) '$.city_Id'
)

END   
ELSE  
BEGIN  
PRINT 'Invalid JSON Imported'  
END  
 


END 



IF (@flag = 2)  
BEGIN 
select hotel_id from tblHYGSTHotel  where is_updated is null or is_updated=0
END  


IF (@flag = 3)  
BEGIN 
update tblHYGSTHotel set images=@images,[address]=@address,postcode=@postcode,
longitude=@longitude,latitude=@latitude,phone=@phone,email=@email,website=@website,
descriptions=@descriptions,checkIn=@checkIn,checkOut=@checkOut,is_updated=1,last_updated=GETDATE(),isTest=@isTest
where hotel_id=@hotel_id
END 

END  


