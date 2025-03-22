CREATE TABLE Cards(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Firstname NVARCHAR(MAX) NOT NULL,
	Lastname NVARCHAR(MAX) NOT NULL,
	CardNumber NVARCHAR(MAX) NOT NULL,
	ExpirationDate DATE NOT NULL,
	CVV NVARCHAR(MAX) NOT NULL,
	PinCode NVARCHAR(MAX) NOT NULL,
	AccountId INT NOT NULL,

	FOREIGN KEY (AccountId)
	REFERENCES Accounts(Id)
)
GO