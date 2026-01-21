-- =====================================================
-- Add Concept column to DeliveryOrder table
-- =====================================================

-- Add Concept column
ALTER TABLE DeliveryOrder ADD COLUMN Concept TEXT NULL;

-- =====================================================
-- Update sp_DeliveryOrder_Update stored procedure
-- =====================================================

DELIMITER $$

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_Update$$
CREATE PROCEDURE sp_DeliveryOrder_Update(
    IN pId INT,
    IN pStatusId INT,
    IN pTotal DOUBLE,
    IN pCurrency VARCHAR(3),
    IN pConcept TEXT
)
BEGIN
    UPDATE DeliveryOrder
    SET StatusId = pStatusId,
        Total = pTotal,
        Currency = pCurrency,
        Concept = pConcept
    WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DELIMITER ;
