IF object_id('SplitIdCVSList') IS NOT NULL
    drop function SplitIdCVSList;
GO

CREATE FUNCTION [dbo].[SplitIdCVSList] (@InStr VARCHAR(MAX))
RETURNS @TempTab TABLE
   (id int not null)
AS
BEGIN
    ;-- Ensure input ends with comma
	SET @InStr = REPLACE(@InStr + ',', ',,', ',')
	DECLARE @SP INT
DECLARE @VALUE VARCHAR(1000)
WHILE PATINDEX('%,%', @INSTR ) <> 0 
BEGIN
   SELECT  @SP = PATINDEX('%,%',@INSTR)
   SELECT  @VALUE = LEFT(@INSTR , @SP - 1)
   SELECT  @INSTR = STUFF(@INSTR, 1, @SP, '')
   INSERT INTO @TempTab(id) VALUES (@VALUE)
END
	RETURN
END
GO


IF object_id('uspGetProductionInformation') IS NOT NULL
    drop procedure uspGetProductionInformation;
GO

create procedure dbo.uspGetProductionInformation
	@ids as varchar(1000)
as
begin

DECLARE @ProductIds as table
(
  ID int
);

DECLARE @LocationQuantities as table
(
  ID int,
  MaxLocationQuantity int,
  LocationId int,
  LocationName nvarchar(50)
);

INSERT INTO @ProductIds 
	SELECT * FROM dbo.SplitIdCVSList(@ids)

INSERT INTO @LocationQuantities 
	select ProductID, 
	[pi].Quantity,
	[pi].LocationID,
	l.Name
	from Production.ProductInventory [pi]
	inner join @ProductIds p on p.ID = [pi].ProductID
	inner join Production.Location l on [pi].LocationID = l.LocationID

    select p.ID as [RequestedProductId], 
		coalesce(cast(pp.ProductID as bit), 0) as [Found],
       pp.Name,
	   pp.ProductNumber,

	   --The average StandardCost from the Production.ProductCostHistory using the year value from the StartDate.
	    (select AVG(StandardCost) from Production.ProductCostHistory ch 
			where ch.ProductID = p.ID and Year(ch.StartDate) = Year(pp.SellStartDate)) as AverageCost,
			
	   --The name of the Production.Location that contains the MAX Quantity from the Production.ProductInventory table.
	   (select top 1 l.[LocationName] from @LocationQuantities l where l.ID = p.ID order by l.MaxLocationQuantity desc) as MaxQuantityLocation
	  
	   from @ProductIds p
	left outer join Production.Product pp on p.ID = pp.ProductID
end
go

exec uspGetProductionInformation '748, 768, 858, 10000'

exec uspGetProductionInformation '778, 802, 859, 900'