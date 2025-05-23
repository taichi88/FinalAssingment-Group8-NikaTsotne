CREATE TABLE Cards(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Firstname NVARCHAR(50) NOT NULL,
	Lastname NVARCHAR(50) NOT NULL,
	CardNumber NVARCHAR(100) NOT NULL,
	ExpirationDate DATE NOT NULL,
	CVV NVARCHAR(100) NOT NULL,
	PinCode NVARCHAR(100) NOT NULL,
	AccountId INT NOT NULL,

	FOREIGN KEY (AccountId)
	REFERENCES Accounts(Id)
)