CREATE TABLE Transactions(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Amount decimal(18,2) NOT NULL,
	Currency NVARCHAR(3) NOT NULL,
	TransactionDate DATE NOT NULL,
	FromAccountId INT NOT NULL,
	ToAccountId INT NOT NULL,
	IsATM BIT NULL,
	TransactionType INT NULL,
	TransactionFee DECIMAL(18,2) NOT NULL DEFAULT 0,
	FOREIGN KEY (FromAccountId)
	REFERENCES Accounts(Id),

	FOREIGN KEY (ToAccountId)
	REFERENCES Accounts(Id)
)