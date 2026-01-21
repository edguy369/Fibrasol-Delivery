-- =====================================================
-- Update BackOrder.ClientId to be nullable
-- This allows creating comandas without a client
-- The client is now defined at the Invoice level
-- =====================================================

-- Step 1: Drop the foreign key constraint
ALTER TABLE `BackOrder` DROP FOREIGN KEY `FK_BackOrder_Clients_ClientId`;

-- Step 2: Modify the column to allow NULL
ALTER TABLE `BackOrder` MODIFY COLUMN `ClientId` INT NULL;

-- Step 3: Re-add the foreign key constraint with ON DELETE SET NULL
ALTER TABLE `BackOrder`
ADD CONSTRAINT `FK_BackOrder_Clients_ClientId`
FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`Id`) ON DELETE SET NULL;

-- Step 4: Update stored procedure sp_BackOrder_Create to handle NULL ClientId
DELIMITER $$

DROP PROCEDURE IF EXISTS sp_BackOrder_Create$$
CREATE PROCEDURE sp_BackOrder_Create(IN pClientId INT, IN pDeliveryOrderId INT, IN pNumber VARCHAR(255), IN pWeight DOUBLE)
BEGIN
    INSERT INTO BackOrder (ClientId, DeliveryOrderId, Number, Weight)
    VALUES (NULLIF(pClientId, 0), pDeliveryOrderId, pNumber, pWeight);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_BackOrder_Update$$
CREATE PROCEDURE sp_BackOrder_Update(IN pId INT, IN pClientId INT, IN pNumber VARCHAR(255), IN pWeight DOUBLE)
BEGIN
    UPDATE BackOrder SET ClientId = NULLIF(pClientId, 0), Number = pNumber, Weight = pWeight WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DELIMITER ;

-- =====================================================
-- Verification: Check the column is now nullable
-- =====================================================
-- Run: DESCRIBE BackOrder;
-- ClientId should show NULL in the Null column
