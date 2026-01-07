-- ============================================================
-- Migration Script: Add ClientId to Invoice (Dual System)
-- ============================================================
-- This migration adds ClientId to Invoice table while keeping
-- the existing ClientId in BackOrder table intact.
--
-- IMPORTANT: BackOrder.ClientId is NOT removed - this is a
-- dual system where both levels can have client references.
-- ============================================================

-- Step 1: Add ClientId column to Invoice table (nullable initially)
ALTER TABLE `Invoice`
ADD COLUMN `ClientId` INT NULL AFTER `BackorderId`;

-- Step 2: Migrate existing data - copy ClientId from BackOrder to Invoice
-- This ensures all existing invoices inherit their backorder's client
UPDATE `Invoice` i
INNER JOIN `BackOrder` bo ON i.BackorderId = bo.Id
SET i.ClientId = bo.ClientId;

-- Step 3: Add foreign key constraint
-- Using ON DELETE SET NULL to prevent cascade issues
ALTER TABLE `Invoice`
ADD CONSTRAINT `FK_Invoice_Clients_ClientId`
    FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`Id`)
    ON DELETE SET NULL;

-- Note: We intentionally leave ClientId as nullable to:
-- 1. Support existing invoices that might not have been migrated
-- 2. Allow flexibility in the UI (optional override of backorder client)
-- 3. Maintain backwards compatibility

-- ============================================================
-- VERIFICATION QUERIES (run manually to verify migration)
-- ============================================================
-- Check how many invoices were updated:
-- SELECT COUNT(*) as updated_invoices FROM Invoice WHERE ClientId IS NOT NULL;
--
-- Check for any invoices without client (should be 0 after migration):
-- SELECT COUNT(*) as orphan_invoices FROM Invoice WHERE ClientId IS NULL;
--
-- Verify the FK constraint exists:
-- SELECT CONSTRAINT_NAME FROM information_schema.TABLE_CONSTRAINTS
-- WHERE TABLE_NAME = 'Invoice' AND CONSTRAINT_TYPE = 'FOREIGN KEY';
