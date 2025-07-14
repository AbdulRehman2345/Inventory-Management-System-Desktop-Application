CREATE DATABASE InventoryDB;
USE InventoryDB;
SELECT * FROM Products;
 SELECT 
     ID, 
     Barcode, 
     PName AS [Product Name], 
     Category AS [Product Category], 
     PStatus AS [Product Status], 
     DateAdded AS [Date Added]
 FROM Products;
 CREATE TABLE Stocks (
    stock_id INT IDENTITY(1,1) PRIMARY KEY,
    barcode_id INT,
    stock_code NVARCHAR(100),
    quantity INT,
    minimum_stock INT,
    cp_unit DECIMAL(10,2),    
    sp_unit DECIMAL(10,2),     
    arrival_date DATETIME,
    expiry_date DATETIME NULL,
    date_added DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Stock_Product 
        FOREIGN KEY (barcode_id) 
        REFERENCES Products(ID)
);
SELECT * FROM Stocks;
ALTER TABLE Products
ALTER COLUMN Barcode BIGINT;
DROP TABLE Stock;
ALTER TABLE Stocks
ALTER TABLE Stocks
ALTER COLUMN arrival_date DATE;

ALTER TABLE Stocks
ALTER COLUMN expiry_date DATE;
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    Date DATE DEFAULT CAST(GETDATE() AS DATE),
    Subtotal DECIMAL(10, 2) NOT NULL,
    DiscountPercent DECIMAL(5, 2) NOT NULL,
    TotalPayable DECIMAL(10, 2) NOT NULL,
    CashReceived DECIMAL(10, 2) NOT NULL,
    ChangeReturned DECIMAL(10, 2) NOT NULL
);
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    Barcode BIGINT NOT NULL,
    ProductName NVARCHAR(100),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL
);
ALTER TABLE Orders
DROP COLUMN DiscountPercent;
SELECT * FROM OrderDetails;
SELECT * FROM Orders;
DELETE FROM Orders WHERE OrderID = 11;
