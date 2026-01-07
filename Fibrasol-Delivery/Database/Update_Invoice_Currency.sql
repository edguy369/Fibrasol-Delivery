-- Migration script to add Currency column to Invoice table
-- Currency can be 'Q' (Quetzales) or 'USD' (US Dollars)
-- Default value is 'Q' to maintain compatibility with existing data

ALTER TABLE Invoice ADD COLUMN Currency VARCHAR(3) DEFAULT 'Q';
