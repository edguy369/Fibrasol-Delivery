-- Update Script: Add SalespersonId to Invoice table
-- This script adds the SalespersonId column and foreign key constraint to the existing Invoice table

-- Step 1: Add the SalespersonId column to the Invoice table
-- Note: We're adding it as nullable first, then we'll update it and make it NOT NULL
ALTER TABLE `Invoice`
ADD COLUMN `SalespersonId` INT NULL AFTER `SignedAttatchment`;

-- Step 2: Set a default SalespersonId for existing invoices
-- This assumes there's at least one SalesPerson in the SalesPerson table
-- You may need to adjust this based on your specific requirements
UPDATE `Invoice`
SET `SalespersonId` = (SELECT MIN(`Id`) FROM `SalesPerson` LIMIT 1)
WHERE `SalespersonId` IS NULL;

-- Step 3: Make the column NOT NULL now that all rows have values
ALTER TABLE `Invoice`
MODIFY COLUMN `SalespersonId` INT NOT NULL;

-- Step 4: Add the foreign key constraint
ALTER TABLE `Invoice`
ADD CONSTRAINT `FK_Invoice_SalesPerson_SalespersonId`
FOREIGN KEY (`SalespersonId`) REFERENCES `SalesPerson` (`Id`) ON DELETE CASCADE;