-- =====================================================
-- Database Optimization Script
-- Fibrasol Delivery System
-- =====================================================
-- This script adds indexes to improve query performance
-- Safe to run on existing databases (uses IF NOT EXISTS)
-- =====================================================

-- =====================================================
-- 1. FOREIGN KEY INDEXES
-- =====================================================
-- These indexes improve JOIN performance significantly

-- DeliveryOrder table
CREATE INDEX IF NOT EXISTS idx_deliveryorder_statusid
    ON DeliveryOrder(StatusId);

-- BackOrder table
CREATE INDEX IF NOT EXISTS idx_backorder_clientid
    ON BackOrder(ClientId);

CREATE INDEX IF NOT EXISTS idx_backorder_deliveryorderid
    ON BackOrder(DeliveryOrderId);

-- Invoice table
CREATE INDEX IF NOT EXISTS idx_invoice_backorderid
    ON Invoice(BackorderId);

CREATE INDEX IF NOT EXISTS idx_invoice_salespersonid
    ON Invoice(SalesPersonId);

-- DeliveryOrderDrivers junction table
CREATE INDEX IF NOT EXISTS idx_deliveryorderdrivers_deliveryorderid
    ON DeliveryOrderDrivers(DeliveryOrderId);

CREATE INDEX IF NOT EXISTS idx_deliveryorderdrivers_driverid
    ON DeliveryOrderDrivers(DriverId);

-- =====================================================
-- 2. FILTER COLUMN INDEXES
-- =====================================================
-- These indexes improve WHERE clause performance

-- Invoice.SignedAttatchment - used in unsigned orders queries
CREATE INDEX IF NOT EXISTS idx_invoice_signedattatchment
    ON Invoice(SignedAttatchment(255));

-- DeliveryOrder.Created - used in date range filters for sales reports
CREATE INDEX IF NOT EXISTS idx_deliveryorder_created
    ON DeliveryOrder(Created);

-- =====================================================
-- 3. SEARCH COLUMN INDEXES
-- =====================================================
-- These indexes improve LIKE and equality searches

-- SalesPerson.Name - used in GetByName queries
CREATE INDEX IF NOT EXISTS idx_salesperson_name
    ON SalesPerson(Name(100));

-- Clients.Name - used in client searches
CREATE INDEX IF NOT EXISTS idx_clients_name
    ON Clients(Name(100));

-- Drivers.Name - used in driver searches
CREATE INDEX IF NOT EXISTS idx_drivers_name
    ON Drivers(Name(100));

-- =====================================================
-- 4. COMPOSITE INDEXES FOR COMPLEX QUERIES
-- =====================================================
-- These indexes optimize specific complex queries

-- Optimize sales report query (SalesPerson → Invoice → BackOrder → DeliveryOrder)
CREATE INDEX IF NOT EXISTS idx_invoice_salespersonid_backorderid
    ON Invoice(SalesPersonId, BackorderId);

-- Optimize unsigned orders query
CREATE INDEX IF NOT EXISTS idx_invoice_signedattatchment_backorderid
    ON Invoice(SignedAttatchment(255), BackorderId);

-- Optimize delivery order queries with status and date
CREATE INDEX IF NOT EXISTS idx_deliveryorder_statusid_created
    ON DeliveryOrder(StatusId, Created);

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
-- Run these queries to verify indexes were created:
--
-- SHOW INDEX FROM DeliveryOrder;
-- SHOW INDEX FROM BackOrder;
-- SHOW INDEX FROM Invoice;
-- SHOW INDEX FROM DeliveryOrderDrivers;
-- SHOW INDEX FROM SalesPerson;
-- SHOW INDEX FROM Clients;
-- SHOW INDEX FROM Drivers;
--
-- =====================================================
-- PERFORMANCE TESTING
-- =====================================================
-- Before running this script, capture baseline performance:
--
-- EXPLAIN SELECT ... (your complex queries)
--
-- After running, compare EXPLAIN results to see improvements
-- =====================================================
