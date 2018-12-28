#include "Command.h"
#include "Robot.h"
Command::Command() {

}
queue<Action*> Command::GetActions(Robot& robot)
{
	return queue<Action*>();
}
queue<Command*> Command::GetContainedCommands(Robot& robot)
{
	return queue<Command*>();
}