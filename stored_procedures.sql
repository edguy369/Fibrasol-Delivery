-- =====================================================
-- Stored Procedures for Fibrasol Delivery System
-- =====================================================
-- This script creates all stored procedures for the application
-- Naming convention: sp_<TableName>_<Operation>
-- =====================================================

DELIMITER $$

-- =====================================================
-- CLIENT STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_Client_Count$$
CREATE PROCEDURE sp_Client_Count()
BEGIN
    SELECT COUNT(Id) FROM Clients;
END$$

DROP PROCEDURE IF EXISTS sp_Client_Create$$
CREATE PROCEDURE sp_Client_Create(IN pName VARCHAR(255))
BEGIN
    INSERT INTO Clients (Name) VALUES (pName);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_Client_Delete$$
CREATE PROCEDURE sp_Client_Delete(IN pId INT)
BEGIN
    DELETE FROM Clients WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_Client_GetAll$$
CREATE PROCEDURE sp_Client_GetAll()
BEGIN
    SELECT * FROM Clients;
END$$

DROP PROCEDURE IF EXISTS sp_Client_GetByName$$
CREATE PROCEDURE sp_Client_GetByName(IN pName VARCHAR(255))
BEGIN
    SELECT * FROM Clients WHERE Name = pName;
END$$

DROP PROCEDURE IF EXISTS sp_Client_Update$$
CREATE PROCEDURE sp_Client_Update(IN pId INT, IN pName VARCHAR(255))
BEGIN
    UPDATE Clients SET Name = pName WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

-- =====================================================
-- SALESPERSON STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_SalesPerson_Count$$
CREATE PROCEDURE sp_SalesPerson_Count()
BEGIN
    SELECT COUNT(Id) FROM SalesPerson;
END$$

DROP PROCEDURE IF EXISTS sp_SalesPerson_Create$$
CREATE PROCEDURE sp_SalesPerson_Create(IN pName VARCHAR(255))
BEGIN
    INSERT INTO SalesPerson (Name) VALUES (pName);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_SalesPerson_Delete$$
CREATE PROCEDURE sp_SalesPerson_Delete(IN pId INT)
BEGIN
    DELETE FROM SalesPerson WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_SalesPerson_GetAll$$
CREATE PROCEDURE sp_SalesPerson_GetAll()
BEGIN
    SELECT * FROM SalesPerson;
END$$

DROP PROCEDURE IF EXISTS sp_SalesPerson_GetByName$$
CREATE PROCEDURE sp_SalesPerson_GetByName(IN pName VARCHAR(255))
BEGIN
    SELECT * FROM SalesPerson WHERE Name = pName;
END$$

DROP PROCEDURE IF EXISTS sp_SalesPerson_Update$$
CREATE PROCEDURE sp_SalesPerson_Update(IN pId INT, IN pName VARCHAR(255))
BEGIN
    UPDATE SalesPerson SET Name = pName WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_SalesPerson_GetSalesReport$$
CREATE PROCEDURE sp_SalesPerson_GetSalesReport(IN pStartDate DATETIME, IN pEndDate DATETIME)
BEGIN
    SELECT
        sp.Id,
        sp.Name,
        COALESCE(SUM(CASE
            WHEN do.Created >= pStartDate AND do.Created <= pEndDate
            THEN i.Value
            ELSE 0
        END), 0) as TotalSales
    FROM SalesPerson sp
    LEFT JOIN Invoice i ON sp.Id = i.SalesPersonId
    LEFT JOIN BackOrder bo ON i.BackorderId = bo.Id
    LEFT JOIN DeliveryOrder do ON bo.DeliveryOrderId = do.Id
    GROUP BY sp.Id, sp.Name
    ORDER BY TotalSales DESC;
END$$

-- =====================================================
-- RIDER/DRIVER STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_Rider_Count$$
CREATE PROCEDURE sp_Rider_Count()
BEGIN
    SELECT COUNT(Id) FROM Drivers;
END$$

DROP PROCEDURE IF EXISTS sp_Rider_Create$$
CREATE PROCEDURE sp_Rider_Create(IN pName VARCHAR(255))
BEGIN
    INSERT INTO Drivers (Name) VALUES (pName);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_Rider_Delete$$
CREATE PROCEDURE sp_Rider_Delete(IN pId INT)
BEGIN
    DELETE FROM Drivers WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_Rider_GetAll$$
CREATE PROCEDURE sp_Rider_GetAll()
BEGIN
    SELECT * FROM Drivers;
END$$

DROP PROCEDURE IF EXISTS sp_Rider_Update$$
CREATE PROCEDURE sp_Rider_Update(IN pId INT, IN pName VARCHAR(255))
BEGIN
    UPDATE Drivers SET Name = pName WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

-- =====================================================
-- DELIVERY ORDER STATUS STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_DeliveryOrderStatus_Count$$
CREATE PROCEDURE sp_DeliveryOrderStatus_Count()
BEGIN
    SELECT COUNT(Id) FROM DeliveryOrderStatus;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrderStatus_Create$$
CREATE PROCEDURE sp_DeliveryOrderStatus_Create(IN pName VARCHAR(255))
BEGIN
    INSERT INTO DeliveryOrderStatus (Name) VALUES (pName);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrderStatus_Delete$$
CREATE PROCEDURE sp_DeliveryOrderStatus_Delete(IN pId INT)
BEGIN
    DELETE FROM DeliveryOrderStatus WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrderStatus_GetAll$$
CREATE PROCEDURE sp_DeliveryOrderStatus_GetAll()
BEGIN
    SELECT * FROM DeliveryOrderStatus;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrderStatus_Update$$
CREATE PROCEDURE sp_DeliveryOrderStatus_Update(IN pId INT, IN pName VARCHAR(255))
BEGIN
    UPDATE DeliveryOrderStatus SET Name = pName WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

-- =====================================================
-- DELIVERY ORDER STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_Count$$
CREATE PROCEDURE sp_DeliveryOrder_Count()
BEGIN
    SELECT COUNT(Id) FROM DeliveryOrder;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_Create$$
CREATE PROCEDURE sp_DeliveryOrder_Create(IN pStatusId INT, IN pTotal DOUBLE)
BEGIN
    INSERT INTO DeliveryOrder (StatusId, Total) VALUES (pStatusId, pTotal);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_Delete$$
CREATE PROCEDURE sp_DeliveryOrder_Delete(IN pId INT)
BEGIN
    DELETE FROM DeliveryOrder WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_GetAll$$
CREATE PROCEDURE sp_DeliveryOrder_GetAll()
BEGIN
    SELECT
        a.Id, a.Created, a.Total, a.Currency, a.Concept, a.StatusId,
        b.Id, b.Name
    FROM DeliveryOrder a
    INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_GetAllUnsigned$$
CREATE PROCEDURE sp_DeliveryOrder_GetAllUnsigned()
BEGIN
    SELECT DISTINCT
        a.Id, a.Created, a.Total, a.Currency, a.Concept, a.StatusId,
        b.Id, b.Name
    FROM DeliveryOrder a
    INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id
    INNER JOIN BackOrder c ON a.Id = c.DeliveryOrderId
    INNER JOIN Invoice e ON e.BackorderId = c.Id
    WHERE e.Attatchment IS NOT NULL
      AND e.Attatchment != ''
      AND (e.SignedAttatchment IS NULL OR e.SignedAttatchment = '');
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_GetById$$
CREATE PROCEDURE sp_DeliveryOrder_GetById(IN pId INT)
BEGIN
    SELECT
        a.Id, a.Created, a.Total, a.StatusId,
        b.Id, b.Name,
        f.Id AS RiderAssignationId,
        g.Id, g.Name,
        c.Id AS BackorderId, c.Id, c.Number, c.Weight,
        d.Id AS BackOrderClientId, d.Id, d.Name,
        e.Id AS InvoiceId, e.Id, e.Address, e.Reference, e.Value, e.Attatchment, e.SignedAttatchment, e.Currency,
        ic.Id AS InvoiceClientId, ic.Id, ic.Name,
        h.Id AS SalesPersonId, h.Id, h.Name
    FROM DeliveryOrder a
    INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id
    LEFT JOIN BackOrder c ON a.Id = c.DeliveryOrderId
    LEFT JOIN Clients d ON d.Id = c.ClientId
    LEFT JOIN Invoice e ON e.BackorderId = c.Id
    LEFT JOIN Clients ic ON ic.Id = e.ClientId
    LEFT JOIN DeliveryOrderDrivers f ON f.DeliveryOrderId = a.Id
    LEFT JOIN Drivers g ON g.Id = f.DriverId
    LEFT JOIN SalesPerson h ON h.Id = e.SalesPersonId
    WHERE a.Id = pId;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrder_Update$$
CREATE PROCEDURE sp_DeliveryOrder_Update(IN pId INT, IN pStatusId INT, IN pTotal DOUBLE, IN pCurrency VARCHAR(3))
BEGIN
    UPDATE DeliveryOrder SET StatusId = pStatusId, Total = pTotal, Currency = pCurrency WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

-- =====================================================
-- BACKORDER STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_BackOrder_Create$$
CREATE PROCEDURE sp_BackOrder_Create(IN pClientId INT, IN pDeliveryOrderId INT, IN pNumber VARCHAR(255), IN pWeight DOUBLE)
BEGIN
    -- NULLIF converts 0 to NULL for optional ClientId
    INSERT INTO BackOrder (ClientId, DeliveryOrderId, Number, Weight)
    VALUES (NULLIF(pClientId, 0), pDeliveryOrderId, pNumber, pWeight);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_BackOrder_Delete$$
CREATE PROCEDURE sp_BackOrder_Delete(IN pId INT)
BEGIN
    DELETE FROM BackOrder WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_BackOrder_Update$$
CREATE PROCEDURE sp_BackOrder_Update(IN pId INT, IN pClientId INT, IN pNumber VARCHAR(255), IN pWeight DOUBLE)
BEGIN
    -- NULLIF converts 0 to NULL for optional ClientId
    UPDATE BackOrder SET ClientId = NULLIF(pClientId, 0), Number = pNumber, Weight = pWeight WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

-- =====================================================
-- INVOICE STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_Invoice_Count$$
CREATE PROCEDURE sp_Invoice_Count()
BEGIN
    SELECT COUNT(Id) FROM Invoice;
END$$

DROP PROCEDURE IF EXISTS sp_Invoice_CountSigned$$
CREATE PROCEDURE sp_Invoice_CountSigned()
BEGIN
    SELECT COUNT(Id) FROM Invoice WHERE SignedAttatchment != '';
END$$

DROP PROCEDURE IF EXISTS sp_Invoice_Create$$
CREATE PROCEDURE sp_Invoice_Create(
    IN pBackorderId INT,
    IN pClientId INT,
    IN pAddress VARCHAR(500),
    IN pReference VARCHAR(255),
    IN pValue DOUBLE,
    IN pAttatchment VARCHAR(500),
    IN pSignedAttatchment VARCHAR(500),
    IN pSalesPersonId INT,
    IN pCurrency VARCHAR(3)
)
BEGIN
    INSERT INTO Invoice (BackorderId, ClientId, Address, Reference, Value, Attatchment, SignedAttatchment, SalesPersonId, Currency)
    VALUES (pBackorderId, pClientId, pAddress, pReference, pValue, pAttatchment, pSignedAttatchment, pSalesPersonId, pCurrency);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_Invoice_Delete$$
CREATE PROCEDURE sp_Invoice_Delete(IN pId INT)
BEGIN
    DELETE FROM Invoice WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DROP PROCEDURE IF EXISTS sp_Invoice_Update$$
CREATE PROCEDURE sp_Invoice_Update(
    IN pId INT,
    IN pClientId INT,
    IN pAddress VARCHAR(500),
    IN pReference VARCHAR(255),
    IN pValue DOUBLE,
    IN pAttatchment VARCHAR(500),
    IN pSignedAttatchment VARCHAR(500),
    IN pSalesPersonId INT,
    IN pCurrency VARCHAR(3)
)
BEGIN
    UPDATE Invoice
    SET ClientId = pClientId, Address = pAddress, Reference = pReference, Value = pValue,
        Attatchment = pAttatchment, SignedAttatchment = pSignedAttatchment, SalesPersonId = pSalesPersonId, Currency = pCurrency
    WHERE Id = pId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

-- =====================================================
-- DELIVERY ORDER DRIVERS STORED PROCEDURES
-- =====================================================

DROP PROCEDURE IF EXISTS sp_DeliveryOrderDriver_Create$$
CREATE PROCEDURE sp_DeliveryOrderDriver_Create(IN pDeliveryOrderId INT, IN pDriverId INT)
BEGIN
    INSERT INTO DeliveryOrderDrivers (DeliveryOrderId, DriverId)
    VALUES (pDeliveryOrderId, pDriverId);
    SELECT LAST_INSERT_ID() AS Id;
END$$

DROP PROCEDURE IF EXISTS sp_DeliveryOrderDriver_Delete$$
CREATE PROCEDURE sp_DeliveryOrderDriver_Delete(IN pDeliveryOrderId INT, IN pDriverId INT)
BEGIN
    DELETE FROM DeliveryOrderDrivers WHERE DeliveryOrderId = pDeliveryOrderId AND DriverId = pDriverId;
    SELECT ROW_COUNT() AS AffectedRows;
END$$

DELIMITER ;

-- =====================================================
-- VERIFICATION QUERY
-- =====================================================
-- Run this to verify all stored procedures were created:
-- SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES
-- WHERE ROUTINE_SCHEMA = 'fibrasol_delivery' AND ROUTINE_TYPE = 'PROCEDURE'
-- ORDER BY ROUTINE_NAME;
-- =====================================================
