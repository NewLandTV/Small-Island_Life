#include <jdbc/cppconn/prepared_statement.h>
#include "MySQL.h"

MySQL::MySQL() : driver(get_driver_instance()), connection(driver->connect(HOST_NAME, USER_NAME, PASSWORD))
{
	connection->setSchema("small_island_life");
}

MySQL::~MySQL()
{
	if (connection != nullptr)
	{
		delete connection;
	}
}

int MySQL::GetAccountUID(const char* id, const char* password)
{
	sql::PreparedStatement* preparedStatement = connection->prepareStatement("SELECT uid FROM account WHERE id = ? AND password = ?;");

	preparedStatement->setString(1, id);
	preparedStatement->setString(2, password);

	sql::ResultSet* result = preparedStatement->executeQuery();

	unsigned int uid = result->next() ? result->getInt(1) : 0;

	delete result;
	delete preparedStatement;

	return uid;
}

bool MySQL::CreateAccount(const char* id, const char* password)
{
	sql::PreparedStatement* preparedStatement = connection->prepareStatement("SELECT * FROM account WHERE id = ?;");

	preparedStatement->setString(1, id);

	sql::ResultSet* result = preparedStatement->executeQuery();

	unsigned int row = result->rowsCount();

	delete result;
	delete preparedStatement;

	// 이미 존재하는 계정
	if (row >= 1)
	{
		return false;
	}

	preparedStatement = connection->prepareStatement("INSERT INTO account(id, password) VALUES(?, ?);");

	preparedStatement->setString(1, id);
	preparedStatement->setString(2, password);

	bool finalResult = preparedStatement->execute();

	delete preparedStatement;

	return !finalResult;
}