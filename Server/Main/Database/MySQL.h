#ifndef __MYSQL_H__
#define __MYSQL_H__

#include <jdbc/cppconn/driver.h>
#include "DatabaseConfig.h"

#pragma comment(lib, "mysqlcppconn.lib")

class MySQL
{
private:
	sql::Driver* driver;
	sql::Connection* connection;

public:
	MySQL();
	~MySQL();

	int GetAccountUID(const char* id, const char* password);
	bool CreateAccount(const char* id, const char* password);
};

#endif