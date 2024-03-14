#include "Server/Server.h"

int main()
{
	Server server;

	if (server.Initialize(5108))
	{
		server.Start();
	}

	return 0;
}