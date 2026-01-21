-- Add Currency column to DeliveryOrder table
ALTER TABLE `DeliveryOrder` ADD COLUMN `Currency` VARCHAR(3) DEFAULT 'Q';

-- Update the stored procedure sp_DeliveryOrder_Update to include Currency
DROP PROCEDURE IF EXISTS sp_DeliveryOrder_Update;
DELIMITER $$
CREATE PROCEDURE sp_DeliveryOrder_Update(IN pId INT, IN pStatusId INT, IN pTotal DOUBLE, IN pCurrency VARCHAR(3))
BEGIN
    UPDATE DeliveryOrder SET StatusId = pStatusId, Total = pTotal, Currency = pCurrency WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$
DELIMITER ;

-- Update the stored procedure sp_DeliveryOrder_GetAll to include Currency
DROP PROCEDURE IF EXISTS sp_DeliveryOrder_GetAll;
DELIMITER $$
CREATE PROCEDURE sp_DeliveryOrder_GetAll()
BEGIN
    SELECT
        a.Id, a.Created, a.Total, a.Currency, a.StatusId,
        b.Id, b.Name
    FROM DeliveryOrder a
    INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id;
END$$
DELIMITER ;
