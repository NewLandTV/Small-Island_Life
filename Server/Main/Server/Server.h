#ifndef __SERVER_H__
#define __SERVER_H__

#include <WinSock2.h>
#include "../Database/MySQL.h"

#define MAX_BUFFER_SIZE 4096

#pragma comment(lib, "ws2_32.lib")

struct SocketInfo
{
	WSAOVERLAPPED overlapped;
	WSABUF dataBuffer;
	SOCKET socket;
	char messageBuffer[MAX_BUFFER_SIZE];
	int recvBytes;
	int sendBytes;
};

class Server
{
private:
	SocketInfo* socketInfo;
	SOCKET listeningSocket;
	HANDLE iocpHandle;
	HANDLE* workerHandle;

	MySQL mySQL;

	// Flags
	bool accept;
	bool runningWorkerThread;

public:
	Server();
	~Server();

	bool Initialize(unsigned short port);
	void Start();
	bool CreateWorkerThread();
	void WorkerThread();
};

#endif